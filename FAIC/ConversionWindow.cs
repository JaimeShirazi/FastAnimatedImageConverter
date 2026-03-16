using System;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Windows.Forms;

namespace FAIC
{
    public partial class ConversionWindow : Form
    {
        public struct Inputs
        {
            public Process ffmpeg;
            public string outputPath;
            public double expectedLength;
        }

        private Guid jobId;
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();
        private readonly System.Windows.Forms.Timer timer = new();
        private string outputPath;
        private Dictionary<string, string> progressAccum = new();
        private CancellationTokenSource cancellationTokenSource;
        private double expectedLength;
        private bool isPipe;
        private bool complete;
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
        public ConversionWindow(Inputs inputs, CancellationTokenSource cancellationTokenSource)
        {
            InitializeComponent();
            jobId = Guid.NewGuid();
            Program.RegisterJob(jobId);
            outputPath = inputs.outputPath;
            expectedLength = inputs.expectedLength;
            timer.Interval = 100;
            timer.Tick += (_, _) => UpdateUI();
            timer.Start();
            inputs.ffmpeg.ErrorDataReceived += (_, e) =>
            {
                TryReceiveUpdate(e.Data);
            };
            this.cancellationTokenSource = cancellationTokenSource;
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
        private void UpdateUI()
        {
            double seconds = Math.Truncate(stopwatch.Elapsed.TotalSeconds);
            Text = $"Conversion Job {Program.GetJobIndex(jobId) + 1} (busy for {seconds}s)";
            if (buffer.changed)
            {
                ApplyBuffer();
            }
        }
        private void TryOutput(string packet)
        {
            Program.TryOutput($"[{Program.GetJobIndex(jobId) + 1}] {packet}");
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

            if (key == "progress")
            {
                FlushProgress();
            }
        }
        private struct StateBuffer
        {
            public bool changed;
            public string FrameStatsLabel
            {
                get => frameStatsLabel;
                set
                {
                    changed = true;
                    frameStatsLabel = value;
                }
            }
            private string frameStatsLabel;
            public string SizeStatsLabel
            {
                get => sizeStatsLabel;
                set
                {
                    changed = true;
                    sizeStatsLabel = value;
                }
            }
            private string sizeStatsLabel;
            public double EncodeProgressBarNormalised
            {
                get => encodeProgressBarNormalised;
                set
                {
                    changed = true;
                    encodeProgressBarNormalised = value;
                }
            }
            private double encodeProgressBarNormalised;
            public ProgressBarStyle EncodeProgressBarStyle
            {
                get => encodeProgressBarStyle;
                set
                {
                    changed = true;
                    encodeProgressBarStyle = value;
                }

            }
            private ProgressBarStyle encodeProgressBarStyle;
        }
        private StateBuffer buffer;
        private void ApplyBuffer()
        {
            buffer.changed = false;
            frameStatsLabel.Text = string.IsNullOrEmpty(buffer.FrameStatsLabel) ? "" : buffer.FrameStatsLabel;
            sizeStatsLabel.Text = string.IsNullOrEmpty(buffer.SizeStatsLabel) ? "" : buffer.SizeStatsLabel;
            encodeProgressBar.Value = (int)(encodeProgressBar.Maximum * buffer.EncodeProgressBarNormalised);
            encodeProgressBar.Style = buffer.EncodeProgressBarStyle;
        }
        private void FlushProgress()
        {
            if (progressAccum.ContainsKey("progress"))
            {
                if (progressAccum["progress"].Trim().ToLower() == "end")
                {
                    buffer.FrameStatsLabel = "Finalising...";
                    buffer.EncodeProgressBarStyle = ProgressBarStyle.Marquee;
                    buffer.EncodeProgressBarNormalised = 1;
                    progressAccum.Clear();
                    return;
                }
            }

            int frame = 0;
            double fps = 0;
            string speed = "";
            if (progressAccum.ContainsKey("frame"))
            {
                int.TryParse(progressAccum["frame"], out frame);
            }
            if (progressAccum.ContainsKey("fps"))
            {
                double.TryParse(progressAccum["fps"], out fps);
            }
            if (progressAccum.ContainsKey("speed"))
            {
                speed = progressAccum["speed"];
            }
            buffer.FrameStatsLabel = $"{frame} frames at {speed} ({fps:N2}fps)";
            if (isPipe)
            {
                buffer.SizeStatsLabel = "";
            }
            else
            {
                long size = 0;
                if (progressAccum.ContainsKey("total_size"))
                {
                    if (long.TryParse(progressAccum["total_size"], out size))
                    {
                        size /= 1000;
                    }
                }
                buffer.SizeStatsLabel = $"{size}kB";
            }
            if (progressAccum.ContainsKey("out_time_ms"))
            {
                if (double.TryParse(progressAccum["out_time_ms"], out double timeMs))
                {
                    buffer.EncodeProgressBarNormalised = (timeMs * 0.001) / expectedLength;
                }
            }
            buffer.EncodeProgressBarStyle = ProgressBarStyle.Blocks;

            progressAccum.Clear();
        }
        public void Complete()
        {
            complete = true;
            closeButton.Enabled = true;
            closeAndShowButton.Enabled = true;
        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!complete)
            {
                cancellationTokenSource?.Cancel();
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
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
