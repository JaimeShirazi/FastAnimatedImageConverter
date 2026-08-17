using FAIC.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FAIC.Types.Formats
{
    public enum OutputCodec
    {
        AVIF,
        JXL,
        WEBP,
        APNG,
        GIF
    }
    public static class OutputCodecUtils
    {
        public static OutputCodec GetTarget(string path)
        {
            string extension = Path.GetExtension(path).TrimStart('.').ToLowerInvariant();

            switch (extension)
            {
                case "avif":
                default:
                    if (extension != "avif") Program.TryOutput("Error: Unrecognised extension. Outputting as AVIF.");
                    return OutputCodec.AVIF;
                case "gif":
                    return OutputCodec.GIF;
                case "jxl":
                    return OutputCodec.JXL;
                case "apng":
                case "png":
                    return OutputCodec.APNG;
                case "webp":
                    return OutputCodec.WEBP;
            }
        }
        public static bool SupportsTransparency(this OutputCodec _) => true; //currently, all formats support transparency
        public static bool SupportsHDR(this OutputCodec codec) => codec switch
        {
            OutputCodec.AVIF or OutputCodec.JXL or OutputCodec.APNG => true,
            _ => false
        };
    }
}
