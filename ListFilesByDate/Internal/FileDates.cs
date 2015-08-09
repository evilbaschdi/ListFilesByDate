namespace ListFilesByDate.Internal
{
    public class FileDates : IFileDates
    {
        public string FileName { get; set; }

        public string CreationTime { get; set; }

        public string LastAccessTime { get; set; }

        public string LastWriteTime { get; set; }

        public string CreationTimeUtc { get; set; }

        public string LastAccessTimeUtc { get; set; }

        public string LastWriteTimeUtc { get; set; }
    }
}