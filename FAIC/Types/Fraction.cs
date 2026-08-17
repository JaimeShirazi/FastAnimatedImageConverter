using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace FAIC.Types
{
    public readonly record struct Fraction(long Num, long Den) : ISpanParsable<Fraction>, IFormattable
    {
        public bool IsInvalid => Num <= 0 && Den <= 0;

        #region ISpanParsable
        public static Fraction Parse(string s, IFormatProvider provider) =>
            TryParse(s, provider, out var r)
                ? r
                : throw new FormatException($"Invalid rational: '{s}'");
        public static bool TryParse([NotNullWhen(true)] string s, IFormatProvider provider, out Fraction result)
        {
            if (s is null)
            {
                result = default;
                return false;
            }

            return TryParse(s.AsSpan(), provider, out result);
        }
        public static Fraction Parse(ReadOnlySpan<char> s, IFormatProvider provider) =>
            TryParse(s, provider, out var r)
                ? r
                : throw new FormatException($"Invalid rational: '{s.ToString()}'");
        public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider provider, out Fraction result)
        {
            result = default;
            int split = s.IndexOf('/');
            if (split < 1 || split >= s.Length - 1) return false;

            if (!long.TryParse(s[..split], NumberStyles.Integer, provider, out long num)) return false;
            if (!long.TryParse(s[(split + 1)..], NumberStyles.Integer, provider, out long den)) return false;

            result = new Fraction(num, den);
            return true;
        }
        #endregion

        #region IFormattable
        public string ToString(string format, IFormatProvider formatProvider) =>
            string.Create(formatProvider, $"{Num}/{Den}");
        public override string ToString() =>
            ToString(null, CultureInfo.InvariantCulture);
        #endregion

        public static bool IsFrameRateSteady(Fraction averageFrameRate, Fraction baseFrameRate)
        {
            //Without both, we can't say for sure, so we assume it is.
            if (averageFrameRate.IsInvalid || baseFrameRate.IsInvalid) return true;

            return (Math.Abs((double)averageFrameRate - (double)baseFrameRate)) / (double)averageFrameRate < 0.0025;
        }

        public static explicit operator double(Fraction rational) => rational.Den == 0 ? double.NaN : (double)rational.Num / rational.Den;
    }
}
