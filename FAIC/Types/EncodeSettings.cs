using FAIC.Properties;

namespace FAIC.Types
{
    public struct EncodeSettings
    {
        public enum ResampleSetting
        {
            None, Bilinear, Lanczos, Spline36
        }
        public enum InterpolateSetting
        {
            None, Nearest, Blended
        }
        public string InputPath;
        public string OutputPath;
        /// <summary>
        /// From 0 to 100
        /// </summary>
        public int Quality;
        public int Width, Height;
        public ResampleSetting Resample;
        public decimal Start, End;
        public double Speed;
        public decimal TargetFrameRate;
        public InterpolateSetting Interpolate;
        public int Repeats;
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

            return $"-ss {Start} -to {End} " +
                $"-i \"{InputPath}\" " +
                "-map 0:v:0 " + //select only video
                videoFilters;
        }
    }
}
