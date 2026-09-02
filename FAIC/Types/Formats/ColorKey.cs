namespace FAIC.Types.Formats
{
    /// <summary>
    /// Lookup for the possible known values of colour primaries, transfer and/or space.
    /// </summary>
    public enum ColorKey
    {
        Unsupported,
        rgb,
        gbr,
        bt709,
        bt470m,
        bt470bg,
        smpte170m,
        smpte240m,
        iec61966_2_1,
        bt2020,
        bt2020_10,
        bt2020_12,
        bt2020nc,
        smpte2084, //Bt.2020 PQ
        arib_std_b67, //Bt.2020 HLG
    }
    public static class ColorKeyUtils
    {
        public static bool TransferIsHDR(this ColorKey transfer)
            => transfer is ColorKey.smpte2084
                        or ColorKey.arib_std_b67;
        public static bool TransferIsHighBit(this ColorKey transfer)
            => transfer is ColorKey.bt2020
                        or ColorKey.bt2020_10
                        or ColorKey.bt2020_12
                        or ColorKey.bt2020nc
                        or ColorKey.smpte2084
                        or ColorKey.arib_std_b67;
        /// <param name="key">Value ffprobe returns from color_transfer</param>
        public static ColorKey FromFFmpegName(string key)
        {
            return key.ToLowerInvariant() switch
            {
                "rgb" => ColorKey.rgb,
                "gbr" => ColorKey.gbr,
                "bt709" => ColorKey.bt709,
                "bt470m" or "gamma22" => ColorKey.bt470m,
                "bt470bg" or "gamma28" => ColorKey.bt470bg,
                "smpte170m" => ColorKey.smpte170m,
                "smpte240m" => ColorKey.smpte240m,
                "iec61966-2-1" => ColorKey.iec61966_2_1,
                "bt2020" => ColorKey.bt2020,
                "bt2020-10" => ColorKey.bt2020_10,
                "bt2020-12" => ColorKey.bt2020_12,
                "bt2020nc" => ColorKey.bt2020nc,
                "smpte2084" => ColorKey.smpte2084,
                "arib-std-b67" => ColorKey.arib_std_b67,
                _ => ColorKey.Unsupported
            };
        }
        public static string ToFFmpegName(this ColorKey key) => key switch
        {
            ColorKey.rgb => "rgb",
            ColorKey.gbr => "gbr",
            ColorKey.bt709 => "bt709",
            ColorKey.bt470m => "bt470m",
            ColorKey.bt470bg => "bt470bg",
            ColorKey.smpte170m => "smpte170m",
            ColorKey.smpte240m => "smpte240m",
            ColorKey.iec61966_2_1 => "iec61966-2-1",
            ColorKey.bt2020 => "bt2020",
            ColorKey.bt2020_10 => "bt2020-10",
            ColorKey.bt2020_12 => "bt2020-12",
            ColorKey.bt2020nc => "bt2020nc",
            ColorKey.smpte2084 => "smpte2084",
            ColorKey.arib_std_b67 => "arib-std-b67",
            _ => throw new System.NotImplementedException($"No FFmpeg name has been defined for key \"{key}\"."),
        };
    }
}