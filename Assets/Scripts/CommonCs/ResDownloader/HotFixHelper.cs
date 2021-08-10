using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Networking;

namespace CommonCs.ResDownloader
{
    public class HotFixHelper : MonoBehaviour
    {
        /// <summary>
        /// 异步下载单个bundle文件 可以是web服务器上的也可以是本地的
        /// </summary>
        /// <param name="info">需要下载的文件信息</param>
        /// <param name="onComplete">下载完成的回调</param>
        /// <returns></returns>
        public IEnumerator DownloadFile(DownloadFileInfo info, Action<DownloadFileInfo> onComplete)
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
        public IEnumerator DownloadAllFile(List<DownloadFileInfo> infos, Action<DownloadFileInfo> onComplete, Action onAllComplete)
        {
            foreach (var downloadFileInfo in infos)
            {
                yield return DownloadFile(downloadFileInfo, onComplete);
            }
            onAllComplete?.Invoke();
        }

        public void Init()
        {
            
        }
        
    }
}
