using System.ComponentModel;

namespace FAIC.Types.Forms
{
    [ToolboxItem(true)]
    [DesignerCategory("Code")]
    [Serializable]
    public class InfinityNumericUpDown : NumericUpDown
    {
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
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (Value < 0)
            {
                ChangingText = true;
                UpdateEditText();
            }
        }
        protected override void ValidateEditText()
        {
            string text = Text.Trim();

            ChangingText = true;

            if (string.IsNullOrEmpty(text))
            {
                Value = Minimum; return;
            }

            if (decimal.TryParse(text, out decimal value))
            {
                if (value < 0 || value > Maximum)
                {
                    Value = Minimum;
                }
                else
                {
                    Value = Math.Clamp(Math.Round(value), 0, Maximum);
                }
            }
            else
            {
                Value = Minimum; //Treat literally anything else as infinity
            }
        }
        protected override void UpdateEditText()
        {
            if (Value < 0)
            {
                ChangingText = true;
                UserEdit = true;
                Text = "∞";
            }
            else
            {
                UserEdit = false;
                base.UpdateEditText();
            }
        }
        protected override void OnValueChanged(EventArgs e)
        {
            base.OnValueChanged(e);
            Invalidate();
        }
    }
}
