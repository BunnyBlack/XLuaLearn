using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using CommonCs;
using UnityEditor;
using UnityEngine;

namespace Editor.AssetBundleTools
{
    public class AbTool : UnityEditor.Editor
    {
        // 需要打bundle的路径列表
        private static readonly List<string> FileTypeConfigList = new List<string>();
        private static readonly List<string> DirTypeConfigList = new List<string>();

        // key -> filename without extension  value -> file fullPath
        private static readonly Dictionary<string, string> FileTypePathDic = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> DirTypePathDic = new Dictionary<string, string>();

        // key -> filename without extension  value -> bundle name
        private static readonly Dictionary<string, string> PackagedBundleDic = new Dictionary<string, string>();
        // key -> bundleName  value -> list of dependent bundle      key depend on value
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
            GenerateFileIndexConfig();
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
                        "包含同名资源!",
                        $"fileName: {filename}",
                        $"existing path: {sourceDic[filename]}",
                        $"incoming path: {fullPath}"
                    };
                    Debug.LogError(string.Join("\n", message));
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
            PackagedBundleDic.Clear();
            
            // 先生成按文件夹的打包
            foreach (var path in DirTypeConfigList)
            {
                var assetBundleBuild = new AssetBundleBuild();
                assetBundleBuild.assetBundleName = CommonUtil.GetBundleName(path);
                assetBundleBuild.assetNames = new[] { CommonUtil.GetUnityPath(path) };
                assetBundleBuildList.Add(assetBundleBuild);
            }
            
            // 构建按文件夹生成的文件与包对应关系的字典
            foreach (var pair in DirTypePathDic)
            {
                var filename = pair.Key;
                var filePath = pair.Value;

                var bundleName = CommonUtil.GetBundleName(filePath);
                
                PackagedBundleDic.Add(filename, bundleName);
            }

            // 按单个文件文件打包 需要获取依赖并剔除冗余
            foreach (var pair in FileTypePathDic)
            {
                var filename = pair.Key;
                var filePath = pair.Value;

                var relativePath = CommonUtil.GetUnityPath(filePath);
                var dependencies = AssetDatabase.GetDependencies(relativePath);

                var bundleName = CommonUtil.GetBundleName(relativePath);

                var assetBundleBuild = new AssetBundleBuild();
                assetBundleBuild.assetBundleName = bundleName;

                var fileList = new List<string>();
                
                // 依赖已经包含了该资源本身
                foreach (var dependency in dependencies)
                {
                    if(dependency.EndsWith(".cs"))
                        continue;
                    var dependencyName = Path.GetFileNameWithoutExtension(dependency);
                    
                    // 如果依赖的文件已经打过包了，就不再打进自己的包，而是建立包依赖
                    if (PackagedBundleDic.ContainsKey(dependencyName))
                    {
                        if (PackagedBundleDic[dependencyName] == bundleName)
                            continue;

                        if (BundleDependencyDic.ContainsKey(dependencyName))
                        {
                            var dependencyList = BundleDependencyDic[dependencyName];
                            dependencyList.Add(PackagedBundleDic[dependencyName]);
                        }
                        else
                        {
                            var dependencyList = new List<string> { PackagedBundleDic[dependencyName] };
                            BundleDependencyDic.Add(dependencyName, dependencyList);
                        }
                    }
                    else
                    {
                        fileList.Add(CommonUtil.GetUnityPath(dependency));
                        PackagedBundleDic.Add(dependencyName, bundleName);
                    }
                }
                assetBundleBuild.assetNames = fileList.ToArray();
                assetBundleBuildList.Add(assetBundleBuild);
            }
        }

        private static void GenerateBundleDependencyConfig()
        {
            // 建立包与依赖包关系配置
            var xmlPath = Global.BundleOutputPath + "/bundleDependency.xml";
            var xmlDoc = new XmlDocument();
            var root = xmlDoc.CreateElement("bundle_dependency");
            xmlDoc.AppendChild(root);
            
            foreach (var pair in BundleDependencyDic)
            {
                var bundleName = pair.Key;
                var dependencies = pair.Value;
                var bundleNode = xmlDoc.CreateElement("bundle");
                bundleNode.SetAttribute("name", bundleName);
                root.AppendChild(bundleNode);

                foreach (var dependencyName in dependencies)
                {
                    var dependencyNode = xmlDoc.CreateElement("dependency");
                    dependencyNode.SetAttribute("name", dependencyName);
                    bundleNode.AppendChild(dependencyNode);
                }
            }
            
            xmlDoc.Save(xmlPath);
        }
        
        
        private static void GenerateFileIndexConfig()
        {
            // 建立文件与包名之间的关联配置
            var xmlPath = Global.BundleOutputPath + "/fileIndex.xml";
            var xmlDoc = new XmlDocument();
            var root = xmlDoc.CreateElement("file_index");
            xmlDoc.AppendChild(root);

            foreach (var pair in DirTypePathDic)
            {
                var filename = pair.Key;
                var fullPath = pair.Value;

                var fileNode = xmlDoc.CreateElement("file");
                fileNode.SetAttribute("name", filename);
                fileNode.SetAttribute("bundle_name", fullPath);
                root.AppendChild(fileNode);
            }
            
            foreach (var pair in FileTypePathDic)
            {
                var filename = pair.Key;
                var fullPath = pair.Value;

                var fileNode = xmlDoc.CreateElement("file");
                fileNode.SetAttribute("name", filename);
                fileNode.SetAttribute("bundle_name", fullPath);
                root.AppendChild(fileNode);
            }
            
            xmlDoc.Save(xmlPath);
        }

        # endregion

    }
}
