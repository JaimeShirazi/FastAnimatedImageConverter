namespace FAIC.Types
{
    public struct EncodeSettings
    {
        public enum TuningSetting
        {
            Fast, Best
        }
        public enum ResampleSetting
        {
            None, Bilinear, Lanczos, Spline36
        }
        public enum InterpolateSetting
        {
            None, Nearest, Blended
        }
        public string InputPath;
        public string InputFormat;
        public string OutputPath;
        public ConvertJobTarget OutputFormat;
        /// <summary>
        /// From 0 to 100
        /// </summary>
        public int Quality;
        public int Width, Height;
        public ResampleSetting Resample;
        public TuningSetting Tuning;
        public decimal Start, End;
        public double Speed;
        public decimal TargetFrameRate;
        public InterpolateSetting Interpolate;
        public int Repeats;
        public bool Transparent;
        public Func<ArgumentsWindowInputs, ArgumentsWindowOutputs> onBeforeArguments;
        public bool isConcat;
        public EncodeSettings(int largestTargetDimension, int largestOriginalDimension, bool preferFast, bool preferBest)
        {
            if (preferFast)
            {
                Tuning = TuningSetting.Fast;
                if (largestTargetDimension == largestOriginalDimension)
                {
                    Resample = ResampleSetting.None;
                }
                else
                {
                    Resample = ResampleSetting.Bilinear;
                }
            }
            else if (preferBest)
            {
                Tuning = TuningSetting.Best;
                if (largestTargetDimension == largestOriginalDimension)
                {
                    Resample = ResampleSetting.None;
                }
                else if (largestTargetDimension < largestOriginalDimension)
                {
                    Resample = ResampleSetting.Lanczos;
                }
                else
                {
                    Resample = ResampleSetting.Spline36;
                }
            }
            else
            {
                Program.TryOutput(ConsoleMessageType.Warning, "Failed to determine tuning. Defaulting to fast.");
                Tuning = TuningSetting.Fast;
                Resample = ResampleSetting.Bilinear;
            }
        }
        public static bool IsFormatTransparencySupported(string format)
        {
            string lowerFormat = format.ToLower();
            if (lowerFormat == "vp8") return true;
            if (lowerFormat == "vp9") return true;
            return false;
        }
        public override string ToString()
        {
            return $"Input {InputPath}, Output {OutputPath}\n{Quality}% quality, {Width}x{Height} by {Resample}\nFrom {Start} to {End}\n{Speed}x speed at {TargetFrameRate}FPS by {Interpolate}\n";
        }
        public string GetFFmpegArguments()
        {
            string videoFilters = "";
            void AddFilter(string value)
            {
                if (videoFilters.Length > 0) videoFilters += ",";
                videoFilters += value;
            }
            if (Speed != 1)
            {
                AddFilter($"setpts=PTS/{Speed}");
            }
            switch (Resample)
            {
                case ResampleSetting.Bilinear:
                    AddFilter($"scale={Width}:{Height}:flags=bilinear");
                    break;
                case ResampleSetting.Lanczos:
                    AddFilter($"scale={Width}:{Height}:flags=lanczos");
                    break;
                case ResampleSetting.Spline36:
                    AddFilter($"scale={Width}:{Height}:flags=spline");
                    break;
            }
            switch (Interpolate)
            {
                case InterpolateSetting.Nearest:
                    AddFilter($"fps={TargetFrameRate}");
                    break;
                case InterpolateSetting.Blended:
                    AddFilter($"minterpolate=fps={TargetFrameRate}:mi_mode=blend");
                    break;
            }
            if (videoFilters.Length > 0)
            {
                videoFilters = $"-vf \"{videoFilters}\" ";
            }

            string decoderOverride = "";
            if (Transparent
                && IsFormatTransparencySupported(InputFormat))
            {
                switch (InputFormat.ToLower())
                {
                    case "vp8":
                        decoderOverride = "-c:v libvpx ";
                        break;
                    case "vp9":
                        decoderOverride = "-c:v libvpx-vp9 ";
                        break;
                }
            }

            string safeOverride = "";
            if (isConcat)
            {
                decoderOverride = "-f concat ";
                safeOverride = "-safe 0 ";
            }

            return "-y -nostats -stats_period 0.25 -progress pipe:2 -threads 0 " +
                safeOverride +
                $"-ss {Start} -to {End} " +
                decoderOverride +
                $"-i \"{InputPath}\" " +
                "-map 0:v:0 " + //select only video
                videoFilters;
        }
    }
}
