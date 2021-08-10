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
            p.StandardInput.WriteLine($"start {Global.PersistentDataPath}&exit");
            p.WaitForExit();
            p.Close();
        }

        [MenuItem("test/输出PersistentDataPath")]
        private static void PrintPersistentDataPath()
        {
            Debug.Log(Global.PersistentDataPath);
        }
    }
}
