using System;
using System.Text;

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
}