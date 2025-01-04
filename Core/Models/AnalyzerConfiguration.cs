using System.Text.Json.Serialization;

namespace CodeAnalyzer.Core.Models;

public class AnalyzerConfiguration
{
    [JsonPropertyName("ignoredFiles")]
    public IEnumerable<string> IgnoredFiles { get; set; } = new List<string>();

    [JsonPropertyName("strictNaming")]
    public bool StrictNaming { get; set; } = true;
}
