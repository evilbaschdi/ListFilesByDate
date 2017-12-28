namespace ListFilesByDate.Core
{
    /// <summary>
    /// </summary>
    public interface IApplicationBasics
    {
        /// <summary>
        /// </summary>
        void BrowseFolder();

        /// <summary>
        /// </summary>
        void BrowseLoggingFolder();

        /// <summary>
        /// </summary>
        string InitialDirectory { get; }

        /// <summary>
        /// </summary>
        string LoggingPath { get; }
    }
}