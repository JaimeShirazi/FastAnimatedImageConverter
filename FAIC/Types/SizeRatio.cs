using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Windows.Forms;

namespace FAIC.Types
{
    [ToolboxItem(true)]
    [Serializable]
    public class SizeRatio : NumericUpDown
    {
        private const decimal MinimumRatio = 0m;

        //Uses default ValidateEditText/UpdateEditText before constructor runs
        private bool isConstructed;
        private int sourceWidth = 1;
        private int sourceHeight = 1;
        private decimal cropWidthRatio = 1m;
        private decimal cropHeightRatio = 1m;

        public SizeRatio()
        {
            base.Minimum = 0;
            base.Maximum = int.MaxValue; //I'm hard clamping the max ratio to at least integer overflow when 1x1 source.
            base.Value = 1;

            TextAlign = HorizontalAlignment.Left;
            isConstructed = true;
            UpdateEditText();
        }

        public event Action? OnSizeVisualUpdate;

        public event Action? OnSizeValueUpdate;

        [DefaultValue(1)]
        public int SourceWidth
        {
            get => sourceWidth;
            set
            {
                value = Math.Max(value, 0);
                if (sourceWidth == value)
                    return;

                sourceWidth = value;
                RefreshSizeVisual();
            }
        }

        [DefaultValue(1)]
        public int SourceHeight
        {
            get => sourceHeight;
            set
            {
                value = Math.Max(value, 0);
                if (sourceHeight == value)
                    return;

                sourceHeight = value;
                RefreshSizeVisual();
            }
        }

        [DefaultValue(typeof(decimal), "1")]
        public decimal CropWidthRatio
        {
            get => cropWidthRatio;
            set
            {
                value = ClampRatio(value);
                if (cropWidthRatio == value)
                    return;

                cropWidthRatio = value;
                RefreshSizeVisual();
            }
        }

        [DefaultValue(typeof(decimal), "1")]
        public decimal CropHeightRatio
        {
            get => cropHeightRatio;
            set
            {
                value = ClampRatio(value);
                if (cropHeightRatio == value)
                    return;

                cropHeightRatio = value;
                RefreshSizeVisual();
            }
        }

        [DefaultValue(typeof(decimal), "1")]
        public decimal Ratio
        {
            get => base.Value;
            set
            {
                value = ClampRatio(value);

                // A programmatic assignment behaves like NumericUpDown.Value: it
                // replaces any uncommitted text and immediately refreshes the edit box.
                UserEdit = false;

                if (base.Value == value)
                {
                    UpdateEditText();
                    return;
                }

                base.Value = value;
            }
        }

        /// <summary>
        /// Hides NumericUpDown.Value from the designer. The underlying value is still
        /// the ratio; Ratio is the preferred public property.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new decimal Value
        {
            get => Ratio;
            set => Ratio = value;
        }

        /// <summary>
        /// The source width after cropping but before Ratio is applied.
        /// This can be fractional when the crop is supplied as a ratio.
        /// </summary>
        [Browsable(false)]
        public decimal CroppedWidth => SourceWidth * CropWidthRatio;

        /// <summary>
        /// The source height after cropping but before Ratio is applied.
        /// This can be fractional when the crop is supplied as a ratio.
        /// </summary>
        [Browsable(false)]
        public decimal CroppedHeight => SourceHeight * CropHeightRatio;

        [Browsable(false)]
        public int EffectiveWidth
        {
            get => Math.Max(RoundToPixel(CroppedWidth * Ratio), 1);
            set => Ratio = RatioForAxis(Math.Max(value, 0), CroppedWidth) ?? Ratio;
        }

        [Browsable(false)]
        public int EffectiveHeight
        {
            get => Math.Max(RoundToPixel(CroppedHeight * Ratio), 1);
            set => Ratio = RatioForAxis(Math.Max(value, 0), CroppedHeight) ?? Ratio;
        }

