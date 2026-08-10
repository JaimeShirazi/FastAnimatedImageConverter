using FAIC.Types.Cuts;
using FAIC.Types.Formats;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;

namespace FAIC.Types
{
    public struct EncodeSettings
    {
        private const string MAP_LABEL = "videoOut";
        private static string FFmpegNumber(decimal value) => value.ToString("G17", CultureInfo.InvariantCulture);
        private static string FFmpegNumber(double value) => value.ToString("G17", CultureInfo.InvariantCulture);
        private static string FFmpegNumber(int value) => value.ToString("G17", CultureInfo.InvariantCulture);
        private static string FFmpegDuration(decimal value) => value.ToString(
            "0.#############################",
            CultureInfo.InvariantCulture);
        public interface IFilterParameters
        {
            public void ToDisplay(StringBuilder builder);
        }
        public struct FilterComplex(List<Segment.FilterChain> chains) : IFilterParameters
        {
            List<Segment.FilterChain> Chains = chains;
            public void ToDisplay(StringBuilder builder)
            {
                builder.Append($"-filter_complex \"[0:v:0]split={Chains.Count}");
                for (int i = 0; i < Chains.Count; i++)
                {
                    builder.Append($"[source{i}]");
                }
                builder.Append(';');
                for (int i = 0; i < Chains.Count; i++)
                {
                    builder.Append($"[source{i}]");
                    Chains[i].ToDisplay(builder);
                    builder.Append($"[segment{i}];");
                }
                for (int i = 0; i < Chains.Count; i++)
                {
                    builder.Append($"[segment{i}]");
                }
                builder.Append($"concat=n={Chains.Count}:v=1:a=0[{MAP_LABEL}]\" ");
            }
        }
        public struct Segment
        {
            public struct FilterConfig(List<(string, string)> config) : IFilterParameters
            {
                public List<(string, string)> Config = config;
                public readonly void ToDisplay(StringBuilder builder)
                {
                    for (int i = 0; i < Config.Count; i++)
                    {
                        if (i > 0) builder.Append(':');
                        builder.Append(Config[i].Item1);
                        builder.Append('=');
                        builder.Append(Config[i].Item2);
                    }
                }
            }
            public struct FilterSetting(string setting) : IFilterParameters
            {
                public string Setting = setting;
                public readonly void ToDisplay(StringBuilder builder)
                {
                    builder.Append(Setting);
                }
            }
            public struct FilterChain(List<(string, IFilterParameters)> chain) : IFilterParameters
            {
                public List<(string, IFilterParameters)> Chain = chain;
                public readonly void ToDisplay(StringBuilder builder)
                {
                    for (int i = 0; i < Chain.Count; i++)
                    {
                        if (i > 0) builder.Append(',');
                        builder.Append(Chain[i].Item1);
                        builder.Append('=');
                        Chain[i].Item2.ToDisplay(builder);
                    }
                }
            }
            public Segment(Cut from, ProbeMediaInfo currentInfo,
                int outputWidth, int outputHeight,
                TuningSetting tuningSetting,
                bool isTransparent,
                double Speed, decimal TargetFrameRate, InterpolateSetting Interpolate)
            {
                Start = from.Start * (decimal)currentInfo.Length;
                End = from.End * (decimal)currentInfo.Length;
                System.Windows.Rect Crop = from.NormalizedCrop;
                this.outputWidth = outputWidth;
                this.outputHeight = outputHeight;
                this.isTransparent = isTransparent;
                this.Speed = Speed;
                this.TargetFrameRate = TargetFrameRate;
                this.Interpolate = Interpolate;

                int sourceWidth = currentInfo.Width;
                int sourceHeight = currentInfo.Height;

                if (sourceWidth <= 0 || sourceHeight <= 0)
                    throw new ArgumentOutOfRangeException(nameof(sourceWidth));

                if (outputWidth <= 0 || outputHeight <= 0)
                    throw new ArgumentOutOfRangeException(nameof(outputWidth));

                const double tolerance = 1e-9;

                double normalizedRight = Crop.X + Crop.Width;
                double normalizedBottom = Crop.Y + Crop.Height;

                if (Crop.Width <= 0.0 ||
                    Crop.Height <= 0.0 ||
                    Crop.X < -tolerance ||
                    Crop.Y < -tolerance ||
                    normalizedRight > 1.0 + tolerance ||
                    normalizedBottom > 1.0 + tolerance)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(Crop),
                        "The crop must be a non-empty normalized rectangle inside 0–1.");
                }

