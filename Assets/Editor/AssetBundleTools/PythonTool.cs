using System.Diagnostics;
using System.IO;
using CommonCs;
using CommonCs.Utils;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Editor.AssetBundleTools
{
    public static class PythonTool
    {
        private static readonly string ToolPath =
            CommonUtil.GetStandardPath(Path.GetDirectoryName(Global.RootPath)) + "/PythonTools/tools.py";

        private static string _pythonPath;

        [MenuItem("Python工具/设置Python位置/手动设置Python.exe位置")]
        public static void SetPythonPathManually()
        {
            var path = EditorUtility.OpenFilePanel("选择Python.exe文件", "C:", "exe");

            Debug.Log($"Python Path: {_pythonPath}");
            if (!string.IsNullOrEmpty(path))
            {
                _pythonPath = path;
            }
        }

        [MenuItem("Python工具/设置Python位置/自动设置Python.exe位置")]
        public static void WherePython()
        {
            DoWherePython();
        }

        [MenuItem("Python工具/导出AB包")]
        public static void GenerateExportBundles()
        {
            var command = $"\"{ToolPath}\" \"generate_version_bundles('{Global.BundleOutputPath}', '{Global.ExportBundlePath}', '{Global.ReleasePath}')\"";
            DoPythonFunction(command);
        }
        

        # region private

        private static void DoWherePython()
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
            p.StandardInput.WriteLine("Where Python&exit");
            var outputInfo = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();

            // 按换行符分开
            var outputStrList = outputInfo.Split('\n');
            // 找到结尾是 python.exe 的字符串作为 python 的路径
            foreach (var outputStr in outputStrList)
            {

                if (!outputStr.Trim().EndsWith("python.exe"))
                    continue;

                // 别忘了去除头尾的空格，否则 process 找不到文件名
                _pythonPath = outputStr.Trim();
                Debug.Log($"Python Path: {_pythonPath}");
                break;
            }
            if (string.IsNullOrEmpty(_pythonPath))
            {
                Debug.LogError("未找到Python路径！");
            }
        }
        
        /// <summary>
        /// 传入命令行语句来执行Python的方法
        /// </summary>
        /// <param name="command"></param>
        private static void DoPythonFunction(string command)
        {
            if (string.IsNullOrEmpty(_pythonPath))
            {
                DoWherePython();
            }

            var p = new Process
            {
                StartInfo =
                {
                    //设置要启动的应用程序
                    FileName = _pythonPath,
                    // 是否使用操作系统shell启动
                    UseShellExecute = false,
                    // 接受来自调用程序的输入信息
                    RedirectStandardInput = true,
                    // 输出信息
                    RedirectStandardOutput = true,
                    // 输出错误
                    RedirectStandardError = true,
                    // 不显示程序窗口
                    CreateNoWindow = false,
                    Arguments = command
                }
            };
            //启动程序
            p.Start();
            var outputInfo = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            p.Close();

            Debug.Log(outputInfo);
        }

        # endregion

    }
}
