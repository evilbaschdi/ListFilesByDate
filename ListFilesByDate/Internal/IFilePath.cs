using System.Collections.Generic;

namespace ListFilesByDate.Internal
{
    public interface IFilePath
    {
        List<string> GetFileList(string initialDirectory);
    }
}