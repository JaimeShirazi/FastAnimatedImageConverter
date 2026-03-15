using FAIC.Types;

namespace FAIC
{
    public partial class CustomArguments : Form
    {
        private Action<ArgumentsWindowOutputs> onConfirm;
        public CustomArguments(ArgumentsWindowInputs inputs)
        {
            InitializeComponent();
            onConfirm = inputs.onConfirm;
            arguments.Text = inputs.arguments;
            title.Text = $"Edit the {inputs.programName} arguments in the text box below.";
        }
        private void confirm_Click(object sender, EventArgs e)
        {
            onConfirm.Invoke(new()
            {
                arguments = arguments.Text,
                confirmed = true
            });
            Close();
        }
    }
}