        /// <summary>
        /// Ratio delta which changes the longer post-crop axis by one unrounded pixel.
        /// If both axes are equal, both change by one pixel.
        /// </summary>
        [Browsable(false)]
        public decimal PixelRatioIncrement
        {
            get
            {
                decimal longestAxis = Math.Max(CroppedWidth, CroppedHeight);
                return longestAxis > 0m ? 1m / longestAxis : 0m;
            }
        }

        // These properties are fixed because changing them would break the invariant
        // that NumericUpDown.Value is the 0..1 ratio.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new decimal Minimum
        {
            get => MinimumRatio;
            set
            {
                if (value != MinimumRatio)
                    throw new NotSupportedException("SizeRatio.Minimum is fixed at 0.");
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new decimal Increment
        {
            get => PixelRatioIncrement;
            set => throw new NotSupportedException(
                "SizeRatio calculates its increment from the post-crop resolution.");
        }

        public override void UpButton()
        {
            CommitPendingText();

            decimal increment = PixelRatioIncrement;
            if (increment > 0m)
                Ratio = ClampRatio(Ratio + increment);
        }

        public override void DownButton()
        {
            CommitPendingText();

            decimal increment = PixelRatioIncrement;
            if (increment > 0m)
                Ratio = ClampRatio(Ratio - increment);
        }

        /// <summary>
        /// Accepts a width ("1280"), a resolution ("1280 x 720" or
        /// "1280 × 720"), or a percentage ("50%").
        /// </summary>
        public bool TrySetFromText(string text)
        {
            if (!TryGetRatioFromText(text, out decimal parsedRatio))
                return false;

            Ratio = parsedRatio;
            return true;
        }

        protected override void ValidateEditText()
        {
            if (!isConstructed)
            {
                base.ValidateEditText();
                return;
            }

            string pendingText = Text;

            //NumericUpDown.Value calls ValidateEditText whenever UserEdit is true.
            //Clear it before parsing because parsing reads Ratio/base.Value
            UserEdit = false;
            bool parsed = TryGetRatioFromText(pendingText, out decimal parsedRatio);
            if (parsed)
                Ratio = parsedRatio;

            //Invalid inputs retains current value
            UpdateEditText();
        }

        protected override void UpdateEditText()
        {
            if (!isConstructed)
            {
                base.UpdateEditText();
                return;
            }

            //Preserve partially entered text until validation
            if (UserEdit) return;

            ChangingText = true;
            Text = string.Format(
                CultureInfo.CurrentCulture,
                "{0}×{1}",
                EffectiveWidth,
                EffectiveHeight);
        }

        protected override void OnTextBoxKeyPress(object? source, KeyPressEventArgs e)
        {
            // NumericUpDown normally rejects x, ×, %, and spaces. Raise the public
            // KeyPress event, then apply a filter suited to this control's grammar.
            OnKeyPress(e);
            if (e.Handled || (ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
                return;

            NumberFormatInfo numberFormat = CultureInfo.CurrentCulture.NumberFormat;
            string key = e.KeyChar.ToString();

            bool allowed =
                char.IsDigit(e.KeyChar) ||
                e.KeyChar == ' ' ||
                e.KeyChar == '\b' ||
                e.KeyChar == 'x' ||
                e.KeyChar == 'X' ||
                e.KeyChar == '*' ||
                e.KeyChar == '\u00D7' ||
                e.KeyChar == '%' ||
                e.KeyChar == '.' ||
                key == numberFormat.NumberDecimalSeparator ||
                key == numberFormat.NumberGroupSeparator ||
                key == numberFormat.PositiveSign;

            e.Handled = !allowed;
        }

        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            OnSizeVisualUpdate?.Invoke();
            OnSizeValueUpdate?.Invoke();
        }

        private void RefreshSizeVisual()
        {
            UserEdit = false;
            UpdateEditText();
            OnSizeVisualUpdate?.Invoke();
        }

        private void CommitPendingText()
        {
            if (UserEdit)
                ValidateEditText();
        }

        private bool TryGetRatioFromText(string text, out decimal parsedRatio)
        {
            parsedRatio = Ratio;

            if (string.IsNullOrWhiteSpace(text))
                return false;

            string input = text.Trim();

            //Try do percentage
            if (input.EndsWith("%", StringComparison.Ordinal))
            {
                string percentageText = input[..^1].Trim();
                if (!TryParseNumber(percentageText, out decimal percentage) || percentage < 0m)
                    return false;

                parsedRatio = ClampRatio(percentage / 100m);
                return true;
            }

            string[] dimensions = input.Split(
                new[] { 'x', 'X', '\u00D7', '*' },
                StringSplitOptions.None);

            //Derive height from width
            if (dimensions.Length == 1)
            {
                if (!TryParseNumber(dimensions[0], out decimal requestedWidth) ||
                    requestedWidth < 0m)
                {
                    return false;
                }

                if (requestedWidth == EffectiveWidth)
                    return true;

                if (CroppedWidth <= 0m)
                    return false;

                parsedRatio = RatioForAxis(requestedWidth, CroppedWidth) ?? Ratio;
                return true;
            }

            //Get the 2 numbers and return false if fails
            if (dimensions.Length != 2 ||
                !TryParseNumber(dimensions[0], out decimal requestedWidthPair) ||
                !TryParseNumber(dimensions[1], out decimal requestedHeightPair) ||
                requestedWidthPair < 0m ||
                requestedHeightPair < 0m)
            {
                return false;
            }

            bool widthChanged = requestedWidthPair != EffectiveWidth;
            bool heightChanged = requestedHeightPair != EffectiveHeight;

            if (!widthChanged && !heightChanged)
                return true;

            if (widthChanged && !heightChanged)
            {
                if (CroppedWidth <= 0m)
                    return false;

                parsedRatio = RatioForAxis(requestedWidthPair, CroppedWidth) ?? Ratio;
                return true;
            }

            if (!widthChanged && heightChanged)
            {
                if (CroppedHeight <= 0m)
                    return false;

                parsedRatio = RatioForAxis(requestedHeightPair, CroppedHeight) ?? Ratio;
                return true;
            }

            return TryGetCompromiseRatio(
                requestedWidthPair,
                requestedHeightPair,
                out parsedRatio);
        }

        private bool TryGetCompromiseRatio(
            decimal requestedWidth,
            decimal requestedHeight,
            out decimal parsedRatio)
        {
            decimal width = CroppedWidth;
            decimal height = CroppedHeight;
            parsedRatio = Ratio;

            if (width <= 0m && height <= 0m)
                return false;

            if (width <= 0m)
            {
                parsedRatio = RatioForAxis(requestedHeight, height) ?? Ratio;
                return true;
            }

            if (height <= 0m)
            {
                parsedRatio = RatioForAxis(requestedWidth, width) ?? Ratio;
                return true;
            }

            // Orthogonally project the requested W/H point onto the one-ratio size
            // line. This minimizes total squared pixel error across both axes.
            try
            {
                decimal numerator = width * requestedWidth + height * requestedHeight;
                decimal denominator = width * width + height * height;
                parsedRatio = ClampRatio(numerator / denominator);
            }
            catch
            {
                return false;
            }

            return true;
        }

        private static decimal? RatioForAxis(
            decimal requestedPixels,
            decimal fullSizePixels)
        {
            if (fullSizePixels <= 0m) return null;
            if (requestedPixels <= 0m) return null;

            return requestedPixels / fullSizePixels;
        }

        private static bool TryParseNumber(string text, out decimal value)
        {
            text = text.Trim();

            if (decimal.TryParse(
                    text,
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out value))
            {
                return true;
            }

            // Also accept invariant input when it differs from the UI culture. This
            // is convenient for pasted dimensions such as "1920.5".
            return decimal.TryParse(
                text,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out value);
        }

        private static int RoundToPixel(decimal pixels)
        {
            return decimal.ToInt32(decimal.Round(
                pixels,
                0,
                MidpointRounding.AwayFromZero));
        }

        private static decimal ClampRatio(decimal ratio) => Math.Clamp(ratio, 0, int.MaxValue);
    }
}
