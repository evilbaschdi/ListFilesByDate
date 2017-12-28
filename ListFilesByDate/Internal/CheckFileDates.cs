using System;
using System.IO;

namespace ListFilesByDate.Internal
{
    /// <inheritdoc />
    public class CheckFileDates : ICheckFileDates
    {
        /// <inheritdoc />
        public bool IsDifferent(string path, string dateType, DateTime filter, bool? direction)
        {
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
            if (dateType == null)
            {
                throw new ArgumentNullException(nameof(dateType));
            }

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
            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }
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
}