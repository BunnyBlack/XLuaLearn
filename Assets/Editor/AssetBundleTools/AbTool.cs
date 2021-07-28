using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Editor.AssetBundleTools
{
    public class AbTool : UnityEditor.Editor
    {
        // 需要打bundle的路径列表
        private static readonly List<string> FileTypeConfigList = new List<string>();
        private static readonly List<string> DirTypeConfigList = new List<string>();

        // key -> filename without extension  value -> file path
        private static readonly Dictionary<string, string> FileTypePathDic = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> DirTypePathDic = new Dictionary<string, string>();

        // key -> filename without extension  value -> bundle name
        private static readonly Dictionary<string, List<string>> BundleDependencyDic = new Dictionary<string, List<string>>();
        
        [MenuItem("Tools/打包工具/生成打包配置xml")]
        private static void CreateBundlePathConfig()
        {
            var xmlPath = Global.BundlePathConfigPath;
            var xmlDoc = new XmlDocument();
            var root = xmlDoc.CreateElement("bundle_path");
            xmlDoc.AppendChild(root);

            var audioPath = xmlDoc.CreateElement("item");
            SetBundleXmlAttribute(audioPath, Global.RootPath + "/Data/Audio", "dir");
            root.AppendChild(audioPath);

            var effectPath = xmlDoc.CreateElement("item");
            SetBundleXmlAttribute(effectPath, Global.RootPath + "/Data/Effect", "dir");
            root.AppendChild(effectPath);

            var modelPath = xmlDoc.CreateElement("item");
            SetBundleXmlAttribute(modelPath, Global.RootPath + "/Data/Model", "file");
            root.AppendChild(modelPath);

            var uiPath = xmlDoc.CreateElement("item");
            SetBundleXmlAttribute(uiPath, Global.RootPath + "/Data/UI", "file");
            root.AppendChild(uiPath);

            var scenePath = xmlDoc.CreateElement("item");
            SetBundleXmlAttribute(scenePath, Global.RootPath + "/Scenes", "dir");
            root.AppendChild(scenePath);

            // var commonCsPath = xmlDoc.CreateElement("item");
            // SetBundleXmlAttribute(commonCsPath, Global.RootPath + "/Scripts/CommonCs", "dir");
            // root.AppendChild(commonCsPath);
            //
            // var commonLuaPath = xmlDoc.CreateElement("item");
            // SetBundleXmlAttribute(commonLuaPath, Global.RootPath + "/Scripts/CommonLua", "dir");
            // root.AppendChild(commonLuaPath);
            //
            // var sourceCsPath = xmlDoc.CreateElement("item");
            // SetBundleXmlAttribute(sourceCsPath, Global.RootPath + "/Scripts/SourceCs", "dir");
            // root.AppendChild(sourceCsPath);
            //
            // var sourceLuaPath = xmlDoc.CreateElement("item");
            // SetBundleXmlAttribute(sourceLuaPath, Global.RootPath + "/Scripts/SourceLua", "dir");
            // root.AppendChild(sourceLuaPath);

            xmlDoc.Save(xmlPath);
            AssetDatabase.Refresh();
            Debug.Log($"生成打包配置xml: {xmlPath}");
        }


        [MenuItem("Tools/打包工具/打ab包")]
        private static void BuildAssetBundles()
        {
            CreateStreamingDir();

            var assetBundleBuildList = new List<AssetBundleBuild>();

            ParseBundleConfig();
            CreateFilePathDicReferringToConfig();
            GetBundleBuildConfig(assetBundleBuildList);

            BuildPipeline.BuildAssetBundles(Global.BundleOutputPath,
                assetBundleBuildList.ToArray(),
                BuildAssetBundleOptions.ChunkBasedCompression,
                EditorUserBuildSettings.activeBuildTarget);

            GenerateBundleDependencyConfig();
            AssetDatabase.Refresh();
            Debug.Log("打ab包");
        }




        # region tools

        private static void CreateStreamingDir()
        {
            if (Directory.Exists(Global.BundleOutputPath))
            {
                Directory.Delete(Global.BundleOutputPath, true);
            }

            Directory.CreateDirectory(Global.BundleOutputPath);
            AssetDatabase.Refresh();
        }

        private static void SetBundleXmlAttribute(XmlElement element, string path, string type)
        {
            element.SetAttribute("path", path);
            element.SetAttribute("bundle_type", type);
        }

        private static void ParseBundleConfig()
        {
            DirTypeConfigList.Clear();
            FileTypeConfigList.Clear();

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(Global.BundlePathConfigPath);

            var elements = xmlDoc.GetElementsByTagName("item");
            foreach (XmlElement element in elements)
            {
                var type = element.GetAttribute("bundle_type");
                var path = element.GetAttribute("path");

                // 根据类型将需要处理的路径放入待处理列表中
                switch (type)
                {
                    case "dir" when !string.IsNullOrEmpty(path):
                        DirTypeConfigList.Add(path);
                        break;
                    case "dir":
                        Debug.Log("path为空");
                        break;
                    case "file" when !string.IsNullOrEmpty(path):
                        FileTypeConfigList.Add(path);
                        break;
                    case "file":
                        Debug.Log("path为空");
                        break;
                    default:
                        Debug.Log($"错误的打包类型：{type}");
                        break;
                }
            }
        }

        private static void CreateFilePathDicReferringToConfig()
        {
            FileTypePathDic.Clear();
            DirTypePathDic.Clear();

            // 按文件夹打包的类型也做全文件遍历，用于剔除冗余用
            foreach (var path in DirTypeConfigList)
            {
                TraverseDirectory(path, DirTypePathDic);
            }

            foreach (var path in FileTypeConfigList)
            {
                TraverseDirectory(path, FileTypePathDic);
            }
        }

        private static void TraverseDirectory(string rootPath, IDictionary<string, string> sourceDic)
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
                        "已包含同名资源!",
                        $"fileName: {filename}",
                        $"existing path: {sourceDic[filename]}",
                        $"incoming path: {fullPath}"
                    };
                    Debug.Log(string.Join("\n", message));
                }
                else
                {
                    sourceDic.Add(filename, fullPath);
                }
            }

            var subDirectories = Directory.GetDirectories(rootPath);

            foreach (var directory in subDirectories)
            {
                TraverseDirectory(directory, sourceDic);
            }
        }

        private static void GetBundleBuildConfig(ICollection<AssetBundleBuild> assetBundleBuildList)
        {
            BundleDependencyDic.Clear();
            
            // 先生成按文件夹的打包
            foreach (var path in DirTypeConfigList)
            {
                var assetBundleBuild = new AssetBundleBuild();
                assetBundleBuild.assetBundleName = EditorUtil.GetBundleName(path);
                assetBundleBuild.assetNames = new[] { path };
                assetBundleBuildList.Add(assetBundleBuild);
            }
            
            // 构建按文件夹生成的包的依赖字典
            foreach (var pair in DirTypePathDic)
            {
                var filename = pair.Key;
                var filePath = pair.Value;

                var bundleName = EditorUtil.GetBundleName(filePath);
                
            }

            // 按单个文件文件打包 需要获取依赖并剔除冗余
            foreach (var pair in FileTypePathDic)
            {
                var filename = pair.Key;
                var filePath = pair.Value;

                var relativePath = EditorUtil.GetUnityPath(filePath);
                var dependencies = AssetDatabase.GetDependencies(relativePath);

                // 依赖已经包含了该资源本身
                foreach (var dependency in dependencies)
                {
                    var dependencyName = Path.GetFileNameWithoutExtension(dependency);
                    
                }
            }
        }

        private static void GenerateBundleDependencyConfig()
        {
            
        }

        # endregion

    }
}
