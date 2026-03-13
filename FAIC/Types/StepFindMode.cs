namespace FAIC.Types
{
    [Flags]
    public enum StepFindMode
    {
        Previous = 1,
        Equal = 2,
        Next = 4,

        PreviousOrNow = Previous | Equal,
        NextOrNow = Equal | Next
    }
    public static class StepFindModeUtils
    {
        public static bool Previous(this StepFindMode mode) => mode.HasFlag(StepFindMode.Previous);
        public static bool Now(this StepFindMode mode) => mode.HasFlag(StepFindMode.Equal);
        public static bool Next(this StepFindMode mode) => mode.HasFlag(StepFindMode.Next);
    }
}
