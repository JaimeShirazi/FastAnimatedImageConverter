using FAIC.Types;
using FAIC.Types.Formats;
using System.Diagnostics;
using System.IO;
using System.Windows.Controls;

namespace FAIC
{
    public partial class ConversionWindow : Form
    {
        private Guid jobId;
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();
        private readonly System.Windows.Forms.Timer timer = new();
        private string outputPath;

        private Task conversionTask;
        private CancellationTokenSource cancellationTokenSource;
        private decimal expectedLength;
        private decimal frameRate;
        
        public ConversionWindow(EncodeSettings settings)
        {
            InitializeComponent();
            AutoSize = false;
            MinimumSize = new Size(Width, Height);
            int widest = Screen.PrimaryScreen.WorkingArea.Width;
            for (int i = 0; i < Screen.AllScreens.Length; i++)
            {
                widest = Math.Max(Screen.AllScreens[i].WorkingArea.Width, widest);
            }
            MaximumSize = new Size(widest, Height);
            FormBorderStyle = FormBorderStyle.Sizable;

            cancellationTokenSource = new();

            jobId = Guid.NewGuid();
            Program.RegisterJob(jobId);
            outputPath = settings.OutputPath;
            expectedLength = Math.Max(settings.ExpectedLength, 0);
            frameRate = settings.TargetFrameRate;
            timer.Interval = 1000;
            timer.Tick += (_, _) =>
            {
                double seconds = Math.Truncate(stopwatch.Elapsed.TotalSeconds);
                Text = $"Conversion Job {Program.GetJobIndex(jobId) + 1} (busy for {seconds}s)";
            };
            timer.Start();
            isUnknown = settings.OutputFormat == OutputCodec.WEBP;

            //TODO: CONCAT FRAME DURATION & SPEED/FPS NOT WORKING CORRECTLY
            conversionTask = settings.OutputFormat switch
            {
                OutputCodec.AVIF => EncodeAVIF(settings, cancellationTokenSource.Token),
                OutputCodec.JXL => EncodeJXL(settings, cancellationTokenSource.Token),
                OutputCodec.WEBP => EncodeWebP(settings, cancellationTokenSource.Token),
                OutputCodec.APNG => EncodeAPNG(settings, cancellationTokenSource.Token),
                OutputCodec.GIF => EncodeGIF(settings, cancellationTokenSource.Token),
            };
        }
        private void TryOutput(string packet) => Program.TryOutput(jobId, packet);
        private static void TryOutput(string packet, ConsoleMessageType type) => Program.TryOutput(type, packet);

        #region Progress handling
        private Dictionary<string, string> progressAccum = new();
        private bool isPipe;
        private bool isUnknown;
        private bool complete;

        #region Progress called from within
        public void RegisterFfmpeg(Process ffmpeg)
        {
            ffmpeg.ErrorDataReceived += (_, e) =>
            {
                TryReceiveUpdate(e.Data);
            };
        }
        public void RegisterPipe(Process pipe)
        {
            isPipe = true;
            pipe.ErrorDataReceived += (_, e) =>
            {
                if (!string.IsNullOrWhiteSpace(e.Data))
                    TryOutput(e.Data);
            };
        }
        public void Complete()
        {
            complete = true;
            closeButton.Enabled = true;
            closeAndShowButton.Enabled = true;
            cancelButton.Enabled = false;
            encodeProgressBar.Style = ProgressBarStyle.Blocks;
            { //Hack to instantly set the position of the progress bar
                encodeProgressBar.Value = encodeProgressBar.Maximum;
                encodeProgressBar.Value -= 1;
                encodeProgressBar.Value += 1;
            }
            frameStatsLabel.Text = "Done";
            stopwatch.Stop();
        }
        #endregion