                // Convert normalized crop boundaries to source-pixel boundaries.
                // Rounding boundaries separately avoids accumulating width/position errors.
                static int ToPixelEdge(double normalizedPosition, int sourceSize)
                {
                    return Math.Clamp(
                        (int)Math.Round(
                            normalizedPosition * sourceSize,
                            MidpointRounding.AwayFromZero),
                        0,
                        sourceSize);
                }

                cropX = ToPixelEdge(Crop.X, sourceWidth);
                cropY = ToPixelEdge(Crop.Y, sourceHeight);
                int cropRight = ToPixelEdge(normalizedRight, sourceWidth);
                int cropBottom = ToPixelEdge(normalizedBottom, sourceHeight);

                croppedWidth = cropRight - cropX;
                croppedHeight = cropBottom - cropY;

                if (croppedWidth < 1 || croppedHeight < 1)
                {
                    throw new ArgumentException(
                        "The normalized crop becomes empty after conversion to source pixels.",
                        nameof(Crop));
                }

                // Uniformly enlarge or reduce the crop until one dimension reaches
                // the output canvas boundary.
                double containScale = Math.Min(
                    outputWidth / (double)croppedWidth,
                    outputHeight / (double)croppedHeight);

                scaledWidth = Math.Clamp(
                    (int)Math.Round(
                        croppedWidth * containScale,
                        MidpointRounding.AwayFromZero),
                    1,
                    outputWidth);

                scaledHeight = Math.Clamp(
                    (int)Math.Round(
                        croppedHeight * containScale,
                        MidpointRounding.AwayFromZero),
                    1,
                    outputHeight);

                bool scalingRequired =
                    scaledWidth != croppedWidth ||
                    scaledHeight != croppedHeight;

                bool isUpscaling =
                    scaledWidth > croppedWidth ||
                    scaledHeight > croppedHeight;

