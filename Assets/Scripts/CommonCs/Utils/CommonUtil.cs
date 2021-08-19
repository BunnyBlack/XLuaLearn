using System;
using UnityEngine;

namespace CommonCs.Utils
{
    public static class CommonUtil
    {
        /// <summary>
        /// 通过绝对路径获取相对 Assets 文件夹的相对路径
        /// </summary>
        /// <param name="absolutePath">绝对路径</param>
        /// <returns>相对Assets路径</returns>
        public static string GetUnityPath(string absolutePath)
        {
            if (absolutePath.IndexOf("Assets", StringComparison.Ordinal) != -1)
            {
                return absolutePath.Substring(absolutePath.IndexOf("Assets", StringComparison.Ordinal));
            }
            Debug.LogError($"路径未包含Assets: {absolutePath}");
            return absolutePath;
        }

        /// <summary>
        /// 从Assets的子目录开始的路径（不含Assets）
        /// </summary>
        /// <param name="path">绝对路径</param>
        /// <returns>包名</returns>
        public static string GetBundleName(string path)
        {
            var bundleName = path;

            var startIndex = path.IndexOf("Assets", StringComparison.Ordinal);
            if (startIndex == -1)
            {
                return bundleName.ToLower() + Global.BundleExtension;
            }

            var lastIndex = path.IndexOf(".", StringComparison.Ordinal);
            if (lastIndex == -1)
            {
                // 没有后缀说明是文件夹
                bundleName = path.Substring(startIndex + 7);
            }
            else
            {
                var len = lastIndex - (startIndex + 7);
                bundleName = path.Substring(startIndex + 7, len);
            }

            return bundleName.ToLower() + Global.BundleExtension;
        }

        /// <summary>
        /// 获取标准路径
        /// </summary>
        /// <param name="path">任意路径</param>
        /// <returns>标准路径</returns>
        public static string GetStandardPath(string path)
        {
            return string.IsNullOrEmpty(path) ? string.Empty : path.Trim().Replace("\\", "/");
        }

        /// <summary>
        /// 获取相对于StreamingAssets的路径
        /// </summary>
        /// <param name="path">任意路径</param>
        /// <returns>相对于StreamingAssets的路径</returns>
        public static string GetRelativePathToStreamingAssets(string path)
        {
            var index = path.IndexOf("StreamingAssets", StringComparison.Ordinal);
            if (index != -1)
                return path.Substring(index + 16);

            Debug.LogError("不在StreamingAssets路径下！");
            return path;
        }

        public static string GetLuaNameWithNameSpace(string path)
        {
            var scriptName = path;

            var startIndex = path.IndexOf("Scripts", StringComparison.Ordinal);
            if (startIndex == -1)
            {
                return scriptName;
            }

            var lastIndex = path.IndexOf(".", StringComparison.Ordinal);
            if (lastIndex == -1)
            {
                // lua文件可能会没有后缀
                scriptName = path.Substring(startIndex + 8);
            }
            else
            {
                var len = lastIndex - (startIndex + 8);
                scriptName = path.Substring(startIndex + 8, len);
            }

            return scriptName;
        }
    }
}
