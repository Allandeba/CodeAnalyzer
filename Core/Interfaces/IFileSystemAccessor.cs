namespace CodeAnalyzer.Core.Interfaces;

public interface IFileSystemAccessor
{
    IEnumerable<string> GetFiles(string path, string searchPattern, SearchOption searchOption);
    string ReadAllText(string path);
}