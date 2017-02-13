using System;

namespace ListFilesByDate.Internal
{
    public class FileDates : IFileDates
    {
        public string FileName { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime LastAccessTime { get; set; }

        public DateTime LastWriteTime { get; set; }

        public DateTime CreationTimeUtc { get; set; }

        public DateTime LastAccessTimeUtc { get; set; }

        public DateTime LastWriteTimeUtc { get; set; }
    }
}