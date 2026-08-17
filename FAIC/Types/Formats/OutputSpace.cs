namespace FAIC.Types.Formats
{
    [Flags]
    public enum OutputSpace
    {
        Unsupported = 0,
        bt709 = 1 << 0,
        bt2020_PQ = 1 << 1,
        bt2020_HLG = 1 << 2
    }
    public static class OutputSpaceUtils
    {
        public static OutputSpace SupportedSpaces(InputTransfer transfer, OutputCodec outputCodec)
        {
            if (transfer == InputTransfer.Unsupported) return OutputSpace.Unsupported;
            if (!outputCodec.SupportsHDR()) return OutputSpace.bt709;
            switch (transfer)
            {
                case InputTransfer.smpte2084:
                    return OutputSpace.bt709 | OutputSpace.bt2020_PQ;
                case InputTransfer.arib_std_b67:
                    return OutputSpace.bt709 | OutputSpace.bt2020_HLG;
                default:
                    return OutputSpace.bt709;
            }
        }
        public static bool NeedsTonemapping(this OutputSpace space, InputTransfer transfer) => space switch
        {
            OutputSpace.bt709 => transfer != InputTransfer.bt709,
            OutputSpace.bt2020_PQ => transfer != InputTransfer.smpte2084,
            OutputSpace.bt2020_HLG => transfer != InputTransfer.arib_std_b67,
            _ => true,
        };
        public static OutputSpace GetBestFlag(this OutputSpace flags, bool useHDR)
        {
            if (useHDR)
            {
                if (flags.HasFlag(OutputSpace.bt2020_PQ)) return OutputSpace.bt2020_PQ;
                else if (flags.HasFlag(OutputSpace.bt2020_HLG)) return OutputSpace.bt2020_HLG;
            }
            if (flags.HasFlag(OutputSpace.bt709)) return OutputSpace.bt709;
            return OutputSpace.Unsupported;
        }
    }
}
