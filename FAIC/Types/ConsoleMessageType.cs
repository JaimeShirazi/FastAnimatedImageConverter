namespace FAIC.Types
{
    public enum ConsoleMessageType
    {
        System,
        Warning,
        Error,
        Tip
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
                _ => "Message"
            };
            finalMessage += "] " + message;
            console.AppendLog(finalMessage, color: type switch
            {
                ConsoleMessageType.Warning => Color.Yellow,
                ConsoleMessageType.Tip => Color.Blue,
                ConsoleMessageType.Error => Color.Red,
                _ => null
            });
        }
    }
}
