using System;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class EditorUtil : UnityEditor.Editor
    {
        /// <summary>
        /// 通过绝对路径获取相对 Assets 文件夹的相对路径
        /// </summary>
        /// <param name="absolutePath">绝对路径</param>
        /// <returns>相对路径</returns>
        public static string GetUnityPath(string absolutePath)
        {
            if (absolutePath.IndexOf("Assets", StringComparison.Ordinal) != -1)
            {
                return absolutePath.Substring(absolutePath.IndexOf("Assets", StringComparison.Ordinal));
            }
            Debug.LogError($"路径未包含Assets: {absolutePath}");
            return absolutePath;
        }
    }
}
