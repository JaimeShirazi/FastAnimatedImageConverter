using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace FAIC
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            using (var stream = new MemoryStream(Properties.Resources.About))
            {
                aboutText.LoadFile(stream, RichTextBoxStreamType.RichText);
            }
            aboutText.DetectUrls = true;

            var version = Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;

            versionLabel.Text = $"Version {FormatVersion(version)}";
        }

        private static string FormatVersion(string? raw)
        {
            if (string.IsNullOrEmpty(raw))
                return "Unknown";

            int plusIndex = raw.IndexOf('+');

            string main = "";
            string hash = "";
            if (plusIndex < 0)
            {
                main = raw;
            }
            else
            {
                main = raw.Substring(0, plusIndex);
                hash = raw.Substring(plusIndex + 1);
            }

            if (hash.Length > 7)
                hash = hash.Substring(0, 7);

            return hash.Length > 0 ? $"{main} ({hash})" : $"{main}";
        }
    }
}
