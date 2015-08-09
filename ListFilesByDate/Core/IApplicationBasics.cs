namespace ListFilesByDate.Core
{
    public interface IApplicationBasics
    {
        void BrowseFolder();

        void BrowseLoggingFolder();

        string GetInitialDirectory();

        string GetLoggingPath();
    }
}