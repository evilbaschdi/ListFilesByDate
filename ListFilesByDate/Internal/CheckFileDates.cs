using System.IO;
using ListFilesByDate.Model;

namespace ListFilesByDate.Internal;

/// <inheritdoc />
public class CheckFileDates : ICheckFileDates
{
    /// <inheritdoc />
    public bool IsDifferent(string path, string dateType, DateTime filter, bool? direction)
    {
        ArgumentNullException.ThrowIfNull(path);

        ArgumentNullException.ThrowIfNull(dateType);

        var fileDate = new DateTime();

        switch (dateType)
        {
            case "creation time":
                fileDate = File.GetCreationTime(path);
                break;

            case "last access time":
                fileDate = File.GetLastAccessTime(path);
                break;
            //change date.
            case "last write time":
                fileDate = File.GetLastWriteTime(path);
                break;
        }

        if (direction == true)
        {
            return fileDate < filter;
        }

        return fileDate > filter;
    }

    /// <inheritdoc />
    public FileDates ValueFor(string path)
    {
        ArgumentNullException.ThrowIfNull(path);

        var fileDates = new FileDates
                        {
                            FileName = Path.GetFileName(path),
                            CreationTime = File.GetCreationTime(path),
                            LastAccessTime = File.GetLastAccessTime(path),
                            LastWriteTime = File.GetLastWriteTime(path),
                            CreationTimeUtc = File.GetCreationTimeUtc(path),
                            LastAccessTimeUtc = File.GetLastAccessTimeUtc(path),
                            LastWriteTimeUtc = File.GetLastWriteTimeUtc(path)
                        };

        return fileDates;
    }
}