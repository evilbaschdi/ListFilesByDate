using EvilBaschdi.Core.Settings.ByMachineAndUser;
using JetBrains.Annotations;

namespace ListFilesByDate.Core;

/// <inheritdoc />
public class ApplicationBasics : IApplicationBasics
{
    private readonly IAppSettingByKey _appSettingByKey;

    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="appSettingByKey"></param>
    public ApplicationBasics([NotNull] IAppSettingByKey appSettingByKey)
    {
        _appSettingByKey = appSettingByKey ?? throw new ArgumentNullException(nameof(appSettingByKey));
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

        _appSettingByKey.RunFor("InitialDirectory", folderDialog.SelectedPath);
    }

    /// <inheritdoc />
    public string InitialDirectory
    {
        get => _appSettingByKey.ValueFor("InitialDirectory");
        set => _appSettingByKey.RunFor("InitialDirectory", value);
    }

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

        _appSettingByKey.RunFor("LoggingPath", folderDialog.SelectedPath);
    }

    /// <inheritdoc />
    public string LoggingPath
    {
        get => _appSettingByKey.ValueFor("LoggingPath");
        set => _appSettingByKey.RunFor("LoggingPath", value);
    }
}