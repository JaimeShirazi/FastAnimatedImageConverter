using System.ComponentModel;
using System.Globalization;

namespace FAIC.Types.Forms
{
    [ToolboxItem(true)]
    [DesignerCategory("Code")]
    [Serializable]
    public class MultiplierNumericUpDown : NumericUpDown
    {
        static bool InDesigner => LicenseManager.UsageMode == LicenseUsageMode.Designtime;
        protected override void OnTextBoxKeyPress(object source, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar))
            {
                base.OnTextBoxKeyPress(source, e);
                return;
            }

            //Allow digits normally
            if (char.IsDigit(e.KeyChar))
            {
                base.OnTextBoxKeyPress(source, e);
                return;
            }

            //Allow these characters to pass through
            if ("xX*.,/".Contains(e.KeyChar))
            {
                e.Handled = false;
                return;
            }

            //Allow everything through
            e.Handled = true;
        }
        protected override void OnLostFocus(EventArgs e)
        {
            ValidateEditText(); //Force the text to be validated even though the box wasn't validated
            base.OnLostFocus(e);
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (InDesigner) return;

            ChangingText = true;
            UpdateEditText();
        }
        protected override void ValidateEditText()
        {
            string text = Text.Trim();

            ChangingText = true;

            if (string.IsNullOrEmpty(text))
            {
                Value = 1; return;
            }

            text = text.Replace(',', '.');
            text = text.Replace("x", "").Replace("X", "").Replace("*", "");

            if (Fraction.TryParse(text, CultureInfo.InvariantCulture, out Fraction result))
            {
                Value = (decimal)Math.Round((double)result, 3);
            }

            base.ValidateEditText();
        }
        protected override void UpdateEditText()
        {
            if (!InDesigner && Value != 0)
            {
                ChangingText = true;
                UserEdit = true;
                Text = $"{Value}x";
                return;
            }

            base.UpdateEditText();
        }
        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            Invalidate();
        }
    }
}
