﻿using System.Windows.Forms;

namespace ListFilesByDate.Core
{
    public class ApplicationBasics : IApplicationBasics
    {
        public void BrowseFolder()
        {
            var folderDialog = new FolderBrowserDialog
            {
                SelectedPath = GetInitialDirectory()
            };

            var result = folderDialog.ShowDialog();
            if(result.ToString() != "OK")
            {
                return;
            }

            Properties.Settings.Default.InitialDirectory = folderDialog.SelectedPath;
            Properties.Settings.Default.Save();
        }

        public string GetInitialDirectory()
        {
            return string.IsNullOrWhiteSpace(Properties.Settings.Default.InitialDirectory)
                ? ""
                : Properties.Settings.Default.InitialDirectory;
        }

        public void BrowseLoggingFolder()
        {
            var folderDialog = new FolderBrowserDialog
            {
                SelectedPath = GetLoggingPath()
            };

            var result = folderDialog.ShowDialog();
            if(result.ToString() != "OK")
            {
                return;
            }

            Properties.Settings.Default.LoggingPath = folderDialog.SelectedPath;
            Properties.Settings.Default.Save();
        }

        public string GetLoggingPath()
        {
            return string.IsNullOrWhiteSpace(Properties.Settings.Default.LoggingPath)
                ? ""
                : Properties.Settings.Default.LoggingPath;
        }
    }
}