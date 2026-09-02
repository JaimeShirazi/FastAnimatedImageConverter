using FAIC.Types.Formats;
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
        public double BestLength => isConcat
            ? ConcatData.orderedFrames.Count
            : (Length.ReadData ? Length : StreamTagLength.Value);
        /// <summary>
        /// Best-guess FPS for this media. Negative when no valid FPS was found.
        /// </summary>
        public double EstimatedFrameRate =>
            AverageFrameRate.ReadData && !AverageFrameRate.Value.IsInvalid
            ? (double)AverageFrameRate.Value
            : (!BaseFrameRate.Value.IsInvalid ? (double)BaseFrameRate.Value : -1);

        public ParsedStreamData<double> Length = new(key: "duration");
        public ParsedStreamData<Fraction> AverageFrameRate = new(key: "avg_frame_rate");
        public ParsedStreamData<Fraction> BaseFrameRate = new(key: "r_frame_rate");
        public ParsedStreamData<int> Width = new(key: "width");
        public ParsedStreamData<int> Height = new(key: "height");
        public StringStreamData Codec = new(key: "codec_name");
        public StringStreamData Format = new(key: "format_name");
        public StringStreamData PixelFormat = new(key: "pix_fmt");
        public StringStreamData ColorRange = new(key: "color_range");
        public StringStreamData ColorSpace = new(key: "color_space");
        public StringStreamData ColorPrimaries = new(key: "color_primaries");
        public StringStreamData ColorTransfer = new(key: "color_transfer");
        public StringStreamData ChromaLocation = new(key: "chroma_location");
        public ParsedStreamData<FormattedDuration> StreamTagLength = new(key: "DURATION"); //Duration is sometimes stored in stream_tags instead of stream
        public StreamSideData StreamSideData = new(key: "stream_side_data");
        public StreamSideData FrameSideData = new(key: "frame_side_data");

        private ColorKey supportedTransfer = ColorKey.Unsupported;
        public ColorKey SupportedTransfer
        {
            get => supportedTransfer;
            private set => supportedTransfer = value;
        }

        private double peakNits = 10000;
        public double PeakNits
        {
            get => peakNits;
            private set => peakNits = value;
        }
        private long nominalWhiteNits = 100;
        public long NominalWhiteNits
        {
            get => nominalWhiteNits;
            private set => nominalWhiteNits = value;
        }
        public double ToneMapPeak => Math.Max(1, PeakNits / NominalWhiteNits);

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
                (GetAllFramesData(), "frames"),
                (GetAllStreamData(), "stream"),
                (GetAllStreamTagData(), "stream_tags"),
                (GetAllFormatData(), "format")
            };
            for (int i = 0; i < sources.Length; i++)
            {
                args += $":{sources[i].source}";
                bool hadAnyStreams = false;
                foreach (BaseStreamData data in sources[i].streams)
                {
                    if (!hadAnyStreams)
                    {
                        args += "=";
                        hadAnyStreams = true;
                    }
                    else args += ",";

                    args += $"{data.key}";
                }
            }
            foreach (BaseStreamData fullEntries in GetFullEntriesData())
            {
                args += $":{fullEntries.key}";
            }
            return args[1..];
        }
        public IEnumerable<BaseStreamData> GetAllFramesData()
        {
            yield return PixelFormat;
            yield return ColorRange;
            yield return ColorSpace;
            yield return ColorPrimaries;
            yield return ColorTransfer;
            yield return ChromaLocation;
        }
        public IEnumerable<BaseStreamData> GetAllStreamData()
        {
            yield return Length;
            yield return AverageFrameRate;
            yield return BaseFrameRate;
            yield return Width;
            yield return Height;
            yield return Codec;
            yield return PixelFormat;
            yield return ColorRange;
            yield return ColorSpace;
            yield return ColorPrimaries;
            yield return ColorTransfer;
            yield return ChromaLocation;
        }
        public IEnumerable<BaseStreamData> GetAllStreamTagData()
        {
            yield return StreamTagLength;
        }
        public IEnumerable<BaseStreamData> GetAllFormatData()
        {
            yield return Format;
            yield return Length;
        }
        /// <summary>
        /// This is for the data that are actually wrappers for entire entries, instead of individual properties within an entry.
        /// </summary>
        public IEnumerable<BaseStreamData> GetFullEntriesData()
        {
            yield return StreamSideData;
            yield return FrameSideData;
        }
        public IEnumerable<BaseStreamData> GetAllData()
        {
            foreach (var data in GetAllFramesData()) yield return data;
            foreach (var data in GetAllStreamData()) yield return data;
            foreach (var data in GetAllStreamTagData()) yield return data;
            foreach (var data in GetAllFormatData()) yield return data;
            foreach (var data in GetFullEntriesData()) yield return data;
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
                    data.TryReadIfEmpty(stream0);
                }

                if (stream0.TryGetProperty("tags", out var stream0tags))
                {
                    foreach (BaseStreamData data in GetAllStreamTagData())
                    {
                        data.TryReadIfEmpty(stream0tags);
                    }
                }
            }

            if (root.TryGetProperty("format", out var format))
            {
                foreach (BaseStreamData data in GetAllFormatData())
                {
                    data.TryReadIfEmpty(format);
                }
            }

            foreach (BaseStreamData fullEntry in GetFullEntriesData())
            {
                fullEntry.TryReadIfEmpty(root);
            }

            if (ColorTransfer.ReadData)
            {
                SupportedTransfer = ColorKeyUtils.FromFFmpegName(ColorTransfer.Value);
            }
            else
            {
                SupportedTransfer = ColorKey.bt709; //Force a bt.709 fallback
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
                lengthStatus = $"{BestLength} seconds";
                if (AverageFrameRate.ReadData || BaseFrameRate.ReadData)
                {
                    string fpsSteadiness = "steadiness unknown";
                    if (AverageFrameRate.ReadData && BaseFrameRate.ReadData)
                    {
                        string steadiness = Fraction.IsFrameRateSteady(AverageFrameRate.Value, BaseFrameRate.Value) ? "steady" : "unsteady";
                        if (!AverageFrameRate.Value.IsInvalid && !BaseFrameRate.Value.IsInvalid)
                        {
                            fpsSteadiness = Fraction.IsFrameRateSteady(AverageFrameRate.Value, BaseFrameRate.Value) ? "steady" : "unsteady";
                        }
                    }
                    fpsStatus = $"{EstimatedFrameRate} frame rate ({fpsSteadiness})";
                }
            }
            string dynamicRange = SupportedTransfer.TransferIsHDR() ? "HDR" : "SDR";
            return $"({lengthStatus}, {fpsStatus}, {Width.Value}x{Height.Value} resolution, {dynamicRange}, {Codec.Value} encoding in {Format.Value} format. Color space: {ColorSpace.Value}, Color primaries: {ColorPrimaries.Value}, Color transfer: {ColorTransfer.Value}, Color range: {ColorRange.Value}.)";
        }
        public override string ToString() => ToString(null, CultureInfo.InvariantCulture);
    }
}
