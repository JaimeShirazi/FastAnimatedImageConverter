namespace FAIC.Types
{
    public struct ArgumentsWindowInputs
    {
        public string programName;
        public string arguments;
        public Action<ArgumentsWindowOutputs> onConfirm;
    }
}
