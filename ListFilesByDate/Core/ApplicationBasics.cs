using System;
using System.Windows.Forms;
using EvilBaschdi.CoreExtended.AppHelpers;

namespace ListFilesByDate.Core
{
    /// <inheritdoc />
    public class ApplicationBasics : IApplicationBasics
    {
        private readonly IAppSettingsBase _appSettingsBase;


        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="appSettingsBase"></param>
        public ApplicationBasics(IAppSettingsBase appSettingsBase)
        {
            _appSettingsBase = appSettingsBase ?? throw new ArgumentNullException(nameof(appSettingsBase));
        }

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

            _appSettingsBase.Set("InitialDirectory", folderDialog.SelectedPath);
        }

        /// <inheritdoc />
        public string InitialDirectory => _appSettingsBase.Get<string>("InitialDirectory");

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

            _appSettingsBase.Set("LoggingPath", folderDialog.SelectedPath);
        }

        /// <inheritdoc />
        public string LoggingPath => _appSettingsBase.Get<string>("LoggingPath");
    }
}