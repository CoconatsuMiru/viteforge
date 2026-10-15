namespace ReactGen.UI;

public static class ConsoleUI
{
    public static void Title(string text)
    {
        Console.WriteLine();
        Console.WriteLine(text);
        Console.WriteLine(new string('=', text.Length));
        Console.WriteLine();
    }

    public static void Info(string message) => Write(message, ConsoleColor.Gray);
    public static void Step(string message) => Write($"> {message}", ConsoleColor.Cyan);
    public static void Success(string message) => Write($"[OK] {message}", ConsoleColor.Green);
    public static void Warning(string message) => Write($"[!] {message}", ConsoleColor.Yellow);
    public static void Error(string message) => Write($"[X] {message}", ConsoleColor.Red);

    private static void Write(string message, ConsoleColor color)
    {
        var previous = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = previous;
    }

    // validate returns an error message, or null if the input is valid.
    public static string AskText(
        string prompt,
        string? defaultValue = null,
        Func<string, string?>? validate = null)
    {
        while (true)
        {
            Console.Write(defaultValue is null ? $"{prompt}: " : $"{prompt} [{defaultValue}]: ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input) && defaultValue is not null)
            {
                input = defaultValue;
            }

            input ??= "";

            var error = validate?.Invoke(input);
            if (error is null)
            {
                return input;
            }

            Error(error);
        }
    }

    // Returns the zero-based index of the chosen option.
    public static int AskChoice(string prompt, IReadOnlyList<string> options, int defaultIndex = 0)
    {
        Console.WriteLine(prompt);
        for (var i = 0; i < options.Count; i++)
        {
            Console.WriteLine($"  {i + 1}) {options[i]}");
        }

        while (true)
        {
            Console.Write($"Choose [1-{options.Count}] (default {defaultIndex + 1}): ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrEmpty(input))
            {
                return defaultIndex;
            }

            if (int.TryParse(input, out var number) && number >= 1 && number <= options.Count)
            {
                return number - 1;
            }

            Error("Invalid choice.");
        }
    }

    public static bool AskYesNo(string prompt, bool defaultValue = false)
    {
        var hint = defaultValue ? "Y/n" : "y/N";

        while (true)
        {
            Console.Write($"{prompt} [{hint}]: ");
            var input = Console.ReadLine()?.Trim().ToLowerInvariant();

            if (string.IsNullOrEmpty(input))
            {
                return defaultValue;
            }

            if (input is "y" or "yes")
            {
                return true;
            }

            if (input is "n" or "no")
            {
                return false;
            }

            Error("Please answer y or n.");
        }
    }
}