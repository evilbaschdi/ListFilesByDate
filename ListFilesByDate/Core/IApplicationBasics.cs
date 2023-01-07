namespace ListFilesByDate.Core;

/// <summary>
/// </summary>
public interface IApplicationBasics
{
    /// <summary>
    /// </summary>
    string InitialDirectory { get; set; }

    /// <summary>
    /// </summary>
    string LoggingPath { get; set; }

    /// <summary>
    /// </summary>
    void BrowseFolder();

    /// <summary>
    /// </summary>
    void BrowseLoggingFolder();
}