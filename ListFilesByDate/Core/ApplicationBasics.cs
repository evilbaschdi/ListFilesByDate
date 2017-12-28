using System.Windows.Forms;

namespace ListFilesByDate.Core
{
    /// <inheritdoc />
    public class ApplicationBasics : IApplicationBasics
    {
        /// <inheritdoc />
        public void BrowseFolder()
        {
            var folderDialog = new FolderBrowserDialog
                               {
                                   SelectedPath = InitialDirectory
                               };

            var result = folderDialog.ShowDialog();
            if (result.ToString() != "OK")
            {
                return;
            }

            Properties.Settings.Default.InitialDirectory = folderDialog.SelectedPath;
            Properties.Settings.Default.Save();
        }

        /// <inheritdoc />
        public string InitialDirectory => string.IsNullOrWhiteSpace(Properties.Settings.Default.InitialDirectory)
            ? ""
            : Properties.Settings.Default.InitialDirectory;

        /// <inheritdoc />
        public void BrowseLoggingFolder()
        {
            var folderDialog = new FolderBrowserDialog
                               {
                                   SelectedPath = LoggingPath
                               };

            var result = folderDialog.ShowDialog();
            if (result.ToString() != "OK")
            {
                return;
            }

            Properties.Settings.Default.LoggingPath = folderDialog.SelectedPath;
            Properties.Settings.Default.Save();
        }

        /// <inheritdoc />
        public string LoggingPath => string.IsNullOrWhiteSpace(Properties.Settings.Default.LoggingPath)
            ? ""
            : Properties.Settings.Default.LoggingPath;
    }
}