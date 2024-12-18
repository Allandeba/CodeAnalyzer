using CodeAnalyzer.Core.Interfaces;

namespace CodeAnalyzer.Core.Models;

public class FileSystemAccessor : IFileSystemAccessor
{
    public IEnumerable<string> GetFiles(string path, string searchPattern, SearchOption searchOption)
        => Directory.GetFiles(path, searchPattern, searchOption);

    public string ReadAllText(string path) => File.ReadAllText(path);
}