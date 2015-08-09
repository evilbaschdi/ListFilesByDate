using System;

namespace ListFilesByDate.Internal
{
    public interface ICheckFileDates
    {
        FileDates For(string path);

        bool IsDifferent(string path, string dateType, DateTime filter, bool? direction);
    }
}