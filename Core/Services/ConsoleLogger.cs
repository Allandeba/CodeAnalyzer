using CodeAnalyzer.Core.Interfaces;

namespace CodeAnalyzer.Core.Services;

public class ConsoleLogger : ILogger
{
    public void Log(LogLevel level, string message) =>
        Console.WriteLine($"[{level}] {message}");
}