using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shell;
using EvilBaschdi.About.Core;
using EvilBaschdi.About.Core.Models;
using EvilBaschdi.About.Wpf;
using EvilBaschdi.Core;
using EvilBaschdi.Core.Internal;
using EvilBaschdi.Core.Model;
using EvilBaschdi.Core.Settings.ByMachineAndUser;
using EvilBaschdi.Core.Wpf;
using EvilBaschdi.Core.Wpf.FlyOut;
using ListFilesByDate.Core;
using ListFilesByDate.Internal;
using ListFilesByDate.Model;
using MahApps.Metro.Controls;
using Cursors = System.Windows.Input.Cursors;
using NumericUpDown = MahApps.Metro.Controls.NumericUpDown;
using TextBox = System.Windows.Controls.TextBox;

namespace ListFilesByDate;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class MainWindow : MetroWindow
{
    private readonly IAppSettingByKey _appSettingByKey;
    private readonly IApplicationBasics _basics;
    private readonly ICheckFileDates _checkFileDates;
    private readonly ICurrentFlyOuts _currentFlyOuts;
    private readonly IToggleFlyOut _toggleFlyOut;
    private string _dateType;
    private bool? _direction;
    private DateTime _filterDate;
    private string _initialDirectory;
    private string _loggingPath;

    /// <summary>
    ///     Constructor
    /// </summary>
    public MainWindow()
    {
        IAppSettingsFromJsonFile appSettingsFromJsonFile = new AppSettingsFromJsonFile();
        IAppSettingsFromJsonFileByMachineAndUser appSettingsFromJsonFileByMachineAndUser = new AppSettingsFromJsonFileByMachineAndUser();
        _appSettingByKey = new AppSettingByKey(appSettingsFromJsonFile, appSettingsFromJsonFileByMachineAndUser);
        _basics = new ApplicationBasics(_appSettingByKey);
        InitializeComponent();

        IApplicationStyle applicationStyle = new ApplicationStyle();
        IApplicationLayout applicationLayout = new ApplicationLayout();
        applicationStyle.Run();
        applicationLayout.RunFor((true, false));

        TaskbarItemInfo = new();
        ValidateForm();
        _checkFileDates = new CheckFileDates();
        _currentFlyOuts = new CurrentFlyOuts();
        _toggleFlyOut = new ToggleFlyOut();
    }

    //todo: fileextension.ignore and foldername.ignore
    //todo: save settings
    //todo: Logging
    private void ValidateForm()
    {
        Check.SetCurrentValue(IsEnabledProperty, !string.IsNullOrWhiteSpace(_basics.InitialDirectory) &&
                                                 Directory.Exists(_basics.InitialDirectory));

        _initialDirectory = _basics.InitialDirectory;
        InitialDirectory.SetCurrentValue(TextBox.TextProperty, _initialDirectory);

        _loggingPath = !string.IsNullOrWhiteSpace(_basics.LoggingPath)
            ? _basics.LoggingPath
            : _basics.InitialDirectory;
        LoggingPath.SetCurrentValue(TextBox.TextProperty, _loggingPath);

        var lastDate = Convert.ToDateTime(_appSettingByKey.ValueFor("LastDate"));
        var date = lastDate.Year == 0001
            ? DateTime.Now
            : lastDate;
        FilterDate.SetCurrentValue(DatePicker.SelectedDateProperty, date.Date);
        FilterHour.SetCurrentValue(NumericUpDown.ValueProperty, (double?)date.Hour);
        FilterMinute.SetCurrentValue(NumericUpDown.ValueProperty, (double?)date.Minute);
    }

    private async void CheckDatesOnClick(object sender, RoutedEventArgs e)
    {
        await RunCheckDatesAsync();
    }

