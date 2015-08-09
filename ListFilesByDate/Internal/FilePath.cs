using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ListFilesByDate.Internal
{
    public class FilePath : IFilePath
    {
        private IEnumerable<string> GetSubdirectoriesContainingOnlyFiles(string path)
        {
            return Directory.GetDirectories(path, "*", SearchOption.AllDirectories).ToList();
        }

        public List<string> GetFileList(string initialDirectory)
        {
            var fileList = new List<string>();
            var initialDirectoryFileList = Directory.GetFiles(initialDirectory).ToList();

            foreach(var file in initialDirectoryFileList.Where(file => IsValidFileName(file, fileList)))
            {
                fileList.Add(file);
            }

            var initialDirectorySubdirectoriesFileList =
                GetSubdirectoriesContainingOnlyFiles(initialDirectory).SelectMany(Directory.GetFiles);

            foreach(var file in initialDirectorySubdirectoriesFileList.Where(file => IsValidFileName(file, fileList)))
            {
                fileList.Add(file);
            }

            return fileList;
        }

        private bool IsValidFileName(string file, ICollection<string> fileList)
        {
            var ftl = file.ToLower();
            var fileExtension = Path.GetExtension(ftl);

            return !fileList.Contains(file) && !ftl.Contains("listfilesbydate_log_") &&
                   !string.IsNullOrWhiteSpace(fileExtension) && !fileExtension.Equals(".db") &&
                   !fileExtension.Equals(".sln") &&
                   !ftl.Contains("deploy") && !ftl.Contains(".vs") && !ftl.Contains("argos-localizer") &&
                   !ftl.Contains(".git");
        }
    }
}