using UnityEngine.Networking;

namespace CommonCs.ResDownloader
{
    public class DownloadInfo
    {
        public string URL { get; set; }
        public string FileName { get; set; }
        public DownloadHandler Handler { get; set; }
        public bool OutputError { get; set; } = true;
    }
}
