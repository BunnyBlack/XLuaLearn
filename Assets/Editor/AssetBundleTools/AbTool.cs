using System.IO;
using System.Xml;
using UnityEditor;
using UnityEngine;

namespace Editor.AssetBundleTools
{
    public class AbTool : UnityEditor.Editor
    {
        [MenuItem("Tools/打包工具/生成打包路径xml")]
        private static void GenerateBundlePathXml()
        {
            var xmlPath = Application.dataPath + "/Editor/AssetBundleTools/bundlePath.xml";
            var xmlDoc = new XmlDocument();
            var root = xmlDoc.CreateElement("bundle_path");
            xmlDoc.AppendChild(root);

            var audioPath = xmlDoc.CreateElement("item");
            audioPath.SetAttribute("path", Application.dataPath + "/Data/Audio");
            audioPath.SetAttribute("bundle_type", "dir");
            root.AppendChild(audioPath);

            var effectPath = xmlDoc.CreateElement("item");
            effectPath.SetAttribute("path", Application.dataPath + "/Data/Effect");
            effectPath.SetAttribute("bundle_type", "dir");
            root.AppendChild(effectPath);

            xmlDoc.Save(xmlPath);
            AssetDatabase.Refresh();
        }


        [MenuItem("Tools/打包工具/打ab包")]
        private static void BuildAssetBundles()
        {
            CreateStreamingDir();

            var assetBundleBuild = new AssetBundleBuild[] { };
            BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath,
                assetBundleBuild,
                BuildAssetBundleOptions.ChunkBasedCompression,
                EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
            Debug.Log("打ab包");


        }

        # region tools

        private static void CreateStreamingDir()
        {
            if (Directory.Exists(Application.streamingAssetsPath))
                return;

            Directory.CreateDirectory(Application.streamingAssetsPath);
            AssetDatabase.Refresh();
        }

        # endregion

    }
}
