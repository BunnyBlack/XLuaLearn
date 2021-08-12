using System.Collections.Generic;
using System.IO;
using System.Xml;
using CommonCs.ResDownloader;
using CommonCs.Utils;
using UnityEngine;

namespace CommonCs.HotFixMgr
{
    public class HotFixHelper : MonoBehaviour
    {
        private byte[] localFileIndexData;
        private readonly List<string> releasedFileNames = new List<string>();
        private string localVersion;
        private string serverVersion;
        private string versionSpan;

        public void ReleaseStreamingAssets()
        {
            // 读取本地的fileIndex筛选出所有的bundle 注意 所有的manifest文件和StreamingAssets的两个都是不需要的
            var localFileIndexPath = Global.FileIndexConfigPath;
            var localFileIndexDownloadInfo = new DownloadInfo
            {
                URL = localFileIndexPath
            };

            StartCoroutine(DownLoadHelper.Inst().DownloadFile(localFileIndexDownloadInfo, OnReleaseFileIndexComplete));
        }

        private void OnReleaseFileIndexComplete(DownloadInfo info)
        {
            if (info.Handler is null)
                return;

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

            StartCoroutine(DownLoadHelper.Inst()
                .DownloadAllFile(downloadInfoList, OnReleaseSingleBundleComplete, OnReleaseAllBundlesComplete));
        }

        private void OnReleaseSingleBundleComplete(DownloadInfo info)
        {
            if (info.Handler == null)
                return;

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

            CheckUpdate();
        }


        public bool IsFirstInstall()
        {
            var versionFilePath = CommonUtil.GetStandardPath(Path.Combine(Global.PersistentDataPath, Global.FileIndexName));
            return !File.Exists(versionFilePath);
        }

        public bool IsWholePackageMode()
        {
            // 整包模式
            return File.Exists(Global.FileIndexConfigPath);
        }

        public void CheckUpdate()
        {
            ParseLocalVersion();
            GetServerVersionConfig();
        }


        private void ParseLocalVersion()
        {
            var localFileIndex = CommonUtil.GetStandardPath(Path.Combine(Global.PersistentDataPath, Global.FileIndexName));
            if (!File.Exists(localFileIndex))
                return;

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(localFileIndex);
            if (xmlDoc.DocumentElement == null)
            {
                Debug.LogError("本地版本解析错误");
                return;
            }
            localVersion = xmlDoc.DocumentElement.GetAttribute("version");
        }

        private void GetServerVersionConfig()
        {
            var downloadInfo = new DownloadInfo
            {
                URL = CommonUtil.GetStandardPath(Path.Combine(Global.ResourceServerAddress, Global.ServerVersionConfigName))
            };
            StartCoroutine(DownLoadHelper.Inst().DownloadFile(downloadInfo, OnDownloadServerVersionConfigComplete));
        }

        private void OnDownloadServerVersionConfigComplete(DownloadInfo info)
        {
            if (info.Handler == null)
                return;

            var configText = info.Handler.text;
            var configList = configText.Split('\n');
            var newestVersionTxt = configList[0];
            var versionSpanTxt = configList[1];
            serverVersion = newestVersionTxt.Split('=')[1].Trim();
            versionSpan = versionSpanTxt.Split('=')[1].Trim();

            if (localVersion == serverVersion)
            {
                Debug.Log($"版本已达到最新版本{localVersion}，进入游戏");
                GameSystem.Inst.EnterGame();
            }
            else
            {
                Debug.Log($"服务器版本{serverVersion}，本地版本{localVersion}，开始更新!");
                DoUpdate();
            }
        }

        private void DoUpdate()
        {
            var fileName = $"DiffPack_{localVersion}to{serverVersion}.zip";
            var downloadInfo = new DownloadInfo
            {
                FileName = fileName,
                URL = CommonUtil.GetStandardPath(Path.Combine(Global.ResourceServerAddress, fileName)),
                OutputError = false
            };
            StartCoroutine(DownLoadHelper.Inst().DownloadFile(downloadInfo, OnDownloadDiffPackComplete));
        }

        private void OnDownloadDiffPackComplete(DownloadInfo info)
        {
            if (info.Handler == null)
            {
                Debug.Log("未下载到差异包，开始下载整包");
                var fileName = $"OriginBundle_ver{serverVersion}.zip";
                var downloadInfo = new DownloadInfo
                {
                    URL = CommonUtil.GetStandardPath(Path.Combine(Global.ResourceServerAddress,
                        fileName)),
                    FileName = fileName
                };
                StartCoroutine(DownLoadHelper.Inst().DownloadFile(downloadInfo, OnDownloadOriginPackComplete));
            }
            else
            {
                FileUtil.WriteFileToPath(Global.PersistentDataPath, info.FileName, info.Handler.data);
                Debug.Log("下载差异包完成");
                ExtractPack(CommonUtil.GetStandardPath(Path.Combine(Global.PersistentDataPath, info.FileName)),
                    Global.PersistentDataPath);
            }
        }

        private void OnDownloadOriginPackComplete(DownloadInfo info)
        {
            if (info.Handler == null)
            {
                Debug.LogError($"未下载到{serverVersion}版本的整包！");
                return;
            }
            FileUtil.WriteFileToPath(Global.PersistentDataPath, info.FileName, info.Handler.data);
            Debug.Log("下载原始包完成");
            ExtractPack(CommonUtil.GetStandardPath(Path.Combine(Global.PersistentDataPath, info.FileName)),
                Global.PersistentDataPath);
        }

        private void ExtractPack(string file, string dir)
        {
            ZipHelper.Unzip(file, dir);
            if (File.Exists(file))
            {
                File.Delete(file);
            }

            GameSystem.Inst.EnterGame();
        }
    }
}
