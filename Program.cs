using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CodeAnalyzer.Core.Interfaces;
using CodeAnalyzer.Core.Models;
using CodeAnalyzer.Core.Services;

namespace CodeAnalyzer;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        var analyzerConfig = builder.Configuration.GetSection("AnalyzerConfig").Get<AnalyzerConfiguration>();
        builder.Services.AddSingleton(analyzerConfig!);

        builder.Services.AddSingleton<ILogger, ConsoleLogger>();
        builder.Services.AddSingleton<IFileSystemAccessor, FileSystemAccessor>();
        builder.Services.AddTransient<CodeAnalyzerService>();

        using var host = builder.Build();

        var analyzer = host.Services.GetRequiredService<CodeAnalyzerService>();

        Console.WriteLine("Enter the directory path to analyze:");
        var directoryPath = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            Console.WriteLine("Invalid directory path.");
            return;
        }

        analyzer.AnalyzeDirectory(directoryPath);

        Console.WriteLine("Analysis complete. Press any key to exit.");
        Console.ReadLine();
    }
}