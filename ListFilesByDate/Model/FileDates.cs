using System.Text;

namespace ListFilesByDate.Model;

/// <summary>
/// </summary>
public class FileDates
{
    /// <summary>
    /// </summary>

    public DateTime CreationTime { get; init; }

    /// <summary>
    /// </summary>
    public DateTime CreationTimeUtc { get; init; }

    /// <summary>
    /// </summary>
    public string FileName { get; init; }

    /// <summary>
    /// </summary>
    public DateTime LastAccessTime { get; init; }

    /// <summary>
    /// </summary>

    public DateTime LastAccessTimeUtc { get; init; }

    /// <summary>
    /// </summary>

    public DateTime LastWriteTime { get; init; }

    /// <summary>
    /// </summary>

    public DateTime LastWriteTimeUtc { get; init; }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append($"FileName: {FileName}; ");
        stringBuilder.Append($"CreationTime: {CreationTime}; ");
        stringBuilder.Append($"LastAccessTime: {LastAccessTime}; ");
        stringBuilder.Append($"LastWriteTime: {LastWriteTime}; ");
        stringBuilder.Append($"CreationTimeUtc: {CreationTimeUtc}; ");
        stringBuilder.Append($"LastAccessTimeUtc: {LastAccessTimeUtc}; ");
        stringBuilder.Append($"LastWriteTimeUtc: {LastWriteTimeUtc}; ");

        return stringBuilder.ToString();
    }
}