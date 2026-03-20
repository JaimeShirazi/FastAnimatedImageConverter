namespace FAIC.Types
{
    public enum ConsoleMessageType
    {
        System,
        Warning,
        Error,
        Tip,
        Progress,
        Success
    }
    public static class ConsoleMessageTypeUtils
    {
        public static void AppendWithFormatting(this IConsole console, ConsoleMessageType type, string message)
        {
            string finalMessage = "[";
            finalMessage += type switch
            {
                ConsoleMessageType.System => "System",
                ConsoleMessageType.Warning => "Warning",
                ConsoleMessageType.Error => "Error",
                ConsoleMessageType.Tip => "Tip",
                ConsoleMessageType.Progress or ConsoleMessageType.Success => "Progress",
                _ => "Message"
            };
            finalMessage += "] " + message;
            console.AppendLog(finalMessage, color: type switch
            {
                ConsoleMessageType.Warning => Color.Goldenrod,
                ConsoleMessageType.Tip or ConsoleMessageType.Progress => Color.RoyalBlue,
                ConsoleMessageType.Error => Color.DarkRed,
                ConsoleMessageType.Success => Color.Green,
                _ => null
            }, true);
        }
    }
}
