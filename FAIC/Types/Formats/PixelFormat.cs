namespace FAIC.Types.Formats
{
    public enum PixelFormat
    {
        yuv420p,
        yuva420p,
        yuv444p,
        yuv420p10le,
        yuva420p16le,
        gray,
        gray10le,
        rgba,
        rgb24,
        rgb48le,
        rgb48be,
        rgba64le,
        rgba64be,
        bgra,
    }
    public static class PixelFormatUtils
    {
        public static string ToFFmpegName(this PixelFormat transfer) => transfer switch
        {
            PixelFormat.yuv420p => "yuv420p",
            PixelFormat.yuva420p => "yuva420p",
            PixelFormat.yuv444p => "yuv444p",
            PixelFormat.yuv420p10le => "yuv420p10le",
            PixelFormat.yuva420p16le => "yuva420p16le",
            PixelFormat.gray => "gray",
            PixelFormat.gray10le => "gray10le",
            PixelFormat.rgba => "rgba",
            PixelFormat.rgb24 => "rgb24",
            PixelFormat.rgb48le => "rgb48le",
            PixelFormat.rgb48be => "rgb48be",
            PixelFormat.rgba64le => "rgba64le",
            PixelFormat.rgba64be => "rgba64be",
            PixelFormat.bgra => "bgra",
            _ => throw new NotSupportedException(),
        };
    }
}
