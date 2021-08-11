using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using CommonCs.Utils;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace CommonCs.ResDownloader
{
    public class HotFixHelper : MonoBehaviour
    {
        private byte[] localFileIndexData;
        private readonly List<string> releasedFileNames = new List<string>();
        

        /// <summary>
        /// 异步下载单个bundle文件 可以是web服务器上的也可以是本地的
        /// </summary>
        /// <param name="info">需要下载的文件信息</param>
        /// <param name="onComplete">下载完成的回调</param>
        /// <returns></returns>
        public IEnumerator DownloadFile(DownloadInfo info, Action<DownloadInfo> onComplete)
        {
            var webRequest = UnityWebRequest.Get(info.URL);
            yield return webRequest.SendWebRequest();

            if (webRequest.isHttpError || webRequest.isNetworkError)
            {
                Debug.LogError($"下载文件出错：{info.URL}");
                yield break;
            }

            info.Handler = webRequest.downloadHandler;
            onComplete?.Invoke(info);
            webRequest.Dispose();
        }

        /// <summary>
        /// 异步下载多个文件 可以是web服务器上的也可以是本地的
        /// </summary>
        /// <param name="infos">需要下载的文件信息列表</param>
        /// <param name="onComplete">完成一个下载的回调</param>
        /// <param name="onAllComplete">完成所有下载的回调</param>
        /// <returns></returns>
        public IEnumerator DownloadAllFile(List<DownloadInfo> infos, Action<DownloadInfo> onComplete,
            Action onAllComplete)
        {
            foreach (var downloadFileInfo in infos)
            {
                yield return DownloadFile(downloadFileInfo, onComplete);
            }
            onAllComplete?.Invoke();
        }

        public void ReleaseStreamingAssets()
        {
            // 读取本地的fileIndex筛选出所有的bundle 注意 所有的manifest文件和StreamingAssets的两个都是不需要的
            var localFileIndexPath = Global.FileIndexConfigPath;
            var localFileIndexDownloadInfo = new DownloadInfo
            {
                URL = localFileIndexPath
            };
            
            StartCoroutine(DownloadFile(localFileIndexDownloadInfo, OnReleaseFileIndexComplete));
        }

        private void OnReleaseFileIndexComplete(DownloadInfo info)
        {
            // 解析出fileIndex中所有的bundleName
            var localFileIndexContents = info.Handler.text;
            localFileIndexData = info.Handler.data;
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(localFileIndexContents);
            var elements = xmlDoc.GetElementsByTagName("file");

            var downloadInfoList = new List<DownloadInfo>();

            foreach (XmlElement element in elements)
            {
                var bundleName = element.GetAttribute("bundle_name");
                if (releasedFileNames.Contains(bundleName))
                {
                    continue;
                }
                var bundlePath = CommonUtil.GetStandardPath(Path.Combine(Global.BundleOutputPath, bundleName));
                var bundleDownloadInfo = new DownloadInfo
                {
                    URL = bundlePath,
                    FileName = bundleName
                };
                downloadInfoList.Add(bundleDownloadInfo);
                releasedFileNames.Add(bundleName);
            }
            
            releasedFileNames.Clear();

            // 单独再塞一个bundleDependencies.xml文件进去
            var configPath = Global.BundleDependencyConfigPath;
            var downloadInfo = new DownloadInfo
            {
                URL = configPath,
                FileName = Global.BundleDependenciesName
            };
            downloadInfoList.Add(downloadInfo);

            StartCoroutine(DownloadAllFile(downloadInfoList, OnReleaseSingleBundleComplete, OnReleaseAllBundlesComplete));
        }

        private void OnReleaseSingleBundleComplete(DownloadInfo info)
        {
            // 每释放一个文件 就写到持久化目录中去
            var data = info.Handler.data;
            var fileName = info.FileName;
            var dirPath = Global.PersistentDataPath;

            FileUtil.WriteFileToPath(dirPath, fileName, data);
        }

        private void OnReleaseAllBundlesComplete()
        {
            // 全部文件释放完毕后，把fileIndex文件和bundleDependencies文件释放出来
            var dirPath = Global.PersistentDataPath;
            FileUtil.WriteFileToPath(dirPath, Global.FileIndexName, localFileIndexData);
        }


        public bool IsFirstInstall()
        {
            var versionFilePath = CommonUtil.GetStandardPath(Path.Combine(Global.PersistentDataPath, Global.FileIndexName));
            return !File.Exists(versionFilePath);
        }

        public void CheckUpdate()
        {

        }
    }
}
