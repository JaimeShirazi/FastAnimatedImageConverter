using FAIC.Types.Cuts;
using FAIC.Types.Formats;
using System.Globalization;
using System.Text;

namespace FAIC.Types
{
    public struct EncodeSettings
    {
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
        public struct FilterComplex : IFilterParameters
        {
            public const string MASTER_LABEL = "masterOut";

            public const string COLOR_SOURCE_LABEL = "colorSource";
            public const string ALPHA_SOURCE_LABEL = "alphaSource";
            public const string COLOR_MAP_LABEL = "colorOut";
            public const string ALPHA_MAP_LABEL = "alphaOut";

            private readonly List<Segment.FilterChain> Chains;
            private readonly OutputCodec TargetCodec;
            private readonly ColorHandlingMode ColorHandling;
            private readonly bool transparent;
            public FilterComplex(List<Segment> segments, OutputCodec targetCodec, ColorHandlingMode colorHandling, bool transparent)
            {
                TargetCodec = targetCodec;
                ColorHandling = colorHandling;
                this.transparent = transparent;
                Chains = [];
                for (int i = 0; i < segments.Count; i++)
                {
                    Chains.Add(segments[i].GetFilters(colorHandling, targetCodec));
                }
            }
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

                bool alphaInSecondStream = TargetCodec.TransparencyInSecondStream() && transparent;
                string concatOutput = alphaInSecondStream ? MASTER_LABEL : COLOR_MAP_LABEL;
                builder.Append($"concat=n={Chains.Count}:v=1:a=0[{concatOutput}]");
                if (alphaInSecondStream)
                {
                    string range = ColorHandling.IsFullRange(TargetCodec) ? "full" : "limited";
                    builder.Append(
                        $";[{MASTER_LABEL}]format={ColorHandling.GetColorAndAlphaMerged(TargetCodec).Value.ToFFmpegName()},split=2[{COLOR_SOURCE_LABEL}][{ALPHA_SOURCE_LABEL}];" +
                        $"[{COLOR_SOURCE_LABEL}]" +
                        $"format={ColorHandling.GetPixelFormat(TargetCodec, true).ToFFmpegName()}," +
                        $"setparams=range={range}:" +
                        $"color_primaries={ColorHandling.GetPrimaries().ToFFmpegName()}:" +
                        $"color_trc={ColorHandling.GetTransfer().ToFFmpegName()}:" +
                        $"colorspace={ColorHandling.GetMatrix(TargetCodec, true).ToFFmpegName()}" +
                        $"[{COLOR_MAP_LABEL}];" +
                        $"[{ALPHA_SOURCE_LABEL}]" +
                        "alphaextract," +
                        $"format={ColorHandling.GetAlphaFormat(TargetCodec).Value.ToFFmpegName()}," +
                        "setparams=range=full:" +
                        "color_primaries=unknown:" +
                        "color_trc=unknown:" +
                        "colorspace=unknown" +
                        $"[{ALPHA_MAP_LABEL}]");
                }
                builder.Append("\" ");
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
                Start = from.Start * (decimal)currentInfo.BestLength;
                End = from.End * (decimal)currentInfo.BestLength;
                System.Windows.Rect Crop = from.NormalizedCrop;
                this.outputWidth = outputWidth;
                this.outputHeight = outputHeight;
                this.isTransparent = isTransparent;
                this.Speed = Speed;
                this.TargetFrameRate = TargetFrameRate;
                this.Interpolate = Interpolate;
                tuning = tuningSetting;

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
            }
            public decimal Start;
            public decimal End;
            public int croppedWidth, croppedHeight, cropX, cropY, scaledWidth, scaledHeight, outputWidth, outputHeight;
            public TuningSetting tuning;
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
                ResampleSetting.None => "nearest",
                ResampleSetting.Bilinear => "bilinear",
                ResampleSetting.Lanczos => "lanczos",
                ResampleSetting.Spline36 => "spline36",
                _ => throw new System.NotImplementedException()
            };
            public readonly FilterChain GetFilters(ColorHandlingMode mode, OutputCodec codec)
            {
                List<(string, IFilterParameters)> source =
                [
                    ("trim", new FilterConfig([
                                ("start", FFmpegDuration(Start)),
                                ("end", FFmpegDuration(End))
                            ])
                    ),
                    ("settb", new FilterSetting("AVTB")),
                    ("setpts", new FilterSetting($"(PTS-STARTPTS)/{FFmpegNumber(Speed)}")),
                ];

                switch (Interpolate)
                {
                    case InterpolateSetting.Nearest:
                        source.Add(("fps", new FilterSetting(FFmpegNumber(TargetFrameRate))));
                        break;
                    case InterpolateSetting.Blended:
                        source.Add(("minterpolate", new FilterConfig(
                            [
                                ("fps", FFmpegNumber(TargetFrameRate)),
                                ("mi_mode", "blend"),
                            ])));
                        break;
                }

                string libplaceboFormat = mode.GetPixelFormat(codec, isTransparent).ToFFmpegName();
                if (codec.TransparencyInSecondStream() && isTransparent)
                {
                    libplaceboFormat = mode.GetColorAndAlphaMerged(codec).Value.ToFFmpegName();
                }

                List<(string, string)> libplaceboArgs = [
                            // Output canvas.
                            ("w", FFmpegNumber(outputWidth)),
                            ("h", FFmpegNumber(outputHeight)),

                            // Scale the cropped input and place it within that canvas.
                            ("pos_w", FFmpegNumber(scaledWidth)),
                            ("pos_h", FFmpegNumber(scaledHeight)),
                            ("pos_x", "floor((ow-pw)/2)"),
                            ("pos_y", "floor((oh-ph)/2)"),

                            // Transparent black is important when preserving alpha.
                            ("fillcolor", isTransparent ? "black@0" : "black"),

                            ("upscaler", GetFlag(tuning switch{
                                TuningSetting.Best => ResampleSetting.Spline36,
                                TuningSetting.Fast or _ => ResampleSetting.Bilinear
                            })),
                            ("downscaler", GetFlag(tuning switch{
                                TuningSetting.Best => ResampleSetting.Lanczos,
                                TuningSetting.Fast or _ => ResampleSetting.Bilinear
                            })),

                            ("format", libplaceboFormat),
                            ("colorspace", mode.GetMatrix(codec, isTransparent).ToFFmpegName()),
                            ("color_primaries", mode.GetPrimaries().ToFFmpegName()),
                            ("color_trc", mode.GetTransfer().ToFFmpegName()),
                            ("range", mode.IsFullRange(codec) ? "pc" : "tv"),

                            //Shared tonemapping technique
                            ("tonemapping", "bt.2390"),
                            ("gamut_mode", "perceptual"),
                            ("inverse_tonemapping", "0"),
                            ("peak_detect", "1"),
                            ("smoothing_period", "20"),
                    ];

                if (isTransparent) libplaceboArgs.Add(("alpha_mode", "straight"));

                source.AddRange(
                [
                    ("crop", new FilterConfig([
                                ("w", FFmpegNumber(croppedWidth)),
                                ("h", FFmpegNumber(croppedHeight)),
                                ("x", FFmpegNumber(cropX)),
                                ("y", FFmpegNumber(cropY)),
                                ("exact", "1"),
                            ])
                    ),
                    ("libplacebo", new FilterConfig(libplaceboArgs)),
                    ("setsar", new FilterSetting("1")),
                ]);

                return new FilterChain(source);
            }
        }
        public enum TuningSetting
        {
            Fast,
            Best
        }
        public enum InterpolateSetting
        {
            None,
            Nearest,
            Blended
        }
        public string InputPath;
        public InputFormat InputFormat;
        public InputCodec InputCodec;
        public string OutputPath;
        public OutputCodec OutputFormat;
        public ColorHandlingMode ColorMode;
        /// <summary>
        /// From 0 to 100
        /// </summary>
        public int Quality;
        public int OutputWidth, OutputHeight;
        public TuningSetting Tuning;
        public decimal TargetFrameRate;
        public int Repeats;
        public bool Transparent;
        public decimal ExpectedLength;

        public List<Segment> Segments;

        public Func<ArgumentsWindowInputs, ArgumentsWindowOutputs> onBeforeArguments;
        public EncodeSettings(CutCollection cuts,
            ProbeMediaInfo mediaInfo,
            OutputCodec outputFormat,
            int outputWidth, int outputHeight,
            TuningSetting tuning, bool transparent, ColorHandlingMode colorMode,
            double speed, decimal targetFrameRate, InterpolateSetting interpolate)
        {
            InputFormat = InputFormatUtils.GetTarget(mediaInfo.Format);
            InputCodec = InputCodecUtils.GetTarget(mediaInfo.Codec);
            Transparent = transparent && OutputFormat.SupportsTransparency();
            ColorMode = colorMode;
            Segments = new(cuts.Total);
            TargetFrameRate = targetFrameRate;
            Tuning = tuning;
            OutputFormat = outputFormat;
            OutputWidth = outputWidth;
            OutputHeight = outputHeight;
            decimal expectedLength = cuts.GetTotalLengthRatio() * (decimal)mediaInfo.BestLength;
            ExpectedLength = InputFormat == InputFormat.Concat
                ? expectedLength / TargetFrameRate
                : expectedLength;
            for (int i = 0; i < cuts.Total; i++)
            {
                Segments.Add(new(cuts[i], mediaInfo, OutputWidth, OutputHeight, Tuning, Transparent, speed, TargetFrameRate, interpolate));
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

            new FilterComplex(Segments, OutputFormat, ColorMode, Transparent).ToDisplay(builder);

            builder.Append($"-map \"[{FilterComplex.COLOR_MAP_LABEL}]\" ");
            if (Transparent && OutputFormat.TransparencyInSecondStream())
            {
                builder.Append($"-map \"[{FilterComplex.ALPHA_MAP_LABEL}]\" ");
            }

            return builder.ToString();
        }
    }
}
