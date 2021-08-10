using UnityEngine;
using UnityEngine.Networking;

namespace CommonCs.ResDownloader
{
    public class DownloadFileInfo
    {
        public string URL { get; set; }
        /// <summary>
        /// 已包含在URL中
        /// </summary>
        public string BundleName { get; set; }
        public DownloadHandler Handler { get; set; }
    }
}
