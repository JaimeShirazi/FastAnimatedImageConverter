namespace FAIC.Types.Formats
{
    public enum InputTransfer
    {
        Unsupported,

        //Supported spaces for SDR
        bt709,
        bt470m,
        bt470bg,
        smpte170m,
        smpte240m,
        iec61966_2_1,
        bt2020_10,
        bt2020_12,

        //Supported spaces for HDR
        smpte2084, //Bt.2020 PQ
        arib_std_b67, //Bt.2020 HLG
    }
    public static class InputTransferUtils
    {
        public static bool IsHDR(this InputTransfer transfer)
            => transfer is InputTransfer.smpte2084 or InputTransfer.arib_std_b67;
        public static string ToDisplayString(this InputTransfer transfer, double peakNits) => transfer switch
        {
            InputTransfer.smpte2084 => $"HDR (PQ, Peak: {peakNits})",
            InputTransfer.arib_std_b67 => $"HDR (HLG, Peak: {peakNits})",
            InputTransfer.Unsupported => "Unsupported Dynamic Range",
            _ => "SDR"
        };
        /// <param name="transfer">Value ffprobe returns from color_transfer</param>
        public static InputTransfer GetFromProbe(string transfer)
        {
            return transfer.ToLowerInvariant() switch
            {
                "bt709" => InputTransfer.bt709,
                "bt470m" or "gamma22" => InputTransfer.bt470m,
                "bt470bg" or "gamma28" => InputTransfer.bt470bg,
                "smpte170m" => InputTransfer.smpte170m,
                "smpte240m" => InputTransfer.smpte240m,
                "iec61966-2-1" => InputTransfer.iec61966_2_1,
                "bt2020-10" => InputTransfer.bt2020_10,
                "bt2020-12" => InputTransfer.bt2020_12,
                "smpte2084" => InputTransfer.smpte2084,
                "arib-std-b67" => InputTransfer.arib_std_b67,
                _ => InputTransfer.Unsupported
            };
        }
        public static string ToFFmpegName(this InputTransfer transfer) => transfer switch
        {
            InputTransfer.bt709 => "bt709",
            InputTransfer.bt470m => "bt470m",
            InputTransfer.bt470bg => "bt470bg",
            InputTransfer.smpte170m => "smpte170m",
            InputTransfer.smpte240m => "smpte240m",
            InputTransfer.iec61966_2_1 => "iec61966-2-1",
            InputTransfer.bt2020_10 => "bt2020-10",
            InputTransfer.bt2020_12 => "bt2020-12",
            InputTransfer.smpte2084 => "smpte2084",
            InputTransfer.arib_std_b67 => "arib-std-b67",
            _ => throw new NotSupportedException(),
        };
        /// <returns>The default peak nits for content with this input transfer.</returns>
        public static long TryGetFallbackPeakNits(this InputTransfer transfer) => transfer switch
        {
            InputTransfer.smpte2084 => 10000L, //FFmpeg default peak nits for PQ
            InputTransfer.arib_std_b67 => 1000L, //FFmpeg default peak nits for PQ
            _ => 0L
        };
    }
}