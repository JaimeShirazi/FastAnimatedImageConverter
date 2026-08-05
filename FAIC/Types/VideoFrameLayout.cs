using System;
using System.Windows;

namespace FAIC.Types
{
    /// <summary>
    /// Describes the coded frame and the source-space region that is safe to display.
    /// Coordinates are expressed in coded pixels.
    /// </summary>
    /// <remarks>
    /// I used ChatGPT 5.6 Sol to go ham with the equality/safety checks
    /// </remarks>
    public readonly struct VideoFrameLayout
    {
        public static VideoFrameLayout Empty => default;

        public int CodedWidth { get; }
        public int CodedHeight { get; }
        public Rect VisiblePixels { get; }

        public bool IsValid => CodedWidth > 0
            && CodedHeight > 0
            && !VisiblePixels.IsEmpty
            && VisiblePixels.Width > 0
            && VisiblePixels.Height > 0;

        public bool HasPadding => IsValid
            && (Math.Abs(VisiblePixels.X) > 0.0001
                || Math.Abs(VisiblePixels.Y) > 0.0001
                || Math.Abs(VisiblePixels.Width - CodedWidth) > 0.0001
                || Math.Abs(VisiblePixels.Height - CodedHeight) > 0.0001);

        public double DisplayWidth => IsValid ? VisiblePixels.Width : 0;
        public double DisplayHeight => IsValid ? VisiblePixels.Height : 0;

        /// <summary>
        /// Integer version of <see cref="VisiblePixels"/> for bitmap copies.
        /// Fractional Media Foundation offsets are rounded to the nearest pixel.
        /// </summary>
        public Int32Rect IntegralVisiblePixels
        {
            get
            {
                if (!IsValid)
                    return Int32Rect.Empty;

                int x = Math.Clamp((int)Math.Round(VisiblePixels.X), 0, CodedWidth - 1);
                int y = Math.Clamp((int)Math.Round(VisiblePixels.Y), 0, CodedHeight - 1);
                int width = Math.Clamp((int)Math.Round(VisiblePixels.Width), 1, CodedWidth - x);
                int height = Math.Clamp((int)Math.Round(VisiblePixels.Height), 1, CodedHeight - y);
                return new Int32Rect(x, y, width, height);
            }
        }

        public VideoFrameLayout(int codedWidth, int codedHeight, Rect visiblePixels)
        {
            CodedWidth = Math.Max(0, codedWidth);
            CodedHeight = Math.Max(0, codedHeight);

            if (CodedWidth == 0 || CodedHeight == 0 || !IsFinite(visiblePixels))
            {
                VisiblePixels = Rect.Empty;
                return;
            }

            Rect codedBounds = new(0, 0, CodedWidth, CodedHeight);
            Rect clipped = Rect.Intersect(codedBounds, visiblePixels);
            VisiblePixels = clipped.IsEmpty || clipped.Width <= 0 || clipped.Height <= 0
                ? codedBounds
                : clipped;
        }

        public static VideoFrameLayout FullFrame(int width, int height)
        {
            return width > 0 && height > 0
                ? new VideoFrameLayout(width, height, new Rect(0, 0, width, height))
                : Empty;
        }

        /// <summary>
        /// Maps the visible aperture onto the dimensions exposed by WPF's MediaPlayer.
        /// Some decoders expose coded dimensions while others have already removed the
        /// aperture; the latter case is detected to avoid cropping twice.
        /// </summary>
        public Rect GetMediaPlayerVisiblePixels(int naturalWidth, int naturalHeight)
        {
            if (naturalWidth <= 0 || naturalHeight <= 0)
                return Rect.Empty;

            Rect naturalBounds = new(0, 0, naturalWidth, naturalHeight);
            if (!IsValid || !HasPadding)
                return naturalBounds;

            bool playerAlreadyUsesAperture = NearlyEqual(naturalWidth, VisiblePixels.Width)
                && NearlyEqual(naturalHeight, VisiblePixels.Height)
                && (naturalWidth != CodedWidth || naturalHeight != CodedHeight);

            if (playerAlreadyUsesAperture)
                return naturalBounds;

            double scaleX = naturalWidth / (double)CodedWidth;
            double scaleY = naturalHeight / (double)CodedHeight;
            return new Rect(
                VisiblePixels.X * scaleX,
                VisiblePixels.Y * scaleY,
                VisiblePixels.Width * scaleX,
                VisiblePixels.Height * scaleY);
        }

        private static bool NearlyEqual(double left, double right)
        {
            return Math.Abs(left - right) <= 1.0;
        }

        private static bool IsFinite(Rect rect)
        {
            return !double.IsNaN(rect.X)
                && !double.IsNaN(rect.Y)
                && !double.IsNaN(rect.Width)
                && !double.IsNaN(rect.Height)
                && !double.IsInfinity(rect.X)
                && !double.IsInfinity(rect.Y)
                && !double.IsInfinity(rect.Width)
                && !double.IsInfinity(rect.Height);
        }
    }
}
