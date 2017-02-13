using System;

namespace ListFilesByDate.Internal
{
    public interface IFileDates
    {
        string FileName { get; set; }

        DateTime CreationTime { get; set; }

        DateTime LastAccessTime { get; set; }

        DateTime LastWriteTime { get; set; }

        DateTime CreationTimeUtc { get; set; }

        DateTime LastAccessTimeUtc { get; set; }

        DateTime LastWriteTimeUtc { get; set; }
    }
}