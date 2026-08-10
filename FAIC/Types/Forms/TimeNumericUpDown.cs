using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Documents;

namespace FAIC.Types.Forms
{
    /// <summary>
    /// Control for displaying a time value, the unit defined by the <see cref="IMode"/>.
    /// </summary>
    [ToolboxItem(true)]
    [Serializable]
    public class TimeNumericUpDown : NumericUpDown
    {
        public interface IMode
        {
            void Setup(TimeNumericUpDown self, bool isStart);
            /// <param name="otherValue">When <paramref name="isStart"/> is true, the end value, when false, the start value.</param>
            void SetNew(TimeNumericUpDown self, decimal value, bool isStart, decimal otherValue);
            string Format(decimal value, string preformatted);
            decimal Format(string value);
            decimal GetMinimumCut();
            public decimal GetNormalizedValue(decimal rawValue);
            /// <param name="otherValue">When <paramref name="isStart"/> is true, the end value, when false, the start value.</param>
            protected static void GetRange(bool isStart, decimal minCut, decimal length, decimal otherValue, out decimal min, out decimal max)
            {
                min = isStart ? 0 : otherValue + minCut;
                max = isStart ? otherValue - minCut : length;
            }
            protected static void SafeSet(TimeNumericUpDown self, decimal value, decimal min, decimal max, decimal minCut)
            {
                try
                {
                    if (min > max)
                        throw new System.ArgumentOutOfRangeException($"Minimum {min} cannot be greater than maximum {max}!");
                    if (min - value < minCut * 0.5m || value - max < minCut * 0.5m)
                        value = Math.Clamp(value, min, max); //deal with rounding jank that we don't care about
                    if (value < min || value > max)
                        throw new System.ArgumentOutOfRangeException($"Value {value} must be within the new range {min} and {max}!");

                    //In the case the previous range does not include the new target
                    if (value != Math.Clamp(value, self.Minimum, self.Maximum))
                    {
                        //Expand the range to fit everything first
                        self.Minimum = Math.Min(self.Minimum, min);
                        self.Maximum = Math.Max(self.Maximum, max);
                    }
                    self.Value = value;
                    self.Minimum = min;
                    self.Maximum = max;
                }
                catch (Exception ex)
                {
                    //For some reason, setting values of NumericUpDown is extremely temperamental,
                    //so this is common and should be a little more descriptive than the default error message.
                    throw new System.ArgumentOutOfRangeException($"Failed to set ranges to minimum {min} and maximum {max}.", ex);
                }
            }
        }
        public readonly struct SecondsMode : IMode
        {
            const int DECIMAL_PLACES = 3;
            public decimal MaxSeconds { get; }
            public SecondsMode(decimal maxSeconds)
            {
                MaxSeconds = maxSeconds;
            }
            public void Setup(TimeNumericUpDown self, bool isStart)
            {
                self.DecimalPlaces = DECIMAL_PLACES;
                decimal thisValue = isStart ? 0 : MaxSeconds;
                decimal otherValue = isStart ? MaxSeconds : 0;
                SetNew(self, thisValue, isStart, otherValue);
            }
            /// <param name="otherValue">When <paramref name="isStart"/> is true, the end value, when false, the start value.</param>
            public void SetNew(TimeNumericUpDown self, decimal value, bool isStart, decimal otherValue)
            {
                IMode.GetRange(isStart, GetMinimumCut(), MaxSeconds, otherValue, out decimal min, out decimal max);
                IMode.SafeSet(self, value, min, max, GetMinimumCut());
            }
            public string Format(decimal value, string preformatted) => $"{preformatted}s";
            public static decimal ToDecimal(string value)
            {
                string stripped = Regex.Replace(value, @"[^\d.-]", string.Empty);
                return decimal.TryParse(stripped,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out decimal result)
                    ? result : 0m;
            }
            public decimal Format(string value) => ToDecimal(value);
            public decimal GetMinimumCut() => (decimal)Math.Pow(0.1, DECIMAL_PLACES);
            public decimal GetNormalizedValue(decimal rawValue) => rawValue / MaxSeconds;
        }
        public readonly struct FramesMode : IMode
        {
            public int TotalFrames { get; }
            public FramesMode(int totalFrames)
            {
                TotalFrames = totalFrames;
            }
            public void Setup(TimeNumericUpDown self, bool isStart)
            {
                self.DecimalPlaces = 0;
                decimal thisValue = isStart ? 0 : TotalFrames;
                decimal otherValue = isStart ? TotalFrames : 0;
                SetNew(self, thisValue, isStart, otherValue);
            }
            /// <param name="otherValue">When <paramref name="isStart"/> is true, the end value, when false, the start value.</param>
            public void SetNew(TimeNumericUpDown self, decimal value, bool isStart, decimal otherValue)
            {
                IMode.GetRange(isStart, GetMinimumCut(), TotalFrames, otherValue, out decimal min, out decimal max);
                IMode.SafeSet(self, value, min, max, GetMinimumCut());
            }
            public string Format(decimal value, string preformatted) => $"{preformatted} frame" + (value == 1 ? "" : "s");
            public decimal Format(string value) => Math.Round(
                    SecondsMode.ToDecimal(value),
                    MidpointRounding.AwayFromZero);
            public decimal GetMinimumCut() => 1;
            public decimal GetNormalizedValue(decimal rawValue) => rawValue / TotalFrames;
        }
        private IMode? current;
        public bool IsSetup => current != null;
        public void SetMode(IMode value, bool isStart)
        {
            current = value ?? throw new ArgumentNullException(nameof(value));
            current.Setup(this, isStart);
            UpdateEditText();
        }
        /// <param name="otherValue">When <paramref name="isStart"/> is true, the end value, when false, the start value.</param>
        public void SetNew(decimal value, bool isStart, decimal otherValue) => current.SetNew(this, value, isStart, otherValue);
        public decimal? NormalizedValue => current?.GetNormalizedValue(Value);
        protected override void OnTextBoxKeyPress(object source, KeyPressEventArgs e)
        {
            //Allow everything through
            e.Handled = false;
        }
        protected override void OnLostFocus(EventArgs e)
        {
            ValidateEditText(); //Force the text to be validated even though the box wasn't validated
            base.OnLostFocus(e);
        }
        protected override void UpdateEditText()
        {
            base.UpdateEditText();

            if (current == null)
                return;

            ChangingText = true;
            Text = current.Format(Value, Text);
        }
        protected override void ValidateEditText()
        {
            if (current == null)
            {
                base.ValidateEditText();
                return;
            }

            decimal parsedValue = current.Format(Text);
            parsedValue = Math.Clamp(parsedValue, Minimum, Maximum);

            UserEdit = false;
            Value = parsedValue;
            UpdateEditText();
        }
    }
}