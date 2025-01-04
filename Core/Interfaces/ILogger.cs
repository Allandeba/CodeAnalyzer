namespace CodeAnalyzer.Core.Interfaces;

public interface ILogger
{
    void Log(LogLevel level, string message);
}

public enum LogLevel
{
    Debug,
    Information,
    Warning,
    Error
}
