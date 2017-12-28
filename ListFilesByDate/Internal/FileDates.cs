using System;
using System.Text;

namespace ListFilesByDate.Internal
{
    /// <inheritdoc />
    public class FileDates : IFileDates
    {
        /// <inheritdoc />
        public string FileName { get; set; }

        /// <inheritdoc />
        public DateTime CreationTime { get; set; }

        /// <inheritdoc />
        public DateTime LastAccessTime { get; set; }

        /// <inheritdoc />
        public DateTime LastWriteTime { get; set; }

        /// <inheritdoc />
        public DateTime CreationTimeUtc { get; set; }

        /// <inheritdoc />
        public DateTime LastAccessTimeUtc { get; set; }

        /// <inheritdoc />
        public DateTime LastWriteTimeUtc { get; set; }

        /// <inheritdoc />
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