        private static bool IsFfmpegProgressKey(string key)
        {
            return key switch
            {
                "frame" => true,
                "fps" => true,
                "bitrate" => true,
                "total_size" => true,
                "out_time_us" => true,
                "out_time_ms" => true,
                "out_time" => true,
                "dup_frames" => true,
                "drop_frames" => true,
                "speed" => true,
                "progress" => true,
                _ when key.StartsWith("stream_") && key.EndsWith("_q") => true,
                _ => false
            };
        }
        private void TryReceiveUpdate(string packet)
        {
            if (string.IsNullOrEmpty(packet)) return;
            int split = packet.IndexOf('=');
            if (split <= 0)
            {
                TryOutput(packet);
                return;
            }

            string key = packet[..split];
            string value = packet[(split + 1)..];

            if (!IsFfmpegProgressKey(key))
            {
                TryOutput(packet);
                return;
            }

            if (!progressAccum.ContainsKey(key)) progressAccum.Add(key, value);
            else progressAccum[key] = value;

            TryFlushProgress();
        }
        private struct StateBuffer
        {
            public string FrameStatsLabel;
            public string SizeStatsLabel;
            public decimal TimeMs;
            public ProgressBarStyle EncodeProgressBarStyle;
        }
        private StateBuffer buffer;
        private void TryApplyBuffer(StateBuffer target)
        {
            if (complete) return;
            frameStatsLabel.Text = string.IsNullOrEmpty(target.FrameStatsLabel) ? "" : target.FrameStatsLabel;
            sizeStatsLabel.Text = string.IsNullOrEmpty(target.SizeStatsLabel) ? "" : target.SizeStatsLabel;
            int targetValue = Math.Min((int)(encodeProgressBar.Maximum * ((target.TimeMs * 0.000001m) / expectedLength)), encodeProgressBar.Maximum);
            if (Math.Clamp(targetValue, encodeProgressBar.Minimum, encodeProgressBar.Maximum) != targetValue)
            {
                //TODO: we need to identify why this is happening sometimes and fix the root issue
                Program.TryOutput(ConsoleMessageType.Warning, $"Clamping issue, attempted value was {targetValue} and range is {encodeProgressBar.Minimum} and {encodeProgressBar.Maximum}.");
                targetValue = Math.Clamp(targetValue, encodeProgressBar.Minimum, encodeProgressBar.Maximum);
            }
            encodeProgressBar.Value = expectedLength != 0
                ? targetValue
                : encodeProgressBar.Minimum;
            encodeProgressBar.Style = target.EncodeProgressBarStyle;
        }
        private void TryFlushProgress()
        {
            if (!progressAccum.ContainsKey("progress")) return;

            if (isUnknown)
            {
                buffer.FrameStatsLabel = "Converting...";
                buffer.EncodeProgressBarStyle = ProgressBarStyle.Marquee;
                buffer.TimeMs = expectedLength * 1000000;
            }
            else if (progressAccum["progress"].Trim().ToLower() == "end")
            {
                buffer.FrameStatsLabel = "Finalising...";
                buffer.EncodeProgressBarStyle = ProgressBarStyle.Marquee;
                buffer.TimeMs = expectedLength * 1000000;
            }
            else
            {
                if (!(progressAccum.TryGetValue("frame", out string frameStr) && int.TryParse(frameStr, out int frame)))
                    frame = 0;
                if (!(progressAccum.TryGetValue("fps", out string fpsStr) && decimal.TryParse(fpsStr, out decimal fps)))
                    fps = 0;
                if (!(progressAccum.TryGetValue("speed", out string speed)))
                    speed = "";

                buffer.FrameStatsLabel = $"{frame} frames at {speed} ({fps:N2}fps)";
                if (isPipe)
                {
                    buffer.SizeStatsLabel = "";
                }
                else
                {
                    long size = 0;
                    if (progressAccum.TryGetValue("total_size", out string totalSize))
                    {
                        if (long.TryParse(totalSize, out size))
                        {
                            size /= 1000;
                        }
                    }
                    buffer.SizeStatsLabel = $"{size}kB";
                }
                if (progressAccum.TryGetValue("out_time_ms", out string outTimeMs))
                {
                    if (!decimal.TryParse(outTimeMs, out buffer.TimeMs))
                        buffer.TimeMs = 0;
                }
                if (buffer.TimeMs < 1)
                {
                    //Try using frames as a time estimate
                    buffer.TimeMs = (frame / frameRate) * 1000000m;
                }
                buffer.EncodeProgressBarStyle = ProgressBarStyle.Blocks;
            }

            progressAccum.Clear();

            if (InvokeRequired)
            {
                BeginInvoke(() => TryApplyBuffer(buffer));
            }
            else
            {
                TryApplyBuffer(buffer);
            }
        }
        #endregion

