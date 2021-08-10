using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CommonCs.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonCs.ResMgr
{
    public class ResLoader : MonoBehaviour
    {
        private readonly List<string> bundleConfigList = new List<string>();

        // key -> filename without extension  value -> bundle name
        private readonly Dictionary<string, string> fileIndexDic = new Dictionary<string, string>();

        // key -> bundleName  value -> list of dependent bundle      key depend on value
        private readonly Dictionary<string, List<string>> bundleDependencyDic = new Dictionary<string, List<string>>();

        // key -> filename without extension  value -> fullPath
        private readonly Dictionary<string, string> allAssetDic = new Dictionary<string, string>();

        public void Init()
        {
            if (Global.GameMode == GameMode.Editor)
            {
                ParseBundleConfig();
                GenerateAllAssetDic();
            }
            else
            {
                ParseFileIndex();
                ParseBundleDependencies();
            }
        }


        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="resName">资源名（不含后缀名）</param>
        /// <param name="callback">回调函数</param>
        public void LoadResByName(string resName, Action<Object> callback)
        {
            if (Global.GameMode == GameMode.Editor)
            {
                EditorLoadAssetByName(resName, callback);
            }
            else
            {
                StartCoroutine(LoadResByNameAsync(resName, callback));
            }
        }


        # region private

        private void ParseFileIndex()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Global.FileIndexConfigPath);
            var elements = xmlDoc.GetElementsByTagName("file");

            foreach (XmlElement element in elements)
            {
                // without extension
                var filename = element.GetAttribute("name");
                var bundleName = element.GetAttribute("bundle_name");

                fileIndexDic.Add(filename, bundleName);
            }
        }

        private void ParseBundleDependencies()
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Global.BundleDependencyConfigPath);

            var elements = xmlDoc.GetElementsByTagName("bundle");
            foreach (XmlElement element in elements)
            {
                var bundleName = element.GetAttribute("name");
                var dependencies = element.GetElementsByTagName("dependency");
                var dependencyList =
                    (from XmlElement dependency in dependencies select dependency.GetAttribute("name")).ToList();

                bundleDependencyDic.Add(bundleName, dependencyList);
            }
        }


        private IEnumerator LoadResByNameAsync(string resName, Action<Object> callback = null)
        {
            fileIndexDic.TryGetValue(resName.ToLower(), out var bundleName);
            if (string.IsNullOrEmpty(bundleName))
            {
                Debug.LogError($"试图加载不存在的资源: {resName}");
            }
            else
            {
                var bundlePath = CommonUtil.GetStandardPath(Path.Combine(Global.BundleOutputPath, bundleName));
                var bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
                yield return bundleRequest;

                var resRequest = bundleRequest.assetBundle.LoadAssetAsync(resName.ToLower());
                yield return resRequest;

                bundleDependencyDic.TryGetValue(bundleName, out var dependencies);
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        yield return LoadAssetBundleAsync(dependency);
                    }
                }

                callback?.Invoke(resRequest?.asset);
            }
        }

        private IEnumerator LoadAssetBundleAsync(string bundleName)
        {
            var bundlePath = CommonUtil.GetStandardPath(Path.Combine(Global.BundleOutputPath, bundleName));
            var bundleRequest = AssetBundle.LoadFromFileAsync(bundlePath);
            yield return bundleRequest;
        }

        private void ParseBundleConfig()
        {
            bundleConfigList.Clear();

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Global.BundlePathConfigPath);

            var elements = xmlDoc.GetElementsByTagName("item");
            foreach (XmlElement element in elements)
            {
                var path = element.GetAttribute("path");

                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogError("path为空");
                }
                else
                {
                    bundleConfigList.Add(path);
                }
            }
        }

        private void GenerateAllAssetDic()
        {
            allAssetDic.Clear();

            foreach (var path in bundleConfigList)
            {
                TraverseDirectory(path, allAssetDic);
            }
        }

        private void TraverseDirectory(string rootPath, IDictionary<string, string> sourceDic)
        {
            var files = Directory.GetFiles(rootPath);

            foreach (var fullPath in files)
            {
                if (fullPath.EndsWith(".meta"))
                    continue;

                var filename = Path.GetFileNameWithoutExtension(fullPath);
                if (sourceDic.ContainsKey(filename))
                {
                    var message = new[]
                    {
                        "包含同名资源!",
                        $"fileName: {filename}",
                        $"existing path: {sourceDic[filename]}",
                        $"incoming path: {fullPath}"
                    };
                    Debug.LogError(string.Join("\n", message));
                }
                else
                {
                    sourceDic.Add(filename, CommonUtil.GetStandardPath(fullPath));
                }
            }

            var subDirectories = Directory.GetDirectories(rootPath);

            foreach (var directory in subDirectories)
            {
                TraverseDirectory(directory, sourceDic);
            }
        }

        private void EditorLoadAssetByName(string resName, Action<Object> callback)
        {
#if UNITY_EDITOR

            allAssetDic.TryGetValue(resName, out var fullPath);
            if (string.IsNullOrEmpty(fullPath))
            {
                Debug.LogError($"不存在的资源：{resName}");
            }
            var obj = AssetDatabase.LoadAssetAtPath(CommonUtil.GetUnityPath(fullPath), typeof(Object));
            if (obj == null)
            {
                Debug.LogError($"不存在的资源路径：{fullPath}");
            }
            callback?.Invoke(obj);
#endif


        }

        #endregion

    }
}
