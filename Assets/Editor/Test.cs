using System;
using System.Diagnostics;
using System.IO;
using CommonCs;
using CommonCs.Utils;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Editor
{
    public class Test : UnityEditor.Editor
    {
        [MenuItem("test/test", false, 1)]
        private static void Test1()
        {
            const string path = @"D:\Unity Project\XLuaLearn\Assets\Scripts\CommonCs\Global.cs";

            var sub = path.Substring(0, path.IndexOf(".", StringComparison.Ordinal));
            Debug.Log(sub);
        }

        [MenuItem("test/清空StreamingAssets文件夹", false, 100)]
        private static void ClearStreaming()
        {
            if (Directory.Exists(Global.BundleOutputPath))
            {
                Directory.Delete(Global.BundleOutputPath, true);
            }

            Directory.CreateDirectory(Global.BundleOutputPath);
            AssetDatabase.Refresh();
        }

        [MenuItem("test/打开PersistentDataPath",false,200)]
        private static void OpenPersistentDataPath()
        {
            var p = new Process
            {
                StartInfo =
                {
                    //设置要启动的应用程序
                    FileName = "cmd.exe",
                    // 是否使用操作系统shell启动
                    UseShellExecute = false,
                    // 接受来自调用程序的输入信息
                    RedirectStandardInput = true,
                    // 输出信息
                    RedirectStandardOutput = true,
                    // 输出错误
                    RedirectStandardError = true,
                    // 不显示程序窗口
                    CreateNoWindow = true
                }
            };
            //启动程序
            p.Start();
            p.StandardInput.WriteLine($"start \"\" \"{Global.PersistentDataPath}\"&exit");
            p.WaitForExit();
            p.Close();
        }

        [MenuItem("test/输出PersistentDataPath", false, 201)]
        private static void PrintPersistentDataPath()
        {
            Debug.Log(Global.PersistentDataPath);
        }

        [MenuItem("test/打开输出目录")]
        private static void OpenOutputDir()
        {
            var p = new Process
            {
                StartInfo =
                {
                    //设置要启动的应用程序
                    FileName = "cmd.exe",
                    // 是否使用操作系统shell启动
                    UseShellExecute = false,
                    // 接受来自调用程序的输入信息
                    RedirectStandardInput = true,
                    // 输出信息
                    RedirectStandardOutput = true,
                    // 输出错误
                    RedirectStandardError = true,
                    // 不显示程序窗口
                    CreateNoWindow = true
                }
            };
            //启动程序
            p.Start();
            var outputPath = Global.ExportBundlePath;
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            p.StandardInput.WriteLine($"start \"\" \"{outputPath}\"&exit");
            p.WaitForExit();
            p.Close();
        }
        
        [MenuItem("test/打开Release目录")]
        private static void OpenReleaseDir()
        {
            var p = new Process
            {
                StartInfo =
                {
                    //设置要启动的应用程序
                    FileName = "cmd.exe",
                    // 是否使用操作系统shell启动
                    UseShellExecute = false,
                    // 接受来自调用程序的输入信息
                    RedirectStandardInput = true,
                    // 输出信息
                    RedirectStandardOutput = true,
                    // 输出错误
                    RedirectStandardError = true,
                    // 不显示程序窗口
                    CreateNoWindow = true
                }
            };
            //启动程序
            p.Start();
            var outputPath = Global.ReleasePath;
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            p.StandardInput.WriteLine($"start \"\" \"{outputPath}\"&exit");
            p.WaitForExit();
            p.Close();
        }
        
        [MenuItem("test/打开LuaScripts目录")]
        private static void OpenLuaScriptsDir()
        {
            var p = new Process
            {
                StartInfo =
                {
                    //设置要启动的应用程序
                    FileName = "cmd.exe",
                    // 是否使用操作系统shell启动
                    UseShellExecute = false,
                    // 接受来自调用程序的输入信息
                    RedirectStandardInput = true,
                    // 输出信息
                    RedirectStandardOutput = true,
                    // 输出错误
                    RedirectStandardError = true,
                    // 不显示程序窗口
                    CreateNoWindow = true
                }
            };
            //启动程序
            p.Start();
            var outputPath = Global.ExportLuaPath;
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            p.StandardInput.WriteLine($"start \"\" \"{outputPath}\"&exit");
            p.WaitForExit();
            p.Close();
        }
        
        
        
        
    }
}