                resample = tuningSetting switch
                {
                    TuningSetting.Fast => ResampleSetting.Bilinear,
                    TuningSetting.Best => isUpscaling ? ResampleSetting.Spline36 : ResampleSetting.Lanczos,
                    _ => throw new System.NotImplementedException()
                };
            }
            public decimal Start;
            public decimal End;
            public int croppedWidth, croppedHeight, cropX, cropY, scaledWidth, scaledHeight, outputWidth, outputHeight;
            public ResampleSetting resample;
            public bool isTransparent;
            public double Speed;
            public decimal TargetFrameRate;
            public InterpolateSetting Interpolate;
            public enum ResampleSetting
            {
                None, Bilinear, Lanczos, Spline36
            }
            public static string GetFlag(ResampleSetting setting) => setting switch
            {
                ResampleSetting.None => "neighbor",
                ResampleSetting.Bilinear => "bilinear",
                ResampleSetting.Lanczos => "lanczos",
                ResampleSetting.Spline36 => "spline",
                _ => throw new System.NotImplementedException()
            };
            public readonly FilterChain GetFilters()
            {
                return new FilterChain(
                [
                    ("trim", new FilterConfig([
                                ("start", FFmpegDuration(Start)),
                                ("end", FFmpegDuration(End))
                            ])
                    ),
                    ("settb", new FilterSetting("AVTB")),
                    ("setpts", new FilterSetting($"(PTS-STARTPTS)/{FFmpegNumber(Speed)}")),
                    Interpolate switch
                    {
                        InterpolateSetting.Nearest => ("fps", new FilterSetting(FFmpegNumber(TargetFrameRate))),
                        InterpolateSetting.Blended => ("minterpolate", new FilterConfig([
                                ("fps", FFmpegNumber(TargetFrameRate)),
                                ("mi_mode", "blend"),
                            ])),
                        _ => throw new System.NotImplementedException()
                    },
                    ("format", new FilterSetting("rgba64le")), //helps prevent "green mode"
                    ("crop", new FilterConfig([
                                ("w", FFmpegNumber(croppedWidth)),
                                ("h", FFmpegNumber(croppedHeight)),
                                ("x", FFmpegNumber(cropX)),
                                ("y", FFmpegNumber(cropY)),
                                ("exact", "1"),
                            ])
                    ),
                    ("setsar", new FilterSetting("1")),
                    ("scale", new FilterConfig([
                                ("w", FFmpegNumber(scaledWidth)),
                                ("h", FFmpegNumber(scaledHeight)),
                                ("flags", GetFlag(resample)),
                            ])),
                    ("format", new FilterSetting("rgba")),
                    ("pad", new FilterConfig([
                                ("w", FFmpegNumber(outputWidth)),
                                ("h", FFmpegNumber(outputHeight)),
                                ("x", "floor((ow-iw)/2)"),
                                ("y", "floor((oh-ih)/2)"),
                                ("color", isTransparent ? "black@0" : "black"),
                            ])),
                    ("setsar", new FilterSetting("1")),
                ]
                );
            }
        }
        public enum TuningSetting
        {
            Fast,
            Best
        }
        public enum InterpolateSetting
        {
            Nearest, //When none, nearest is used, and the fps is just matched to the input
            Blended
        }
        public string InputPath;
        public InputFormat InputFormat;
        public InputCodec InputCodec;
        public string OutputPath;
        public OutputCodec OutputFormat;
        /// <summary>
        /// From 0 to 100
        /// </summary>
        public int Quality;
        public int OutputWidth, OutputHeight;
        public TuningSetting Tuning;
        public decimal TargetFrameRate;
        public int Repeats;
        public bool Transparent;
        public int TotalFrames;

        public List<Segment> Segments;

        public Func<ArgumentsWindowInputs, ArgumentsWindowOutputs> onBeforeArguments;
        public EncodeSettings(CutCollection cuts,
            ProbeMediaInfo mediaInfo,
            OutputCodec outputFormat,
            int outputWidth, int outputHeight,
            TuningSetting tuning, bool transparent,
            double speed, decimal targetFrameRate, InterpolateSetting interpolate)
        {
            InputFormat = InputFormatUtils.GetTarget(mediaInfo.Format);
            InputCodec = InputCodecUtils.GetTarget(mediaInfo.Codec);
            Transparent = transparent;
            Segments = new(cuts.Total);
            TargetFrameRate = targetFrameRate;
            Tuning = tuning;
            OutputFormat = outputFormat;
            OutputWidth = outputWidth;
            OutputHeight = outputHeight;
            for (int i = 0; i < cuts.Total; i++)
            {
                decimal length = cuts[i].Start - cuts[i].End;
                TotalFrames += (int)Math.Ceiling(length / TargetFrameRate);
                Segments.Add(new(cuts[i], mediaInfo, OutputWidth, OutputHeight, Tuning, Transparent && OutputFormat.SupportsTransparency(), speed, TargetFrameRate, interpolate));
            }
        }
        public string GetFFmpegArguments()
        {
            StringBuilder builder = new();
            builder.Append("-y -nostats -stats_period 0.25 -progress pipe:2 -threads 0 ");

            if (InputFormat.RequiresContainer(out string container))
            {
                builder.Append($"-f {container} ");
            }

            if (InputFormat.IsUnsafe())
            {
                builder.Append("-safe 0 ");
            }

            if (InputCodec.RequiresDecoder(out string decoder))
            {
                builder.Append($"-c:v {decoder} ");
            }

            builder.Append($"-i \"{InputPath}\" ");

            List<Segment.FilterChain> chains = [];
            for (int i = 0; i < Segments.Count; i++)
            {
                chains.Add(Segments[i].GetFilters());
            }
            new FilterComplex(chains).ToDisplay(builder);

            builder.Append($"-map \"[{MAP_LABEL}]\" ");

            return builder.ToString();
        }
    }
}
