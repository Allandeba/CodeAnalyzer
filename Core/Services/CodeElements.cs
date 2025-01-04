using System.Text.RegularExpressions;

namespace CodeAnalyzer.Core.Services;

public class CodeElements
{
    public IReadOnlyList<string> ClassNames { get; }
    public IReadOnlyList<string> InterfaceNames { get; }
    public IReadOnlyList<string> EnumNames { get; }

    public bool IsEmpty => !ClassNames.Any() && !InterfaceNames.Any() && !EnumNames.Any();
    public bool HasMultipleTypes => (ClassNames.Any() ? 1 : 0) + (InterfaceNames.Any() ? 1 : 0) + (EnumNames.Any() ? 1 : 0) > 1;

    public CodeElements(string content)
    {
        ClassNames = ExtractNames(content, @"(?<=class\s+)\w+");
        InterfaceNames = ExtractNames(content, @"(?<=interface\s+)I\w+");
        EnumNames = ExtractNames(content, @"(?<=enum\s+)\w+");
    }

    private static IReadOnlyList<string> ExtractNames(string content, string pattern)
        => Regex.Matches(content, pattern, RegexOptions.Compiled)
                .Cast<Match>()
                .Select(m => m.Value)
                .ToList()
                .AsReadOnly();
}
