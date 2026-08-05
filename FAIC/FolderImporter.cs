using SharpGen.Runtime.Win32;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text;
using System.Windows.Documents;

namespace FAIC
{
    public partial class FolderImporter : Form
    {
        public struct Result
        {
            public List<string> orderedFrames;
            public readonly IEnumerable<string> ToConcat()
            {
                yield return "ffconcat version 1.0";
                yield return "#Generated with Fast Animated Image Converter";
                yield return "#https://github.com/JaimeShirazi/FastAnimatedImageConverter";
                for (int i = 0; i < orderedFrames.Count; i++)
                {
                    yield return $"file \'{orderedFrames[i]}\'";
                }
            }
            public static bool TryReadFrom(string concatPath, out Result result)
            {
                result = default;

                if (!Path.Exists(concatPath)) return false;

                result.orderedFrames = new List<string>();

                using (StreamReader sr = new StreamReader(concatPath))
                {
                    if (!sr.ReadLine().Equals("ffconcat version 1.0")) return false;

                    while (!sr.EndOfStream)
                    {
                        var s = sr.ReadLine();
                        if (string.IsNullOrEmpty(s)) continue;

                        if (s.StartsWith("file "))
                        {
                            string path = s[5..].Trim('\'');
                            if (!File.Exists(path))
                            {
                                path = Path.Combine(Path.GetDirectoryName(concatPath), path);
                            }

                            if (File.Exists(path))
                            {
                                result.orderedFrames.Add(path);
                            }
                        }
                    }
                }


                return true;
            }
        }
        private string inputPath;
        private Task import;
        private CancellationTokenSource cts;
        private Action<string> onCreated;
        public FolderImporter(string folderPath, Action<string> onConcatCreated)
        {
            InitializeComponent();
            inputPath = folderPath;
            cts = new();
            onCreated = onConcatCreated;
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            if (import != null) return;

            saveConcat.InitialDirectory = Directory.Exists(inputPath) ? inputPath : Path.GetDirectoryName(inputPath);
            saveConcat.FileName = "index.ffcat";
            if (saveConcat.ShowDialog(this) == DialogResult.OK)
            {
                settings.Enabled = false;
                importButton.Enabled = false;
                import = Import(saveConcat.FileName, () =>
                {
                    onCreated.Invoke(saveConcat.FileName);
                    Close();
                });
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
        private async Task Import(string outputPath, Action onWritten)
        {
            if (string.IsNullOrWhiteSpace(inputPath)
                || !Directory.Exists(inputPath))
            {
                MessageBox.Show("Root directory is empty or does not exist.", "Error importing folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (Path.GetFileNameWithoutExtension(outputPath).Length <= 0)
            {
                MessageBox.Show("Output file name must be at least 1 character.", "Error importing folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!Path.HasExtension(outputPath))
            {
                MessageBox.Show("Output file name must have an extension.", "Error importing folder", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string targetPath = Path.Combine(inputPath, outputPath);
            var paths = new List<string>();

            IProgress<string> progress = new Progress<string>((str) => { importLog.Text = str; });

            await Task.Run(() =>
            {
                progress?.Report($"Discovering files...");
                IEnumerable<string> files;
                try
                {
                    files = Directory.EnumerateFiles(inputPath, "*.*", subfoldersCheckbox.Checked ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Directory Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                ulong count = 0, total = 0;
                progress?.Report($"Imported {count} of {total} files...");
                foreach (string path in files)
                {
                    total++;

                    if (extensions.Contains(Path.GetExtension(path)))
                    {
                        count++;
                        paths.Add(path);
                    }

                    if ((count & 255) == 0) //Only update every 256 files
                    {
                        progress?.Report($"Imported {count} of {total} files...");
                    }
                }

                progress?.Report($"Sorting {count} files...");
                paths.Sort(StringComparer.OrdinalIgnoreCase);
            }, cts.Token);

            progress?.Report("Writing ffconcat file...");
            Result result = new Result()
            {
                orderedFrames = paths
            };
            try
            {
                //Explicitly tells the encoder NOT to emit the UTF-8 BOM identifier
                //ffconcat fails when true
                var utf8WithoutBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

                await File.WriteAllLinesAsync(targetPath, result.ToConcat(), utf8WithoutBom, cts.Token);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Imported Index Write Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            progress?.Report("Finished importing.");

            onWritten.Invoke();
        }
        /*private void outputField_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            outputField.Text = string.Concat(outputField.Text.Where(c => !Path.GetInvalidFileNameChars().Contains(c)));
            if (Path.HasExtension(outputField.Text))
            {
                string ext = Path.GetExtension(outputField.Text);
                if (!ext.Equals(".ffcat", StringComparison.OrdinalIgnoreCase)
                    && !ext.Equals(".ffconcat", StringComparison.OrdinalIgnoreCase)
                    && !ext.Equals(".txt", StringComparison.OrdinalIgnoreCase))
                {
                    outputField.Text = Path.GetFileNameWithoutExtension(outputField.Text);
                }
            }
            if (!Path.HasExtension(outputField.Text))
            {
                outputField.Text = outputField.Text.TrimEnd('.') + ".ffcat";
            }
            if (Path.GetFileNameWithoutExtension(outputField.Text).Length <= 0)
            {
                outputField.Text = "index" + outputField.Text;
            }
        }*/
    }
}
