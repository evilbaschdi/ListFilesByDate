﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;
using EvilBaschdi.Core.Internal;
using EvilBaschdi.Core.Model;
using EvilBaschdi.CoreExtended.AppHelpers;
using EvilBaschdi.CoreExtended.Metro;
using EvilBaschdi.CoreExtended.Mvvm;
using EvilBaschdi.CoreExtended.Mvvm.View;
using EvilBaschdi.CoreExtended.Mvvm.ViewModel;
using ListFilesByDate.Core;
using ListFilesByDate.Internal;
using ListFilesByDate.Model;
using ListFilesByDate.Properties;
using MahApps.Metro.Controls;

namespace ListFilesByDate
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : MetroWindow
    {
        private readonly IApplicationStyle _applicationStyle;
        private readonly IAppSettingsBase _appSettingsBase;
        private readonly IApplicationBasics _basics;
        private readonly ICheckFileDates _checkFileDates;
        private readonly IThemeManagerHelper _themeManagerHelper;
        private string _dateType;
        private bool? _direction;
        private DateTime _filterDate;
        private string _initialDirectory;
        private string _loggingPath;
        private int _overrideProtection;

        /// <summary>
        ///     Constructor
        /// </summary>
        public MainWindow()
        {
            _appSettingsBase = new AppSettingsBase(Settings.Default);
            _basics = new ApplicationBasics(_appSettingsBase);
            InitializeComponent();
            _themeManagerHelper = new ThemeManagerHelper();
            _applicationStyle = new ApplicationStyle(_themeManagerHelper);
            _applicationStyle.Load(true);
            TaskbarItemInfo = new TaskbarItemInfo();
            ValidateForm();
            _checkFileDates = new CheckFileDates();
        }

        //todo: fileextension.ignore and foldername.ignore
        //todo: save settings
        //todo: Logging
        private void ValidateForm()
        {
            Check.IsEnabled = !string.IsNullOrWhiteSpace(_appSettingsBase.Get<string>("InitialDirectory")) &&
                              Directory.Exists(_appSettingsBase.Get<string>("InitialDirectory"));

            _initialDirectory = _basics.InitialDirectory;
            InitialDirectory.Text = _initialDirectory;

            _loggingPath = !string.IsNullOrWhiteSpace(_basics.LoggingPath)
                ? _basics.LoggingPath
                : _basics.InitialDirectory;
            LoggingPath.Text = _loggingPath;

            var lastDate = _appSettingsBase.Get<DateTime>("LastDate");
            var date = lastDate.Year == 0001
                ? DateTime.Now
                : lastDate;
            FilterDate.SelectedDate = date.Date;
            FilterHour.Value = date.Hour;
            FilterMinute.Value = date.Minute;
            _overrideProtection = 1;
        }


        private async void CheckDatesOnClick(object sender, RoutedEventArgs e)
        {
            await RunCheckDatesAsync();
        }


        private async Task RunCheckDatesAsync()
        {
            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Indeterminate;
            Cursor = Cursors.Wait;

            _dateType = DateType.Text;
            _filterDate = GetFilterDateTime();
            _direction = SearchDirection.IsChecked;
            var task = Task<ObservableCollection<FileDates>>.Factory.StartNew(CheckDates);
            await task;

            ResultGrid.ItemsSource = task.Result;

            TaskbarItemInfo.ProgressState = TaskbarItemProgressState.Normal;
            Cursor = Cursors.Arrow;
        }

        private ObservableCollection<FileDates> CheckDates()
        {
            var concurrentBag = new ConcurrentBag<FileDates>();
            var excludeExtensionList = new List<string>
                                       {
                                           "sln",
                                           "db"
                                       };

            var excludeFileNameList = new List<string>
                                      {
                                          "listfilesbydate_log_"
                                      };

            var excludeFilePathList = new List<string>
                                      {
                                          "deploy",
                                          ".vs",
                                          "argos-localizer",
                                          ".git"
                                      };

            var multiThreadingHelper = new MultiThreading();
            var filePath = new FileListFromPath(multiThreadingHelper);
            var filePathFilter = new FileListFromPathFilter
                                 {
                                     FilterExtensionsNotToEqual = excludeExtensionList,
                                     FilterFileNamesNotToEqual = excludeFileNameList,
                                     FilterFilePathsToEqual = excludeFilePathList
                                 };
            var fileList = filePath.ValueFor(_initialDirectory, filePathFilter).Distinct();

            Parallel.ForEach(fileList,
                file =>
                {
                    // DateTime.Today, Time.LastWrite
                    if (_checkFileDates.IsDifferent(file, _dateType, _filterDate, _direction))
                    {
                        concurrentBag.Add(_checkFileDates.ValueFor(file));
                    }
                });

            //File.AppendAllText($@"{_loggingPath}\ListFilesByDate_Log_{DateTime.Now:yyyy-MM-dd_HHmm}.txt", _result);

            return new ObservableCollection<FileDates>(concurrentBag);
        }

        private DateTime GetFilterDateTime()
        {
            // ReSharper disable once PossibleInvalidOperationException
            var filterDate = FilterDate.SelectedDate.Value;
            return new DateTime(filterDate.Year, filterDate.Month, filterDate.Day, Convert.ToInt32(FilterHour.Value),
                Convert.ToInt32(FilterMinute.Value), 0);
        }

        private void AboutWindowClick(object sender, RoutedEventArgs e)
        {
            var assembly = typeof(MainWindow).Assembly;
            IAboutWindowContent aboutWindowContent = new AboutWindowContent(assembly, $@"{AppDomain.CurrentDomain.BaseDirectory}\fbd_512.png");

            var aboutWindow = new AboutWindow
                              {
                                  DataContext = new AboutViewModel(aboutWindowContent, _themeManagerHelper)
                              };

            aboutWindow.ShowDialog();
        }

        #region Initial Directory

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            _basics.BrowseFolder();
            InitialDirectory.Text = _appSettingsBase.Get<string>("InitialDirectory");
            _initialDirectory = _appSettingsBase.Get<string>("InitialDirectory");
            ValidateForm();
        }

        private void InitialDirectoryOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(InitialDirectory.Text))
            {
                _appSettingsBase.Set("InitialDirectory", InitialDirectory.Text);
                _initialDirectory = _appSettingsBase.Get<string>("InitialDirectory");
            }

            ValidateForm();
        }

        #endregion Initial Directory

        #region Flyout

        private void ToggleSettingsFlyoutClick(object sender, RoutedEventArgs e)
        {
            ToggleFlyout(0);
        }

        private void ToggleFlyout(int index, bool stayOpen = false)
        {
            var activeFlyout = (Flyout) Flyouts.Items[index];
            if (activeFlyout == null)
            {
                return;
            }

            foreach (
                var nonactiveFlyout in
                Flyouts.Items.Cast<Flyout>()
                       .Where(nonactiveFlyout => nonactiveFlyout.IsOpen && nonactiveFlyout.Name != activeFlyout.Name))
            {
                nonactiveFlyout.IsOpen = false;
            }

            if (activeFlyout.IsOpen && stayOpen)
            {
                activeFlyout.IsOpen = true;
            }
            else
            {
                activeFlyout.IsOpen = !activeFlyout.IsOpen;
            }
        }

        #endregion Flyout


        #region Check Settings

        private void BrowseLoggingPathClick(object sender, RoutedEventArgs e)
        {
            _basics.BrowseLoggingFolder();
            LoggingPath.Text = _appSettingsBase.Get<string>("LoggingPath");
            _loggingPath = _appSettingsBase.Get<string>("LoggingPath");
            ValidateForm();
        }

        private void LoggingPathOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(LoggingPath.Text))
            {
                _appSettingsBase.Set("LoggingPath", LoggingPath.Text);
                _loggingPath = _appSettingsBase.Get<string>("LoggingPath");
            }

            ValidateForm();
        }

        #endregion Check Settings

        #region Choose Date

        private void CommandBindingCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CommandBindingExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            ((Calendar) e.Parameter).SelectedDate = DateTime.Now.Date;
        }

        #endregion Choose Date
    }
}