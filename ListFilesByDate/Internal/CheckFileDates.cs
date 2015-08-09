using System;
using System.IO;

namespace ListFilesByDate.Internal
{
    public class CheckFileDates : ICheckFileDates
    {
        public FileDates For(string path)
        {
            var fileDates = new FileDates
            {
                FileName = $"file name: {Path.GetFileName(path)}",
                CreationTime = $"creation time: {File.GetCreationTime(path)}",
                LastAccessTime = $"last access time: {File.GetLastAccessTime(path)}",
                LastWriteTime = $"last write time: {File.GetLastWriteTime(path)}",
                CreationTimeUtc = $"creation time (UTC): {File.GetCreationTimeUtc(path)}",
                LastAccessTimeUtc = $"last access time (UTC): {File.GetLastAccessTimeUtc(path)}",
                LastWriteTimeUtc = $"last write time (UTC): {File.GetLastWriteTimeUtc(path)}"
            };

            return fileDates;
        }

        public bool IsDifferent(string path, string dateType, DateTime filter, bool? direction)
        {
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
    }
}