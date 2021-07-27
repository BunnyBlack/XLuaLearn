using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class Test : UnityEditor.Editor
    {
        [MenuItem("test/test", false, 1)]
        private static void Test1()
        {
            var assetBundleBuildList = new List<AssetBundleBuild>();
            var assetBundleBuild = new AssetBundleBuild { assetNames = new[] { "Assets\\Data\\UI" }, assetBundleName = "UI" };
            assetBundleBuildList.Add(assetBundleBuild);

            BuildPipeline.BuildAssetBundles(Global.BundleOutputPath,
                assetBundleBuildList.ToArray(),
                BuildAssetBundleOptions.ChunkBasedCompression,
                EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
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
    }
}
