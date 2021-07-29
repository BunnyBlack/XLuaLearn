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

        [MenuItem("test/TestBuild")]
        private static void TestBuild()
        {
            BuildPipeline.BuildAssetBundles(Global.BundleOutputPath,
                BuildAssetBundleOptions.ChunkBasedCompression,
                EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
            Debug.Log("打ab包");
        }
    }
}
