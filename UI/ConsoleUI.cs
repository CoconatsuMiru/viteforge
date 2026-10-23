namespace ReactGen.UI;

// Sends status messages from the services to whoever is listening
// (the progress window sets OutputHandler while the generator runs).
public static class ConsoleUI
{
    public static Action<string, ConsoleColor>? OutputHandler { get; set; }

    public static void Title(string text)
    {
        Write("", ConsoleColor.Gray);
        Write(text, ConsoleColor.Gray);
        Write(new string('=', text.Length), ConsoleColor.Gray);
        Write("", ConsoleColor.Gray);
    }

    public static void Info(string message) => Write(message, ConsoleColor.Gray);
    public static void Step(string message) => Write($"> {message}", ConsoleColor.Cyan);
    public static void Success(string message) => Write($"[OK] {message}", ConsoleColor.Green);
    public static void Warning(string message) => Write($"[!] {message}", ConsoleColor.Yellow);
    public static void Error(string message) => Write($"[X] {message}", ConsoleColor.Red);

    private static void Write(string message, ConsoleColor color) =>
        OutputHandler?.Invoke(message, color);
}