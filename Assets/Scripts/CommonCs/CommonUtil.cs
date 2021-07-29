using System;
using UnityEngine;

namespace CommonCs
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
                return bundleName;
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

            return bundleName;
        }
    }
}
