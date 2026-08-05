using FAIC.Types;
using System.Globalization;
using System.IO;
using System.Text;

namespace FAIC
{
    public partial class FolderImporter : Form
    {
        private string[] inputPaths;
        private Task import;
        private CancellationTokenSource cts;
        private Action<string> onCreated;
        public FolderImporter(string[] filesAndFolderPaths, Action<string> onConcatCreated)
        {
            InitializeComponent();
            inputPaths = filesAndFolderPaths;
            cts = new();
            onCreated = onConcatCreated;
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            if (import != null) return;

            if (fpsValue.Value <= 0)
            {
                MessageBox.Show("Cannot use an FPS value less than or equal to 0.", "Import Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            saveConcatDialogue.InitialDirectory = Path.GetDirectoryName(inputPaths[0]);
            int counter;
            string filePrefix = Path.Combine(Path.GetDirectoryName(inputPaths[0]), "ImportedFileList_");
            for (counter = 1; counter < 99999; counter++)
            {
                if (!File.Exists(filePrefix + counter + ".txt")) break;
            }
            saveConcatDialogue.FileName = filePrefix + counter;
            if (saveConcatDialogue.ShowDialog(this) == DialogResult.OK)
            {
                settings.Enabled = false;
                importButton.Enabled = false;
                import = Import(saveConcatDialogue.FileName);
            }

            
        }
        private static readonly HashSet<string> extensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".png", ".jpg", ".jpeg", ".bmp",
            ".tif", ".tiff",
            ".webp", ".avif",
            ".gif", ".apng",
            ".jxl"
        };
        private async Task Import(string outputPath)
        {
            List<string> sortedPaths = await GetOrderedFrameFilesAsync(cts.Token);

            if (!cts.IsCancellationRequested)
            {
                Program.TryOutput(ConsoleMessageType.Success, "Finished building file list.");
                Program.TryOutput(ConsoleMessageType.Progress, "Writing list to file...");

                FrameCollection result = new FrameCollection()
                {
                    stream = sortedPaths,
                    averageFrameRate = fpsValue.Value
                };

                await Task.Run(() =>
                {
                    var encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

                    using (var stream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.Read))
                    {
                        using (var writer = new StreamWriter(stream, encoding))
                        {
                            result.ToConcat(writer);
                        }
                    }
                });

                Program.TryOutput(ConsoleMessageType.Success, "Finished writing list to file!");
                onCreated.Invoke(outputPath);
            }
        }

        public async Task<List<string>> GetOrderedFrameFilesAsync(CancellationToken token)
        {
            return await Task.Run(async () =>
            {
                // Pass 1: count eligible files for a determinate progress bar.
                int total = 0;
                List<int> totals = new();
                for (int i = 0; i < inputPaths.Length; i++)
                {
                    if (string.IsNullOrWhiteSpace(inputPaths[i])
                    || !Path.Exists(inputPaths[i]))
                    {
                        MessageBox.Show($"File or directory \'{inputPaths[i]}\' is empty or does not exist.", "Error importing folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }

                    FileAttributes attr = File.GetAttributes(inputPaths[i]);
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        totals.Add(CountCandidateFiles(
                            inputPaths[i],
                            subfoldersCheckbox.Checked,
                            token));
                        total += totals[^1];

                    }
                    else
                    {
                        if (extensions.Contains(Path.GetExtension(inputPaths[i])))
                        {
                            totals.Add(1);
                            total++;
                        }
                    }
                }

                token.ThrowIfCancellationRequested();

                // Pass 2: collect them while reporting real progress.
                var results = new List<string>(capacity: Math.Max(total, 4));
                int processed = 0;

                for (int i = 0; i < inputPaths.Length; i++)
                {
                    FileAttributes attr = File.GetAttributes(inputPaths[i]);
                    if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    {
                        int start = processed;
                        int refreshCount = 0;
                        foreach (string path in EnumerateCandidateFiles(
                            inputPaths[i],
                            subfoldersCheckbox.Checked,
                            token))
                        {
                            token.ThrowIfCancellationRequested();

                            results.Add(path);
                            processed++;
                            refreshCount++;

                            if (refreshCount > 256) //Only update every 256 files
                            {
                                UpdateProgress(processed, path, total);
                                refreshCount = 0;
                            }
                        }

                        UpdateProgress(total, $"Sorting input #{i}'s valid files...");
                        Program.TryOutput($"Sorting {processed - start} at offset {start}");
                        results.Sort(start, processed - start, StringComparer.OrdinalIgnoreCase);
                    }
                    else
                    {
                        if (extensions.Contains(Path.GetExtension(inputPaths[i])))
                        {
                            results.Add(inputPaths[i]);
                            processed++;
                        }
                    }
                }

                Thread.Sleep(1000);

                return results;
            }, token);
        }

        private int CountCandidateFiles(
            string rootDirectory,
            bool includeSubfolders,
            CancellationToken token)
        {
            int count = 0;

            foreach (string path in EnumerateAllFilesSafe(rootDirectory, includeSubfolders, token))
            {
                token.ThrowIfCancellationRequested();

                if (extensions.Contains(Path.GetExtension(path)))
                    count++;

                if ((count & 255) == 0) //Only update every 256 files
                {
                    UpdateProgress(count, path);
                }
            }

            UpdateProgress(0, "");

            return count;
        }
        void UpdateProgress(int current, string detailedMessage, int total = -1)
        {
            if (InvokeRequired)
            {
                BeginInvoke(() => UpdateProgress(current, detailedMessage, total));
                return;
            }

            if (total <= 0)
            {
                importProgress.Style = ProgressBarStyle.Marquee;
                importLog.Text = $"Found {current} files";
            }
            else
            {
                importProgress.Style = ProgressBarStyle.Blocks;
                importProgress.Maximum = total;
                importProgress.Value = current;
                importLog.Text = $"Processing {current} of {total} files";
            }
            importLog.Text += $"\n{detailedMessage}";
        }
        private static IEnumerable<string> EnumerateCandidateFiles(
            string rootDirectory,
            bool includeSubfolders,
            CancellationToken token)
        {
            foreach (string path in EnumerateAllFilesSafe(rootDirectory, includeSubfolders, token))
            {
                token.ThrowIfCancellationRequested();

                if (extensions.Contains(Path.GetExtension(path)))
                    yield return path;
            }
        }

        private static IEnumerable<string> EnumerateAllFilesSafe(
            string rootDirectory,
            bool includeSubfolders,
            CancellationToken token)
        {
            var pending = new Stack<string>();
            pending.Push(rootDirectory);

            while (pending.Count > 0)
            {
                token.ThrowIfCancellationRequested();
                string currentDir = pending.Pop();

                IEnumerable<string> files;
                try
                {
                    files = Directory.EnumerateFiles(currentDir);
                }
                catch (Exception ex) when (
                    ex is UnauthorizedAccessException ||
                    ex is DirectoryNotFoundException ||
                    ex is IOException)
                {
                    continue;
                }

                foreach (string file in files)
                {
                    token.ThrowIfCancellationRequested();
                    yield return file;
                }

                if (!includeSubfolders)
                    continue;

                IEnumerable<string> subdirs;
                try
                {
                    subdirs = Directory.EnumerateDirectories(currentDir);
                }
                catch (Exception ex) when (
                    ex is UnauthorizedAccessException ||
                    ex is DirectoryNotFoundException ||
                    ex is IOException)
                {
                    continue;
                }

                foreach (string subdir in subdirs)
                {
                    token.ThrowIfCancellationRequested();
                    pending.Push(subdir);
                }
            }
        }
    }
}
