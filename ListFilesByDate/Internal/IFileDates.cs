namespace ListFilesByDate.Internal
{
    public interface IFileDates
    {
        string FileName { get; set; }

        string CreationTime { get; set; }

        string LastAccessTime { get; set; }

        string LastWriteTime { get; set; }

        string CreationTimeUtc { get; set; }

        string LastAccessTimeUtc { get; set; }

        string LastWriteTimeUtc { get; set; }
    }
}