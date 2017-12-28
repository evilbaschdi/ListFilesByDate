using System;
using EvilBaschdi.Core.DotNetExtensions;

namespace ListFilesByDate.Internal
{
    /// <summary>
    /// </summary>
    public interface ICheckFileDates : IValueFor<string, FileDates>
    {
        /// <summary>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="dateType"></param>
        /// <param name="filter"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        bool IsDifferent(string path, string dateType, DateTime filter, bool? direction);
    }
}