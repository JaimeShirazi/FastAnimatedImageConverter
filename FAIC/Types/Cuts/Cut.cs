using System;
using System.Drawing;

namespace FAIC.Types.Cuts
{
    public struct Cut : IEquatable<Cut>
    {
        private decimal _start;
        private decimal _end;

        public decimal Start
        {
            readonly get => _start;
            set => _start = Math.Clamp(value, 0m, 1m);
        }

        public decimal End
        {
            readonly get => _end;
            set => _end = Math.Clamp(value, 0m, 1m);
        }

        public System.Windows.Rect NormalizedCrop { get; set; }

        public Cut(decimal start, decimal end)
        {
            _start = Math.Clamp(start, 0m, 1m);
            _end = Math.Clamp(end, 0m, 1m);
            NormalizedCrop = new System.Windows.Rect(0, 0, 1, 1);
        }

        public readonly bool Overlaps(decimal normalizedPosition) =>
            normalizedPosition >= Start &&
            (normalizedPosition < End ||
             (normalizedPosition == 1m && End == 1m));

        public readonly bool IsValid()
        {
            if (Start is < 0m or > 1m || End is < 0m or > 1m || Start >= End)
            {
                return false;
            }

            System.Windows.Rect crop = NormalizedCrop;
            return double.IsFinite(crop.X) &&
                   double.IsFinite(crop.Y) &&
                   double.IsFinite(crop.Width) &&
                   double.IsFinite(crop.Height) &&
                   crop.Width > 0f &&
                   crop.Height > 0f;
        }

        public readonly bool Equals(Cut other) =>
            Start == other.Start &&
            End == other.End &&
            NormalizedCrop.Equals(other.NormalizedCrop);

        public override readonly bool Equals(object? obj) =>
            obj is Cut other && Equals(other);

        public override readonly int GetHashCode() =>
            HashCode.Combine(Start, End, NormalizedCrop);
    }
}
