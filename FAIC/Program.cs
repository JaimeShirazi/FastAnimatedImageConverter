using FAIC.Types;
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
            ApplicationConfiguration.Initialize();
            Application.Run(args.Length > 0 ? new Main(args[0]) : new(""));
        }
        const int BUFFER_SIZE = 32 * 1024 * 1024; //32MB
        private static async Task EncodeWithFFmpegPipe(EncodeSettings settings, Process receiver, CancellationToken token)
        {
            receiver.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    TryOutput(e.Data);
            };

            string arguments = settings.GetFFmpegArguments() +
                        "-threads 0 -pix_fmt yuva444p " +
                        "-f yuv4mpegpipe -";

            TryOutput("ffmpeg.exe " + arguments);

            Process ffmpeg = new Process()
            {
                StartInfo = new()
                {
                    FileName = Path.Combine(AppContext.BaseDirectory, "ffmpeg.exe"),
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true
            };
            ffmpeg.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    TryOutput(e.Data);
            };

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
                await ffmpeg.StandardOutput.BaseStream.CopyToAsync(
                    receiver.StandardInput.BaseStream,
                    BUFFER_SIZE,
                    token);

                receiver.StandardInput.Close();

                await ffmpeg.WaitForExitAsync(token);
                await receiver.WaitForExitAsync(token);
            }
            catch (OperationCanceledException)
            {
                TryOutput("Encode cancelled by user.");
            }
        }
        private static async Task EncodeWithFFmpeg(EncodeSettings settings, string arguments, CancellationToken token)
        {
            int loops = Math.Min(settings.Repeats + 1, 0);
            string allArguments = settings.GetFFmpegArguments() +
                        $"-r {settings.TargetFrameRate} " +
                        $"-loop {loops} " +
                        "-threads 0 -pix_fmt rgba " +
                        arguments +
                        $"\"{settings.OutputPath}\"";

            TryOutput("ffmpeg.exe " + allArguments);

            Process ffmpeg = new Process()
            {
                StartInfo = new()
                {
                    FileName = Path.Combine(AppContext.BaseDirectory, "ffmpeg.exe"),
                    Arguments = allArguments,
                    UseShellExecute = false,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                },
                EnableRaisingEvents = true
            };
            ffmpeg.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    TryOutput(e.Data);
            };

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
            TryOutput("avifenc.exe " + arguments);
            Process avifenc = new Process()
            {
                StartInfo = new()
                {
                    FileName = Path.Combine(AppContext.BaseDirectory, "avifenc.exe"),
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            try
            {
                await EncodeWithFFmpegPipe(settings, avifenc, token);
            }
            catch { }
            finally
            {
                TryOutput("Complete!");
            }
        }
        public static async Task EncodeWebP(EncodeSettings settings, CancellationToken token)
        {
            string arguments = "-c:v libwebp ";

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

            await EncodeWithFFmpeg(settings, arguments, token);
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
            string arguments = "-f apng ";

            int apngCompression = (int)Math.Round(9 * (1.0 - settings.Quality / 100.0));
            arguments += $"-compression_level {apngCompression} ";

            await EncodeWithFFmpeg(settings, arguments, token);
        }
        public static async Task EncodeGIF(EncodeSettings settings, CancellationToken token)
        {
            string repetition = settings.Repeats < 0 ? "--repeat 0 " : $"--repeat {settings.Repeats} ";
            string arguments = $"-Q {Math.Max(settings.Quality, 1)} {repetition}-o \"{settings.OutputPath}\" -"; //trailing - indicates stdin input

            TryOutput("gifski.exe " + arguments);

            Process gifski = new Process()
            {
                StartInfo = new()
                {
                    FileName = Path.Combine(AppContext.BaseDirectory, "gifski.exe"),
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardInput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                },
                EnableRaisingEvents = true
            };

            try
            {
                await EncodeWithFFmpegPipe(settings, gifski, token);

                if (settings.Repeats == 0)
                {
                    //gifski is weird and I couldn't get it to write exactly one playthrough to the gif reliably, so if the user wants the gif to only play once, we need to manually fix the tag in the file.
                    TryOutput("Stripping netscape header to play exactly once...");

                    byte[] data = await File.ReadAllBytesAsync(settings.OutputPath, token);
                    byte[] marker = Encoding.ASCII.GetBytes("NETSCAPE2.0");

                    bool match = true;
                    for (int i = 0; i < data.Length - marker.Length; i++)
                    {
                        match = true;
                        for (int j = 0; j < marker.Length; j++)
                        {
                            if (data[i + j] != marker[j])
                            {
                                match = false;
                                break;
                            }
                        }
                        if (!match) continue;

                        //Make sure it actually is the full 19 byte block
                        if (i < 3)
                        {
                            match = false;
                            continue;
                        }
                        if (data[i - 3] != 0x21 || data[i - 2] != 0xFF || data[i - 1] != 0x0B)
                        {
                            match = false;
                            continue;
                        }

                        int blockStart = i - 3;
                        const int BLOCK_LENGTH = 19; //full Netscape extension block length

                        byte[] newData = new byte[data.Length - BLOCK_LENGTH];
                        Buffer.BlockCopy(data, 0, newData, 0, blockStart);
                        Buffer.BlockCopy(data, blockStart + BLOCK_LENGTH,
                                         newData, blockStart,
                                         data.Length - (blockStart + BLOCK_LENGTH));

                        await File.WriteAllBytesAsync(settings.OutputPath, newData, token);
                        break;
                    }

                    if (match)
                    {
                        TryOutput("Done overwriting loop count.");
                    }
                    else
                    {
                        TryOutput("Failed to overwrite loop count. GIF will loop infinitely.");
                    }
                }
            }
            catch { }
            finally
            {
                TryOutput("Complete!");
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
            }
        }
    }
}