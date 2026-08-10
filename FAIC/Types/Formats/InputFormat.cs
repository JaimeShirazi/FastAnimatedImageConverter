using System.Diagnostics.CodeAnalysis;

namespace FAIC.Types.Formats
{
    public enum InputFormat
    {
        Generic = 0,
        Concat
    }
    public static class InputFormatUtils
    {
        public static InputFormat GetTarget(string container)
        {
            switch (container.ToLowerInvariant())
            {
                case "concat":
                    return InputFormat.Concat;
                default:
                    return InputFormat.Generic;
            }
        }
        public static bool IsUnsafe(this InputFormat container) => container switch
        {
            InputFormat.Concat => true,
            _ => false
        };
        public static bool RequiresContainer(this InputFormat format, [NotNullWhen(true)] out string? container)
        {
            container = format switch
            {
                InputFormat.Concat => "concat",
                _ => null
            };
            return (container != null);
        }
    }
}
