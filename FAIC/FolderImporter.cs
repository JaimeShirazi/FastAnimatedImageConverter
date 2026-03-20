using System.IO;

namespace FAIC
{
    public partial class FolderImporter : Form
    {
        public struct Result
        {
            public List<string> orderedFrames;
            public decimal frameRate;
            public string ToConcat()
            {
                string output = "ffconcat version 1.0";
                decimal time = 0;
                for (int i = 0; i < orderedFrames.Count; i++)
                {
                    output += $"\nfile \'{orderedFrames[i]}\'";
                    decimal nextTime = i / frameRate;
                    output += $"\nduration {nextTime - time}";
                    time = nextTime;
                }
                return output;
            }
        }
        private string inputPath;
        private Task import;
        private CancellationTokenSource cts;
        public FolderImporter(string folderPath)
        {
            InitializeComponent();
            inputPath = folderPath;
            cts = new();
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            if (import != null) return;
            settings.Enabled = false;
            importButton.Enabled = false;
            import = Import();
            //TODO: Save the result to a txt file, output it inside the folder, and automatically set the file as the input path of the program.
        }
        private static readonly HashSet<string> extensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".png", ".jpg", ".jpeg", ".bmp",
            ".tif", ".tiff",
            ".webp", ".avif",
            ".gif", ".apng",
            ".jxl"
        };
        private async Task<Result> Import()
        {
            List<string> sortedPaths = await GetOrderedFrameFilesAsync(cts.Token);

            if (!cts.IsCancellationRequested)
            {
                return new Result()
                {
                    orderedFrames = sortedPaths,
                    frameRate = fpsValue.Value
                };
            }
            else
            {
                return new Result();
            }
        }

        public async Task<List<string>> GetOrderedFrameFilesAsync(CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(inputPath)
                || !Directory.Exists(inputPath))
            {
                MessageBox.Show("Root directory is empty or does not exisdt.", "Error importing folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return await Task.Run(() =>
            {
                // Pass 1: count eligible files for a determinate progress bar.
                int total = CountCandidateFiles(
                    inputPath,
                    subfoldersCheckbox.Checked,
                    token);

                token.ThrowIfCancellationRequested();

                // Pass 2: collect them while reporting real progress.
                var results = new List<string>(capacity: Math.Max(total, 4));
                int processed = 0;

                foreach (string path in EnumerateCandidateFiles(
                    inputPath,
                    subfoldersCheckbox.Checked,
                    token))
                {
                    token.ThrowIfCancellationRequested();

                    results.Add(path);
                    processed++;

                    UpdateProgress(processed, path, total);
                }
                UpdateProgress(total, $"Sorting {results.Count} valid files...");
                results.Sort(StringComparer.OrdinalIgnoreCase);
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
