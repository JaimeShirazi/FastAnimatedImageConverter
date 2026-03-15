using FAIC.Types;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;

namespace FAIC
{
    internal static class Program
    {
        public static void TryOutput(string text) => latest?.Write(text);
        private static IConsole latest;
        public static void UpdateConsole(IConsole console) => latest = console;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.EnableVisualStyles();
            ApplicationConfiguration.Initialize();
            Application.Run(args.Length > 0 ? new Main(args[0]) : new(""));
        }
        const int BUFFER_SIZE = 32 * 1024 * 1024; //32MB
        private static Func<(bool redirectStdin, bool redirectStdout), Process> PrepareProcess(EncodeSettings settings, string filename, string programName, string arguments)
        {
            if (settings.onBeforeArguments != null)
            {
                ArgumentsWindowOutputs outputs = settings.onBeforeArguments.Invoke(new()
                {
                    arguments = arguments,
                    programName = programName
                });
                if (outputs.confirmed)
                    arguments = outputs.arguments;
            }

            TryOutput(Path.GetFileName(filename) + " " + arguments);

            return ((bool redirectStdin, bool redirectStdout) setup) => CreateProcess(filename, arguments, setup.redirectStdin, setup.redirectStdout);
        }
        private static Process CreateProcess(string filename, string arguments, bool redirectStdin, bool redirectStdout)
        {
            Process process = new Process()
            {
                StartInfo = new()
                {
                    FileName = filename,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardInput = redirectStdin,
                    RedirectStandardOutput = redirectStdout,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true
            };
            process.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    TryOutput(e.Data);
            };

            return process;
        }
        private static async Task EncodeWithFFmpegPipe(EncodeSettings settings, Func<(bool redirectStdin, bool redirectStdout), Process> createReceiver, CancellationToken token, string pixfmt = "yuv444p")
        {
            string arguments = settings.GetFFmpegArguments() +
                        $"-threads 0 -pix_fmt {pixfmt} -strict -1 " +
                        "-f yuv4mpegpipe -";

            Process ffmpeg = PrepareProcess(settings, Path.Combine(AppContext.BaseDirectory, "ffmpeg.exe"), "ffmpeg", arguments).Invoke((false, true));
            Process receiver = createReceiver.Invoke((true, false));

            receiver.Start();
            ffmpeg.Start();

            ffmpeg.BeginErrorReadLine();
            receiver.BeginErrorReadLine();

            using var registration = token.Register(() =>
            {
                try
                {
                    if (!receiver.HasExited)
                        receiver.StandardInput.Close();
                }
                catch { }

                try
                {
                    if (!ffmpeg.HasExited)
                        ffmpeg.Kill(entireProcessTree: true);
                }
                catch { }

                try
                {
                    if (!receiver.HasExited)
                        receiver.Kill(entireProcessTree: true);
                }
                catch { }
            });

            try
            {
                if (receiver.HasExited)
                    throw new Exception($"Receiver exited early.");

                await ffmpeg.StandardOutput.BaseStream.CopyToAsync(
                    receiver.StandardInput.BaseStream,
                    BUFFER_SIZE,
                    token);

                receiver.StandardInput.Close();

                await ffmpeg.WaitForExitAsync(token);
                await receiver.WaitForExitAsync(token);

                ffmpeg = null;
            }
            catch (OperationCanceledException)
            {
                TryOutput("Encode cancelled by user.");
            }
            catch
            {
                throw;
            }
            finally
            {
                if (ffmpeg != null)
                {
                    try
                    {
                        if (!ffmpeg.HasExited)
                            ffmpeg.Kill(entireProcessTree: true);
                    }
                    catch (InvalidOperationException)
                    {
                        // Process already exited
                    }
                    catch (System.ComponentModel.Win32Exception)
                    {
                        // Access denied or process already terminating
                    }
                }
            }
        }
        private static async Task EncodeWithFFmpeg(EncodeSettings settings, string arguments, CancellationToken token, bool omitLoops = false, string pixfmt = "rgba", string outputPath = "")
        {
            int loops = Math.Min(settings.Repeats + 1, 0);
            string targetOutput = string.IsNullOrEmpty(outputPath) ? settings.OutputPath : outputPath;

            string allArguments = settings.GetFFmpegArguments() +
                        $"-r {settings.TargetFrameRate} ";

            if (!omitLoops) allArguments += $"-loop {loops} ";

            allArguments += $"-threads 0 -pix_fmt {pixfmt} " +
                        arguments +
                        $"\"{targetOutput}\"";

            Process ffmpeg = PrepareProcess(settings, Path.Combine(AppContext.BaseDirectory, "ffmpeg.exe"), "ffmpeg", allArguments).Invoke((false, false));

            ffmpeg.Start();

            ffmpeg.BeginErrorReadLine();

            using var registration = token.Register(() =>
            {
                try
                {
                    if (!ffmpeg.HasExited)
                        ffmpeg.Kill(entireProcessTree: true);
                }
                catch { }
            });

            try
            {
                await ffmpeg.WaitForExitAsync(token);

                TryOutput("Complete!");
            }
            catch (OperationCanceledException)
            {
                TryOutput("Encode cancelled by user.");
            }
        }
        public static async Task EncodeAVIF(EncodeSettings settings, CancellationToken token)
        {
            string repetition = settings.Repeats < 0 ? "infinite" : (settings.Repeats + 1).ToString();
            string arguments = $"--stdin --jobs all -q {Math.Max(settings.Quality, 1)} --repetition-count {repetition} \"{settings.OutputPath}\"";

            var createAvifenc = PrepareProcess(settings, Path.Combine(AppContext.BaseDirectory, "avifenc.exe"), "avifenc", arguments);

            try
            {
                await EncodeWithFFmpegPipe(settings, createAvifenc, token, pixfmt: settings.Transparent ? "yuva444p" : "yuv444p");
            }
            catch (Exception e)
            {
                TryOutput("Error " + e.Message);
            }
            finally
            {
                TryOutput("Complete!");
            }
        }
        public static async Task EncodeWebP(EncodeSettings settings, CancellationToken token)
        {
            string arguments = "-c:v libwebp_anim ";

            if (settings.Quality >= 100)
            {
                //Use -lossless 1
                arguments += "-lossless 1 ";
            }
            else
            {
                int webpQ = (int)Math.Round(100 * Math.Sqrt(Math.Clamp(settings.Quality, 0, 100) / 100.0));
                arguments += $"-lossless 0 -q:v {webpQ} ";
            }

            TryOutput("WARNING: progress does not currently display for WebP, but it is still processing. This is an issue with ffmpeg. It will say 0 frames, but it is still processing, please wait until you see the complete message.");
            TryOutput("");

            await EncodeWithFFmpeg(settings, arguments, token, pixfmt: settings.Transparent ? "yuva420p" : "yuv420p");
        }
        public static async Task EncodeJXL(EncodeSettings settings, CancellationToken token)
        {
            if (settings.Repeats >= 0) TryOutput("WARNING: Repeat count not currently supported for JXL. Output file will loop indefinitely.");

            string arguments = "-c:v libjxl_anim ";

            double q = Math.Clamp(settings.Quality, 0, 100) / 100.0;
            const double MAX_DISTANCE = 15.0;
            int jxlDistance = (int)Math.Round(MAX_DISTANCE * Math.Pow(1.0 - q, 2.0));
            int jxlEffort = settings.Quality < 50
                ? (settings.Resample == EncodeSettings.ResampleSetting.Bilinear ? 5 : 7) //Take a hint from if the user is rescaling bilinearly (fast) or not (best/no resize)
                : 7;

            arguments += $"-distance {jxlDistance} -effort {jxlEffort} ";

            arguments += "-f rawvideo ";

            await EncodeWithFFmpeg(settings, arguments, token);
        }
        public static async Task EncodeAPNG(EncodeSettings settings, CancellationToken token)
        {
            string playsFormat = settings.Repeats < 0 ? "0" : (settings.Repeats + 1).ToString();
            string arguments = $"-plays {playsFormat} ";

            arguments += "-f apng ";

            int apngCompression = (int)Math.Round(9 * (1.0 - settings.Quality / 100.0));
            arguments += $"-compression_level {apngCompression} ";

            await EncodeWithFFmpeg(settings, arguments, token, omitLoops: true);
        }
        public static async Task EncodeGIF(EncodeSettings settings, CancellationToken token)
        {
            if (settings.Transparent)
            {
                switch (MessageBox.Show(
                        "Outputing GIF with transparency. This requires that temporary PNG frames are generated. Depending on your media, this may result in high temporary storage usage. Ensure that your system can handle the output resolution and framerate before proceeding.",
                        "Higher Demand in GIF Transparent Mode",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning
                        )
                    )
                {
                    case DialogResult.Cancel:
                        TryOutput("Operation cancelled by user.");
                        return;
                }
            }
            if (settings.Width > 800 || settings.Height > 800)
            {
                switch (MessageBox.Show(
                        $"Outputing GIF with size {settings.Width}x{settings.Height}. This may result in large file sizes. You may proceed, or consider resizing to a smaller resolution and/or using a more modern format with better compression.",
                        "Large Output Resolution",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning
                        )
                    )
                {
                    case DialogResult.Cancel:
                        TryOutput("Operation cancelled by user.");
                        return;
                }
            }

            string repetition = "--repeat " + settings.Repeats switch
            {
                < 0 => 0, //forever = 0
                0 => -1, //-1 = once
                _ => settings.Repeats
            } + " ";
            
            string arguments = $"-Q {Math.Max(settings.Quality, 1)} {repetition}--width={settings.Width} --height={settings.Height} ";
            if (settings.Transparent) arguments += $"-r {settings.TargetFrameRate} ";
            arguments += $"-o \"{settings.OutputPath}\" ";

            string transparentTempFramesDirectory = Path.Combine(Path.GetTempPath(), "FAIC_" + Guid.NewGuid().ToString("N"));

            if (settings.Transparent)
            {
                arguments += "frame_*.png";
            }
            else
            {
                arguments += "-"; //trailing - indicates stdin input
            }

            var createGifski = PrepareProcess(settings, Path.Combine(AppContext.BaseDirectory, "gifski.exe"), "gifski", arguments);

            if (settings.Transparent)
            {
                int frameDigits = (int)Math.Floor(Math.Log10((double)((settings.End - settings.Start) * settings.TargetFrameRate))) + 1;
                
                try
                {
                    Directory.CreateDirectory(transparentTempFramesDirectory);

                    await EncodeWithFFmpeg(settings, "", token, outputPath: Path.Combine(transparentTempFramesDirectory, $"frame_%0{frameDigits}d.png"));

                    TryOutput("Done preparing frames.");

                    Process gifski = createGifski.Invoke((false, false));
                    gifski.StartInfo.WorkingDirectory = transparentTempFramesDirectory;

                    gifski.Start();

                    gifski.BeginErrorReadLine();

                    using var registration = token.Register(() =>
                    {
                        try
                        {
                            if (!gifski.HasExited)
                                gifski.Kill(entireProcessTree: true);
                        }
                        catch { }
                    });

                    await gifski.WaitForExitAsync(token);
                }
                catch (OperationCanceledException)
                {
                    TryOutput("Encode cancelled by user.");
                }
                finally
                {
                    if (Directory.Exists(transparentTempFramesDirectory))
                    {
                        TryOutput("Cleaning up temporary files...");
                        Directory.Delete(transparentTempFramesDirectory, recursive: true);
                    }
                    TryOutput("Complete!");
                }
            }
            else
            {
                try
                {
                    await EncodeWithFFmpegPipe(settings, createGifski, token);
                }
                catch { }
                finally
                {
                    TryOutput("Complete!");
                }
            }
        }
        public static async Task GetMediaInfo(string path, CancellationToken token, Action<ProbeMediaInfo> receiveInfoCallback)
        {
            void SetupFFProbeProcess(Process target, out Task<string> stdout)
            {
                target.StartInfo.FileName = Path.Combine(AppContext.BaseDirectory, "ffprobe.exe");
                target.StartInfo.RedirectStandardOutput = true;
                target.StartInfo.RedirectStandardError = true;
                target.StartInfo.UseShellExecute = false;
                target.StartInfo.CreateNoWindow = true;

                target.ErrorDataReceived += (_, e) =>
                {
                    if (!string.IsNullOrWhiteSpace(e.Data))
                        TryOutput(e.Data);
                };

                target.Start();

                stdout = target.StandardOutput.ReadToEndAsync();
                target.BeginErrorReadLine();

                using var registration = token.Register(() =>
                {
                    try
                    {
                        if (!target.HasExited)
                            target.Kill(entireProcessTree: true);
                    }
                    catch { }
                });
            }

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments =
                        $"-v error " +
                        "-select_streams v:0 " +
                        "-show_entries stream=codec_name,width,height,avg_frame_rate,r_frame_rate,duration:stream_tags=DURATION:format=duration " + //Duration is sometimes stored in stream_tags instead of stream
                        $"-of json \"{path}\""
                }
            };

            SetupFFProbeProcess(proc, out var stdoutTask);

            try
            {
                await proc.WaitForExitAsync(token);

                string metaJson = await stdoutTask;

                using var doc = JsonDocument.Parse(metaJson);

                ProbeMediaInfo mediaInfo = new();
                mediaInfo.ReadStream(doc.RootElement);
            
                receiveInfoCallback(mediaInfo);
            }
            catch (OperationCanceledException)
            {
                // expected
            }
            catch (Exception e)
            {
                TryOutput($"Unable to probe media: {e.Message}");
                receiveInfoCallback(null);
            }
        }
    }
}