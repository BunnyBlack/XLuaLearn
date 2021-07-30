using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CommonCs
{
    public class ResLoader : MonoBehaviour
    {
        // key -> filename without extension  value -> bundle name
        private readonly Dictionary<string, string> fileIndexDic = new Dictionary<string, string>();

        // key -> bundleName  value -> list of dependent bundle      key depend on value
        private readonly Dictionary<string, List<string>> bundleDependencyDic = new Dictionary<string, List<string>>();
        
        public void Init()
        {
            ParseFileIndex();
            ParseBundleDependencies();
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="resName">资源名（不含后缀名）</param>
        /// <param name="callback">回调函数</param>
        public void LoadResByName(string resName, Action<Object> callback)
        {
            StartCoroutine(LoadResByNameAsync(resName, callback));
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
            fileIndexDic.TryGetValue(resName, out var bundleName);
            if (string.IsNullOrEmpty(bundleName))
            {
                Debug.LogError($"试图加载不存在的资源！{resName}");
                yield break;
            }
            else
            {
                

            }
        }
        

        #endregion

    }
}
