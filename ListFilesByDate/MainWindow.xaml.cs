using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;
using EvilBaschdi.Core.Application;
using EvilBaschdi.Core.DirectoryExtensions;
using EvilBaschdi.Core.Threading;
using EvilBaschdi.Core.Wpf;
using ListFilesByDate.Core;
using ListFilesByDate.Internal;
using MahApps.Metro.Controls;
using Calendar = System.Windows.Controls.Calendar;

namespace ListFilesByDate
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : MetroWindow
    {
        private readonly IMetroStyle _style;
        private readonly ICheckFileDates _checkFileDates;
        private readonly IApplicationBasics _basics;
        private string _dateType;
        private string _loggingPath;
        private string _initialDirectory;
        private int _overrideProtection;
        private DateTime _filterDate;
        private bool? _direction;

        /// <summary>
        ///     Constructor
        /// </summary>
        public MainWindow()
        {
            _basics = new ApplicationBasics();
            InitializeComponent();
            ISettings coreSettings = new CoreSettings(Properties.Settings.Default);
            IThemeManagerHelper themeManagerHelper = new ThemeManagerHelper();
            _style = new MetroStyle(this, Accent, ThemeSwitch, coreSettings, themeManagerHelper);
            _style.Load(true);
            TaskbarItemInfo = new TaskbarItemInfo();
            ValidateForm();
            _checkFileDates = new CheckFileDates();
        }

        //todo: fileextension.ignore and foldername.ignore
        //todo: save settings
        //todo: Logging
        private void ValidateForm()
        {
            var linkerTime = Assembly.GetExecutingAssembly().GetLinkerTime();
            LinkerTime.Content = linkerTime.ToString(CultureInfo.InvariantCulture);
            Check.IsEnabled = !string.IsNullOrWhiteSpace(Properties.Settings.Default.InitialDirectory) &&
                              Directory.Exists(Properties.Settings.Default.InitialDirectory);

            _initialDirectory = _basics.InitialDirectory;
            InitialDirectory.Text = _initialDirectory;

            _loggingPath = !string.IsNullOrWhiteSpace(_basics.LoggingPath)
                ? _basics.LoggingPath
                : _basics.InitialDirectory;
            LoggingPath.Text = _loggingPath;

            var date = Properties.Settings.Default.LastDate.Year == 0001
                ? DateTime.Now
                : Properties.Settings.Default.LastDate;
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

            var multiThreadingHelper = new MultiThreadingHelper();
            var filePath = new FilePath(multiThreadingHelper);
            var fileList = filePath.GetFileList(_initialDirectory, null, excludeExtensionList, null, excludeFileNameList, null, excludeFilePathList).Distinct();

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

        #region Initial Directory

        private void BrowseClick(object sender, RoutedEventArgs e)
        {
            _basics.BrowseFolder();
            InitialDirectory.Text = Properties.Settings.Default.InitialDirectory;
            _initialDirectory = Properties.Settings.Default.InitialDirectory;
            ValidateForm();
        }

        private void InitialDirectoryOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(InitialDirectory.Text))
            {
                Properties.Settings.Default.InitialDirectory = InitialDirectory.Text;
                Properties.Settings.Default.Save();
                _initialDirectory = Properties.Settings.Default.InitialDirectory;
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

        #region MetroStyle

        private void SaveStyleClick(object sender, RoutedEventArgs e)
        {
            if (_overrideProtection == 0)
            {
                return;
            }
            _style.SaveStyle();
        }

        private void Theme(object sender, EventArgs e)
        {
            if (_overrideProtection == 0)
            {
                return;
            }

            _style.SetTheme(sender);
        }

        private void AccentOnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_overrideProtection == 0)
            {
                return;
            }
            _style.SetAccent(sender, e);
        }

        #endregion MetroStyle

        #region Check Settings

        private void BrowseLoggingPathClick(object sender, RoutedEventArgs e)
        {
            _basics.BrowseLoggingFolder();
            LoggingPath.Text = Properties.Settings.Default.LoggingPath;
            _loggingPath = Properties.Settings.Default.LoggingPath;
            ValidateForm();
        }

        private void LoggingPathOnLostFocus(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(LoggingPath.Text))
            {
                Properties.Settings.Default.LoggingPath = LoggingPath.Text;
                Properties.Settings.Default.Save();
                _loggingPath = Properties.Settings.Default.LoggingPath;
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