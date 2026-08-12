using System.Globalization;
using System.Text.Json;

namespace FAIC.Types
{
    /// <summary>
    /// Information about media retrieved from ffprobe on open
    /// </summary>
    public class ProbeMediaInfo : IFormattable
    {
        private readonly string inputPath;
        public ProbeMediaInfo(string inputPath)
        {
            this.inputPath = inputPath;
        }
        public bool IsEmpty()
        {
            foreach (BaseStreamData data in GetAllData())
            {
                if (data.ReadData) return false;
            }
            return true;
        }
        public double Length => isConcat
            ? ConcatData.orderedFrames.Count
            : (StreamLength.ReadData ? StreamLength : (FormatLength.ReadData ? FormatLength : StreamTagLength.Value));
        /// <summary>
        /// Best-guess FPS for this media. Negative when no valid FPS was found.
        /// </summary>
        public double EstimatedFrameRate =>
            AverageFrameRate.ReadData && !AverageFrameRate.Value.IsInvalid
            ? (double)AverageFrameRate.Value
            : (!BaseFrameRate.Value.IsInvalid ? (double)BaseFrameRate.Value : -1);

        public ParsedStreamData<double> StreamLength = new(key: "duration");
        public ParsedStreamData<double> FormatLength = new(key: "duration");
        public ParsedStreamData<Fraction> AverageFrameRate = new(key: "avg_frame_rate");
        public ParsedStreamData<Fraction> BaseFrameRate = new(key: "r_frame_rate");
        public ParsedStreamData<int> Width = new(key: "width");
        public ParsedStreamData<int> Height = new(key: "height");
        public StringStreamData Codec = new(key: "codec_name");
        public StringStreamData Format = new(key: "format_name");
        public StringStreamData ColorSpace = new(key: "color_space");
        public StringStreamData ColorPrimaries = new(key: "color_primaries");
        public StringStreamData ColorTransfer = new(key: "color_transfer");
        public StringStreamData ColorRange = new(key: "color_range");
        public ParsedStreamData<FormattedDuration> StreamTagLength = new(key: "DURATION");

        private bool isConcat = false;
        public bool IsConcat
        {
            get => isConcat;
            private set => isConcat = value;
        }
        public FolderImporter.Result ConcatData => concatData;
        private FolderImporter.Result concatData;

        public string GetInputArguments()
        {
            string args = "";
            var sources = new (IEnumerable<BaseStreamData> streams, string source)[]
            {
                (GetAllStreamData(), "stream"),
                (GetAllStreamTagData(), "stream_tags"),
                (GetAllFormatData(), "format")
            };
            for (int i = 0; i < sources.Length; i++)
            {
                args += $":{sources[i].source}=";
                foreach (BaseStreamData data in sources[i].streams)
                {
                    args += $"{data.key},";
                }
                args = args[..^1];
            }
            return args[1..^0];
        }
        public IEnumerable<BaseStreamData> GetAllStreamData()
        {
            yield return StreamLength;
            yield return AverageFrameRate;
            yield return BaseFrameRate;
            yield return Width;
            yield return Height;
            yield return Codec;
            yield return ColorSpace;
            yield return ColorPrimaries;
            yield return ColorTransfer;
            yield return ColorRange;
        }
        public IEnumerable<BaseStreamData> GetAllStreamTagData()
        {
            yield return StreamTagLength;
        }
        public IEnumerable<BaseStreamData> GetAllFormatData()
        {
            yield return Format;
            yield return FormatLength;
        }
        public IEnumerable<BaseStreamData> GetAllData()
        {
            foreach (var data in GetAllStreamData()) yield return data;
            foreach (var data in GetAllStreamTagData()) yield return data;
            foreach (var data in GetAllFormatData()) yield return data;
        }
        public void ReadStream(JsonElement root)
        {
            if (root.TryGetProperty("streams", out var streams)
                && streams.ValueKind == JsonValueKind.Array
                && streams.GetArrayLength() > 0)
            {
                JsonElement stream0 = streams[0];

                foreach (BaseStreamData data in GetAllStreamData())
                {
                    data.TryRead(stream0);
                }

                if (stream0.TryGetProperty("tags", out var stream0tags))
                {
                    foreach (BaseStreamData data in GetAllStreamTagData())
                    {
                        data.TryRead(stream0tags);
                    }
                }
            }

            if (root.TryGetProperty("format", out var format))
            {
                foreach (BaseStreamData data in GetAllFormatData())
                {
                    data.TryRead(format);
                }
            }

            if (Format.ReadData
                && Format.Value.Equals("concat", StringComparison.OrdinalIgnoreCase))
            {
                IsConcat = true;
                if (!FolderImporter.Result.TryReadFrom(inputPath, out concatData))
                {
                    Format.Value = "unknown";
                }
            }
            else IsConcat = false;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            string lengthStatus = "unknown length";
            string fpsStatus = "unknown frame rate";
            if (IsConcat)
            {
                lengthStatus = $"{concatData.orderedFrames.Count} frames";
                fpsStatus = "unspecified frame rate";
            }
            else
            {
                lengthStatus = $"{Length} seconds";
                if (AverageFrameRate.ReadData || BaseFrameRate.ReadData)
                {
                    string fpsSteadiness = "steadiness unknown";
                    if (AverageFrameRate.ReadData && BaseFrameRate.ReadData)
                    {
                        string steadiness = Fraction.IsSteady(AverageFrameRate.Value, BaseFrameRate.Value) ? "steady" : "unsteady";
                        if (!AverageFrameRate.Value.IsInvalid && !BaseFrameRate.Value.IsInvalid)
                        {
                            fpsSteadiness = Fraction.IsSteady(AverageFrameRate.Value, BaseFrameRate.Value) ? "steady" : "unsteady";
                        }
                    }
                    fpsStatus = $"{EstimatedFrameRate} frame rate ({fpsSteadiness})";
                }
            }
            
            return $"({lengthStatus}, {fpsStatus}, {Width.Value}x{Height.Value} resolution, {Codec.Value} encoding in {Format.Value} format. Color space: {ColorSpace.Value}, Color primaries: {ColorPrimaries.Value}, Color transfer: {ColorTransfer.Value}, Color range: {ColorRange.Value}.)";
        }
        public override string ToString() => ToString(null, CultureInfo.InvariantCulture);
    }
}
