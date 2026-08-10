using System.Diagnostics.CodeAnalysis;

namespace FAIC.Types.Formats
{
    public enum InputCodec
    {
        Generic = 0,
        VP8,
        VP9,
    }
    public static class InputCodecUtils
    {
        public static InputCodec GetTarget(string codec)
        {
            switch (codec.ToLowerInvariant())
            {
                case "vp8":
                    return InputCodec.VP8;
                case "vp9":
                    return InputCodec.VP9;
                default:
                    return InputCodec.Generic;
            }
        }
        public static bool SupportsTransparency(this InputCodec codec) => codec switch
        {
            InputCodec.VP8 => true,
            InputCodec.VP9 => true,
            _ => false
        };
        public static bool RequiresDecoder(this InputCodec codec, [NotNullWhen(true)] out string? decoder)
        {
            decoder = codec switch
            {
                InputCodec.VP8 => "libvpx",
                InputCodec.VP9 => "libvpx-vp9",
                _ => null
            };
            return (decoder != null);
        }
    }
}
