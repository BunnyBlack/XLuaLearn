using UnityEngine;
using UnityEngine.Networking;

namespace CommonCs.ResDownloader
{
    public class DownloadFileInfo
    {
        public string URL { get; set; }
        public string BundleName { get; set; }
        public DownloadHandler Handler { get; set; }
    }
}
