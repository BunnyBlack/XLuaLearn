using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Editor.AssetBundleTools
{
    public class AbTool : UnityEditor.Editor
    {
        private List<string> fileTypeBundleList = new List<string>();
        private List<string> dirTypeBundleList = new List<string>();
        
        
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
            SetBundleXmlAttribute(uiPath,Global.RootPath + "/Data/UI", "file");
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

            GetAssetBundleBuildList(assetBundleBuildList);
            
            BuildPipeline.BuildAssetBundles(Global.BundleOutputPath,
                assetBundleBuildList.ToArray(),
                BuildAssetBundleOptions.ChunkBasedCompression,
                EditorUserBuildSettings.activeBuildTarget);
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
        
        private static void GetAssetBundleBuildList(List<AssetBundleBuild> assetBundleBuildList)
        {
            
        }

        # endregion

    }
}
