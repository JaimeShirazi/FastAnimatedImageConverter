using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FAIC.Types
{
    public readonly record struct FormattedDuration(double Seconds) : ISpanParsable<FormattedDuration>, IFormattable
    {
        #region ISpanParsable
        public static FormattedDuration Parse(string s, IFormatProvider provider) =>
            TryParse(s, provider, out var r)
                ? r
                : throw new FormatException($"Invalid rational: '{s}'");
        public static bool TryParse([NotNullWhen(true)] string s, IFormatProvider provider, out FormattedDuration result)
        {
            if (s is null)
            {
                result = default;
                return false;
            }

            return TryParse(s.AsSpan(), provider, out result);
        }
        public static FormattedDuration Parse(ReadOnlySpan<char> s, IFormatProvider provider) =>
            TryParse(s, provider, out var r)
                ? r
                : throw new FormatException($"Invalid rational: '{s.ToString()}'");
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out FormattedDuration result)
        {
            result = default;

            int first = s.IndexOf(':');
            if (first < 0) return false;

            int second = s[(first + 1)..].IndexOf(':');
            if (second < 0) return false;
            second += first + 1;

            if (!double.TryParse(s[..first],
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var hours))
                return false;

            if (!double.TryParse(s[first..second],
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out var minutes))
                return false;

            if (!double.TryParse(s[second..],
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out var seconds))
                return false;

            result = new(hours * 3600 +
                   minutes * 60 +
                   seconds);

            return true;
        }
        #endregion

        #region IFormattable
        public string ToString(string format, IFormatProvider formatProvider)
        {
            double s = Seconds;

            long hours = (long)(s / 3600);
            s -= hours * 3600;

            int minutes = (int)(s / 60);
            s -= minutes * 60;

            int seconds = (int)s;
            double fraction = s - seconds;

            long fracTicks = (long)(fraction * 10_000_000); // 7 digits

            return $"{hours}:{minutes:D2}:{seconds:D2}.{fracTicks:D7}";
        }
        public override string ToString() =>
            ToString(null, CultureInfo.InvariantCulture);
        #endregion

        public static implicit operator double(FormattedDuration rational) => rational.Seconds;
    }
}
