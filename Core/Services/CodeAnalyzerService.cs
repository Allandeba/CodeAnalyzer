using CodeAnalyzer.Core.Interfaces;
using CodeAnalyzer.Core.Models;

namespace CodeAnalyzer.Core.Services;

public class CodeAnalyzerService
{
    private readonly ILogger _logger;
    private readonly IFileSystemAccessor _fileSystem;
    private readonly AnalyzerConfiguration _config;

    public CodeAnalyzerService(ILogger logger, IFileSystemAccessor fileSystem, AnalyzerConfiguration config)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fileSystem = fileSystem ?? throw new ArgumentNullException(nameof(fileSystem));
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    public void AnalyzeDirectory(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            _logger.Log(LogLevel.Error, $"Directory not found: {directoryPath}");
            return;
        }

        var files = _fileSystem.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories)
                               .Where(filePath => !ShouldIgnoreFile(filePath));

        foreach (var file in files)
            AnalyzeFile(file);
    }

    private bool ShouldIgnoreFile(string filePath)
        => _config
            .IgnoredFiles.Any(path =>
                Path.GetFileName(filePath)
                    .EndsWith(path, StringComparison.OrdinalIgnoreCase)
            );

    private void AnalyzeFile(string filePath)
    {
        try
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var content = _fileSystem.ReadAllText(filePath);
            var codeElements = new CodeElements(content);

            if (codeElements.IsEmpty)
            {
                _logger.Log(LogLevel.Information, $"File {fileName}.cs does not contain any class, interface, or enum.");
                return;
            }

            AnalyzeClasses(fileName, codeElements.ClassNames);
            AnalyzeInterfaces(fileName, codeElements.InterfaceNames);
            AnalyzeEnums(fileName, codeElements.EnumNames);

            if (codeElements.HasMultipleTypes)
                _logger.Log(LogLevel.Warning, $"File {fileName}.cs contains a combination of classes, interfaces, and/or enums.");
        }
        catch (Exception ex)
        {
            _logger.Log(LogLevel.Error, $"Error analyzing file {filePath}: {ex.Message}");
        }
    }

    private void AnalyzeClasses(string fileName, IReadOnlyList<string> classNames)
    {
        if (classNames.Count == 1 && _config.StrictNaming && classNames[0] != fileName)
        {
            _logger.Log(LogLevel.Warning, $"Discrepancy: File {fileName}.cs contains the class {classNames[0]}");
        }
        else if (classNames.Count > 1)
        {
            _logger.Log(LogLevel.Information, $"File {fileName}.cs contains multiple classes: {string.Join(", ", classNames)}");
        }
    }

    private void AnalyzeInterfaces(string fileName, IReadOnlyList<string> interfaceNames)
    {
        if (interfaceNames.Count == 1)
        {
            if (_config.StrictNaming && interfaceNames[0] != fileName)
            {
                _logger.Log(LogLevel.Warning, $"Discrepancy: File {fileName}.cs contains the interface {interfaceNames[0]}");
            }
            else if (_config.StrictNaming && !fileName.StartsWith("I"))
            {
                _logger.Log(LogLevel.Warning, $"Discrepancy: File {fileName}.cs contains an interface, but the file name doesn't start with 'I'");
            }
        }
        else if (interfaceNames.Count > 1)
        {
            _logger.Log(LogLevel.Information, $"File {fileName}.cs contains multiple interfaces: {string.Join(", ", interfaceNames)}");
        }
    }

    private void AnalyzeEnums(string fileName, IReadOnlyList<string> enumNames)
    {
        if (enumNames.Count == 1 && _config.StrictNaming && fileName != enumNames[0])
        {
            _logger.Log(LogLevel.Warning, $"Discrepancy: File {fileName}.cs contains the enum {enumNames[0]}, but the file name should be {enumNames[0]}.cs");
        }
        else if (enumNames.Count > 1)
        {
            _logger.Log(LogLevel.Information, $"File {fileName}.cs contains multiple enums: {string.Join(", ", enumNames)}");
        }
    }
}