        #region Cancel/close panel
        protected override async void OnFormClosing(FormClosingEventArgs e)
        {
            if (!complete)
            {
                cancellationTokenSource?.Cancel();

                if (conversionTask != null)
                    await conversionTask;
            }

            base.OnFormClosing(e);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void closeAndShowButton_Click(object sender, EventArgs e)
        {
            if (System.IO.File.Exists(outputPath))
            {
                Process.Start("explorer.exe", "/select," + outputPath);
            }
            else
            {
                MessageBox.Show("File not found: " + outputPath);
            }
            Close();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region Encode handling
        const int BUFFER_SIZE = 32 * 1024 * 1024; //32MB
        private Func<(bool redirectStdin, bool redirectStdout), Process> PrepareProcess(EncodeSettings settings, string filename, string programName, string arguments)
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

            TryOutput(Path.GetFileName(filename) + " " + arguments, ConsoleMessageType.System);

            return ((bool redirectStdin, bool redirectStdout) setup) => CreateProcess(filename, arguments, setup.redirectStdin, setup.redirectStdout);
        }
        private Process CreateProcess(string filename, string arguments, bool redirectStdin, bool redirectStdout)
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

            return process;
        }
        private async Task EncodeWithFFmpegPipe(EncodeSettings settings, Func<(bool redirectStdin, bool redirectStdout), Process> createReceiver, CancellationToken token, string pixfmt)
        {
            string arguments = settings.GetFFmpegArguments() +
                        $"-threads 0 -pix_fmt {pixfmt} -strict -1 " +
                        "-f yuv4mpegpipe -";

            Process ffmpeg = PrepareProcess(settings, Path.Combine(AppContext.BaseDirectory, "ffmpeg.exe"), "ffmpeg", arguments).Invoke((false, true));
            RegisterFfmpeg(ffmpeg);
            Process receiver = createReceiver.Invoke((true, false));
            RegisterPipe(receiver);

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

                Complete();

                ffmpeg = null;
            }
            catch (OperationCanceledException)
            {
                TryOutput("Encode cancelled by user.", ConsoleMessageType.Error);
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
        private async Task EncodeWithFFmpeg(EncodeSettings settings, string arguments, CancellationToken token, string pixfmt, bool omitLoops = false, string outputPath = "", bool suppressComplete = false)
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
            RegisterFfmpeg(ffmpeg);

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
                Complete();
                if (!suppressComplete)
                {
                    TryOutput("Complete!", ConsoleMessageType.Success);
                }
            }
            catch (OperationCanceledException)
            {
                TryOutput("Encode cancelled by user.", ConsoleMessageType.Error);
            }
        }
        public async Task EncodeAVIF(EncodeSettings settings, CancellationToken token)
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
                TryOutput(e.Message, ConsoleMessageType.Error);
            }
            finally
            {
                TryOutput("Complete!", ConsoleMessageType.Success);
            }
        }
        public async Task EncodeWebP(EncodeSettings settings, CancellationToken token)
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

            TryOutput("Progress does not currently display for WebP, but it is still processing. This is an issue with ffmpeg. It will say 0 frames, but it is still processing, please wait until you see the complete message.", ConsoleMessageType.Warning);

            await EncodeWithFFmpeg(settings, arguments, token, "bgra");
        }
        public async Task EncodeJXL(EncodeSettings settings, CancellationToken token)
        {
            if (settings.Repeats >= 0) TryOutput("Repeat count not currently supported for JXL. Output file will loop indefinitely.", ConsoleMessageType.Warning);

            string arguments = "-c:v libjxl_anim ";

            double q = Math.Clamp(settings.Quality, 0, 100) / 100.0;
            const double MAX_DISTANCE = 15.0;
            int jxlDistance = (int)Math.Round(MAX_DISTANCE * Math.Pow(1.0 - q, 2.0));
            int jxlEffort = settings.Quality switch
            {
                < 50 => settings.Tuning switch
                {
                    EncodeSettings.TuningSetting.Best => 7,
                    EncodeSettings.TuningSetting.Fast or _ => 5
                },
                > 90 => settings.Tuning switch
                {
                    EncodeSettings.TuningSetting.Best => 9,
                    EncodeSettings.TuningSetting.Fast or _ => 7
                },
                _ => 7
            };

            arguments += $"-distance {jxlDistance} -effort {jxlEffort} ";

            arguments += "-f rawvideo ";

            await EncodeWithFFmpeg(settings, arguments, token, settings.Transparent ? "rgba" : "rgb24");
        }
        public async Task EncodeAPNG(EncodeSettings settings, CancellationToken token)
        {
            string playsFormat = settings.Repeats < 0 ? "0" : (settings.Repeats + 1).ToString();
            string arguments = $"-plays {playsFormat} ";

            arguments += "-f apng ";

            int apngCompression = (int)Math.Round(9 * (1.0 - settings.Quality / 100.0));
            arguments += $"-compression_level {apngCompression} ";

            await EncodeWithFFmpeg(settings, arguments, token, settings.Transparent ? "rgba" : "rgb24", omitLoops: true);
        }
        public async Task EncodeGIF(EncodeSettings settings, CancellationToken token)
        {
            if (settings.Transparent)
            {
                TryOutput("Outputing GIF with transparency. This requires that temporary PNG frames are generated. Depending on your media, this may result in high temporary storage usage. Ensure that your system can handle the output resolution and framerate before proceeding.", ConsoleMessageType.Warning);
            }
            if (settings.OutputWidth > 800 || settings.OutputHeight > 800)
            {
                switch (MessageBox.Show(
                        $"Outputing GIF with size {settings.OutputWidth}x{settings.OutputHeight}. This may result in large file sizes. You may proceed, or consider resizing to a smaller resolution and/or using a more modern format with better compression.",
                        "Large Output Resolution",
                        MessageBoxButtons.OKCancel,
                        MessageBoxIcon.Warning
                        )
                    )
                {
                    case DialogResult.Cancel:
                        TryOutput("Operation cancelled by user.", ConsoleMessageType.Error);
                        return;
                }
            }

            string repetition = "--repeat " + settings.Repeats switch
            {
                < 0 => 0, //forever = 0
                0 => -1, //-1 = once
                _ => settings.Repeats
            } + " ";

            string arguments = $"-Q {Math.Max(settings.Quality, 1)} {repetition}--width={settings.OutputWidth} --height={settings.OutputHeight} ";
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
                int expectedFrames = (int)Math.Ceiling(settings.InputFormat == InputFormat.Concat
                    ? settings.ExpectedLength * settings.TargetFrameRate
                    : settings.ExpectedLength / settings.TargetFrameRate
                    );

                int frameDigits = (int)Math.Floor(Math.Log10(expectedFrames)) + 1;

                try
                {
                    Directory.CreateDirectory(transparentTempFramesDirectory);

                    //TODO: NOW THAT MULTIPLE ENCODES CAN HAPPEN SIMULTANEOUSLY, THERE NEEDS TO BE UNIQUE FRAME CACHES
                    await EncodeWithFFmpeg(settings, "", token, "rgba", outputPath: Path.Combine(transparentTempFramesDirectory, $"frame_%0{frameDigits}d.png"), suppressComplete: true);

                    TryOutput("Done preparing frames.", ConsoleMessageType.Progress);

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
                    TryOutput("Encode cancelled by user.", ConsoleMessageType.Error);
                }
                finally
                {
                    if (Directory.Exists(transparentTempFramesDirectory))
                    {
                        TryOutput("Cleaning up temporary files...", ConsoleMessageType.Progress);
                        Directory.Delete(transparentTempFramesDirectory, recursive: true);
                    }
                    TryOutput("Complete!", ConsoleMessageType.Success);
                }
            }
            else
            {
                try
                {
                    await EncodeWithFFmpegPipe(settings, createGifski, token, "yuv444p");
                }
                catch { }
                finally
                {
                    TryOutput("Complete!", ConsoleMessageType.Success);
                }
            }
        }
        #endregion
    }
}
