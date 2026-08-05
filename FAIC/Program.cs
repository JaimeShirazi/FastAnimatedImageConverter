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
        #region Active job tracker
        private static List<Guid> activeJobs = new();
        public static void RegisterJob(Guid id)
        {
            if (activeJobs.Contains(id)) return;
            activeJobs.Add(id);
        }
        public static void DeregisterJob(Guid id)
        {
            if (!activeJobs.Contains(id)) return;
            activeJobs.Remove(id);
        }
        public static int GetJobIndex(Guid id)
        {
            if (activeJobs.Contains(id))
            {
                return activeJobs.IndexOf(id);
            }
            return -1;
        }
        #endregion
        public static void TryOutput(string text) => latest?.AppendLog(text);
        public static void TryOutput(ConsoleMessageType type, string text) => latest?.AppendWithFormatting(type, text);
        public static void TryOutput(Guid jobGuid, string text) => latest?.AppendLog($"[{GetJobIndex(jobGuid) + 1}] {text}");
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

            ProbeMediaInfo mediaInfo = new(path);

            string extension = Path.GetExtension(path);
            bool isConcat = (extension.Equals(".ffcat", StringComparison.CurrentCultureIgnoreCase)
                || extension.Equals(".ffconcat", StringComparison.CurrentCultureIgnoreCase)
                || extension.Equals(".txt", StringComparison.CurrentCultureIgnoreCase));

            var proc = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments =
                        (isConcat ? $"-safe 0 " : "") +
                        $"-v error " +
                        "-select_streams v:0 " +
                        $"-show_entries {mediaInfo.GetInputArguments()} " + //Duration is sometimes stored in stream_tags instead of stream
                        $"-of json \"{path}\""
                }
            };

            SetupFFProbeProcess(proc, out var stdoutTask);

            try
            {
                await proc.WaitForExitAsync(token);

                string metaJson = await stdoutTask;

                using var doc = JsonDocument.Parse(metaJson);

                mediaInfo.ReadStream(doc.RootElement);
            
                receiveInfoCallback(mediaInfo);
            }
            catch (OperationCanceledException)
            {
                // expected
            }
            catch (Exception e)
            {
                TryOutput(ConsoleMessageType.Error, $"Unable to probe media: {e.Message}");
                receiveInfoCallback(null);
            }
        }
    }
}