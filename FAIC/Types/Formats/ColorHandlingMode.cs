namespace FAIC.Types.Formats
{
    public enum ColorHandlingMode
    {
        NormalBits,
        HighBits,
        HighDynamicRange
    }
    public static class ColorHandlingModeUtils
    {
        public static PixelFormat GetPixelFormat(this ColorHandlingMode mode, OutputCodec codec, bool transparent)
        {
            //GIF is typically YUV since it's piped to gifski, but when transparent it's output as transparent PNG frames, so we handle it the same way.
            if (codec == OutputCodec.GIF
                && transparent)
            {
                codec = OutputCodec.APNG;
            }

            return mode switch
            {
                ColorHandlingMode.HighDynamicRange or ColorHandlingMode.HighBits => codec switch
                {
                    OutputCodec.AVIF => PixelFormat.yuv420p10le,
                    OutputCodec.JXL => transparent ? PixelFormat.rgba64le : PixelFormat.rgb48le,
                    OutputCodec.APNG => transparent ? PixelFormat.rgba64be : PixelFormat.rgb48be,
                    _ => throw new System.NotImplementedException("No pixel format defined for output codec.")
                },
                ColorHandlingMode.NormalBits or _ => codec switch
                {
                    OutputCodec.AVIF => PixelFormat.yuv420p,
                    OutputCodec.JXL => transparent ? PixelFormat.rgba : PixelFormat.rgb24,
                    OutputCodec.APNG => transparent ? PixelFormat.rgba : PixelFormat.rgb24,
                    OutputCodec.WEBP => PixelFormat.bgra,
                    OutputCodec.GIF => PixelFormat.yuv444p, //Use a less lossy transfer format so gifski has a higher quality source to crush
                    _ => throw new System.NotImplementedException("No pixel format defined for output codec.")
                },
            };
        }
        public static PixelFormat? GetAlphaFormat(this ColorHandlingMode mode, OutputCodec codec) => mode.GetPixelFormat(codec, true) switch
        {
            PixelFormat.yuv420p => PixelFormat.gray,
            PixelFormat.yuv420p10le => PixelFormat.gray10le,
            _ => null
        };
        public static PixelFormat? GetColorAndAlphaMerged(this ColorHandlingMode mode, OutputCodec codec) => mode.GetPixelFormat(codec, true) switch
        {
            PixelFormat.yuv420p => PixelFormat.yuva420p,
            PixelFormat.yuv420p10le => PixelFormat.yuva420p16le,
            _ => null
        };
        public static ColorKey GetMatrix(this ColorHandlingMode mode, OutputCodec codec, bool transparent)
        {
            //GIF is typically YUV since it's piped to gifski, but when transparent it's output as transparent PNG frames, so we handle it the same way.
            if (codec == OutputCodec.GIF
                && transparent)
            {
                codec = OutputCodec.APNG;
            }

            return mode switch
            {
                ColorHandlingMode.HighDynamicRange => codec switch
                {
                    OutputCodec.AVIF => ColorKey.bt2020nc,
                    OutputCodec.JXL => ColorKey.gbr,
                    _ => throw new System.NotImplementedException("No color matrix defined for output codec.")
                },
                ColorHandlingMode.NormalBits or ColorHandlingMode.HighBits or _ => codec switch
                {
                    OutputCodec.AVIF or OutputCodec.GIF => ColorKey.bt709,
                    OutputCodec.JXL or OutputCodec.WEBP or OutputCodec.APNG => ColorKey.gbr,
                    _ => throw new System.NotImplementedException("No color matrix defined for output codec.")
                },
            };
        }
        public static ColorKey GetPrimaries(this ColorHandlingMode mode) => mode switch
        {
            ColorHandlingMode.HighDynamicRange => ColorKey.bt2020,
            ColorHandlingMode.NormalBits or ColorHandlingMode.HighBits or _ => ColorKey.bt709,
        };
        public static ColorKey GetTransfer(this ColorHandlingMode mode) => mode switch
        {
            ColorHandlingMode.HighDynamicRange => ColorKey.smpte2084,
            ColorHandlingMode.NormalBits or ColorHandlingMode.HighBits or _ => ColorKey.iec61966_2_1,
        };
        public static bool IsFullRange(this ColorHandlingMode mode, OutputCodec codec) => mode switch
        {
            ColorHandlingMode.HighDynamicRange => codec switch
            {
                OutputCodec.AVIF => false,
                OutputCodec.JXL => true,
                _ => throw new System.NotImplementedException("No color matrix defined for output codec.")
            },
            ColorHandlingMode.NormalBits or ColorHandlingMode.HighBits or _ => true,
        };
        
    }
}