    private async Task RunCheckDatesAsync()
    {
        TaskbarItemInfo.SetCurrentValue(TaskbarItemInfo.ProgressStateProperty, TaskbarItemProgressState.Indeterminate);
        SetCurrentValue(CursorProperty, Cursors.Wait);

        _dateType = DateType.Text;
        _filterDate = GetFilterDateTime();
        _direction = SearchDirection.IsOn;
        var task = Task<ObservableCollection<FileDates>>.Factory.StartNew(CheckDates);
        await task;

        ResultGrid.SetCurrentValue(ItemsControl.ItemsSourceProperty, task.Result);

        TaskbarItemInfo.SetCurrentValue(TaskbarItemInfo.ProgressStateProperty, TaskbarItemProgressState.Normal);
        SetCurrentValue(CursorProperty, Cursors.Arrow);
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

        var filePath = new FileListFromPath();
        var filePathFilter = new FileListFromPathFilter
                             {
                                 FilterExtensionsNotToEqual = excludeExtensionList,
                                 FilterFileNamesNotToEqual = excludeFileNameList,
                                 FilterFilePathsToEqual = excludeFilePathList
                             };
        var fileList = filePath.ValueFor(_initialDirectory.ToLower(), filePathFilter).Distinct();

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

        return new(concurrentBag);
    }

    private DateTime GetFilterDateTime()
    {
        // ReSharper disable once PossibleInvalidOperationException
        var filterDate = FilterDate.SelectedDate.Value;
        return new(filterDate.Year, filterDate.Month, filterDate.Day, Convert.ToInt32(FilterHour.Value),
            Convert.ToInt32(FilterMinute.Value), 0);
    }

    private void AboutWindowClick(object sender, RoutedEventArgs e)
    {
        ICurrentAssembly currentAssembly = new CurrentAssembly();
        IAboutContent aboutContent = new AboutContent(currentAssembly);
        IAboutViewModel aboutModel = new AboutViewModel(aboutContent);
        IApplyMicaBrush applyMicaBrush = new ApplyMicaBrush();
        IApplicationLayout applicationLayout = new ApplicationLayout();
        var aboutWindow = new AboutWindow(aboutModel, applicationLayout, applyMicaBrush);

        aboutWindow.ShowDialog();
    }

    #region Flyout

    private void ToggleSettingsFlyoutClick(object sender, RoutedEventArgs e)
    {
        var currentFlyOutsModel = _currentFlyOuts.ValueFor(Flyouts, 0);
        _toggleFlyOut.RunFor(currentFlyOutsModel);
    }

    #endregion Flyout

    #region Initial Directory

    private void BrowseClick(object sender, RoutedEventArgs e)
    {
        _basics.BrowseFolder();
        InitialDirectory.SetCurrentValue(TextBox.TextProperty, _basics.LoggingPath);
        _initialDirectory = _basics.LoggingPath;
        ValidateForm();
    }

    private void InitialDirectoryOnLostFocus(object sender, RoutedEventArgs e)
    {
        if (Directory.Exists(InitialDirectory.Text))
        {
            _basics.InitialDirectory = InitialDirectory.Text;
            _initialDirectory = _basics.InitialDirectory;
        }

        ValidateForm();
    }

    #endregion Initial Directory

    #region Check Settings

    private void BrowseLoggingPathClick(object sender, RoutedEventArgs e)
    {
        _basics.BrowseLoggingFolder();
        LoggingPath.SetCurrentValue(TextBox.TextProperty, _basics.LoggingPath);
        _loggingPath = _basics.LoggingPath;
        ValidateForm();
    }

    private void LoggingPathOnLostFocus(object sender, RoutedEventArgs e)
    {
        if (Directory.Exists(LoggingPath.Text))
        {
            _basics.LoggingPath = LoggingPath.Text;
            _loggingPath = _basics.LoggingPath;
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
        ((Calendar)e.Parameter).SetCurrentValue(Calendar.SelectedDateProperty, DateTime.Now.Date);
    }

    #endregion Choose Date
}