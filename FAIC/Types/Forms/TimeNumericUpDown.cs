using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FAIC.Types.Forms
{
    [ToolboxItem(true)]
    [Serializable]
    public class TimeNumericUpDown : NumericUpDown
    {
        public interface IMode
        {
            void Setup(TimeNumericUpDown self);
            string Format(decimal value, string preformatted);
            decimal Format(string value);
        }
        public readonly struct SecondsMode : IMode
        {
            public decimal MaxSeconds { get; }
            public SecondsMode(decimal maxSeconds)
            {
                MaxSeconds = maxSeconds;
            }
            public void Setup(TimeNumericUpDown self)
            {
                self.DecimalPlaces = 3;
                self.Maximum = MaxSeconds;
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
        }
        public readonly struct FramesMode : IMode
        {
            public int MaxFrames { get; }
            public FramesMode(int maxFrames)
            {
                MaxFrames = maxFrames;
            }
            public void Setup(TimeNumericUpDown self)
            {
                self.DecimalPlaces = 0;
                self.Maximum = MaxFrames;
            }
            public string Format(decimal value, string preformatted) => $"{preformatted} frame" + (value == 1 ? "" : "s");
            public decimal Format(string value) => Math.Round(
                    SecondsMode.ToDecimal(value),
                    MidpointRounding.AwayFromZero);
        }
        private IMode? current;
        public TimeNumericUpDown()
        {
            Current = new SecondsMode(0);
        }
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IMode Current
        {
            get => current ?? new SecondsMode(0);
            set
            {
                current = value ?? throw new ArgumentNullException(nameof(value));
                current.Setup(this);

                if (Value > Maximum)
                {
                    Value = Maximum;
                }

                if (Value < Minimum)
                {
                    Value = Minimum;
                }

                UpdateEditText();
            }
        }
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