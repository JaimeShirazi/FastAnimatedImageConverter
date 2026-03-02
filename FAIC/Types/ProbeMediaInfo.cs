using System.Globalization;
using System.Text.Json;

namespace FAIC.Types
{
    /// <summary>
    /// Information about media retrieved from ffprobe on open
    /// </summary>
    public class ProbeMediaInfo : IFormattable
    {
        public bool IsEmpty()
        {
            foreach (BaseStreamData data in GetAllData())
            {
                if (data.ReadData) return false;
            }
            return true;
        }
        public double Length => StreamLength.ReadData ? StreamLength : (FormatLength.ReadData ? FormatLength : StreamTagLength.Value);
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
        public ParsedStreamData<FormattedDuration> StreamTagLength = new(key: "DURATION");

        public IEnumerable<BaseStreamData> GetAllStreamData()
        {
            yield return StreamLength;
            yield return AverageFrameRate;
            yield return BaseFrameRate;
            yield return Width;
            yield return Height;
            yield return Codec;
        }
        public IEnumerable<BaseStreamData> GetAllStreamTagData()
        {
            yield return StreamTagLength;
        }
        public IEnumerable<BaseStreamData> GetAllFormatData()
        {
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
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            string fpsStatus = "unknown frame rate";
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
            
            return $"({Length} seconds, {fpsStatus}, {Width.Value}x{Height.Value} resolution, {Codec.Value} encoding)";
        }
        public override string ToString() => ToString(null, CultureInfo.InvariantCulture);
    }
}
