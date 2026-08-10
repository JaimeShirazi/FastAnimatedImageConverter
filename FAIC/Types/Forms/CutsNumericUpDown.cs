using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FAIC.Types.Forms
{
    /// <summary>
    /// Control for displaying the actively selected cut
    /// </summary>
    [ToolboxItem(true)]
    [Serializable]
    public class CutsNumericUpDown : NumericUpDown
    {
        public void ForceUpdateEditText() => UpdateEditText();
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

            ChangingText = true;
            Text = $"Cut {Text} of {(int)Math.Round(Maximum)}";
        }
        protected override void ValidateEditText()
        {
            UserEdit = false;

            MatchCollection matches = Regex.Matches(Text, @"\d+(?:\.\d+)?"); //Get all independent seperated numbers
            if (matches.Count > 0)
            {
                Value = (int)decimal.Parse(matches[0].Value);
                if (matches.Count > 1)
                {
                    int overridenTotalCuts = (int)decimal.Parse(matches[1].Value);
                    if (overridenTotalCuts != Maximum)
                    {
                        MessageBox.Show($"Attempting to overrride the total cuts from {Maximum} to {overridenTotalCuts}. To add or remove cuts, use the add and remove buttons in the cuts section.", "Total Cuts Override Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }

            UpdateEditText();
        }
    }
}