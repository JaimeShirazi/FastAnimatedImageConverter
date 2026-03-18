using FAIC.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAIC.Types
{
    public enum ConvertJobTarget
    {
        AVIF,
        JXL,
        WEBP,
        APNG,
        GIF
    }
    public static class ConvertJobTargetUtils
    {
        public static ConvertJobTarget GetTarget(string path)
        {
            string extension = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();

            switch (extension)
            {
                case "avif":
                default:
                    if (extension != "avif") Program.TryOutput("Error: Unrecognised extension. Outputting as AVIF.");
                    return ConvertJobTarget.AVIF;
                case "gif":
                    return ConvertJobTarget.GIF;
                case "jxl":
                    return ConvertJobTarget.JXL;
                case "apng":
                case "png":
                    return ConvertJobTarget.APNG;
                case "webp":
                    return ConvertJobTarget.WEBP;
            }
        }
    }
}
