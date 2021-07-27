using UnityEngine;

namespace Editor
{
    public class Global : UnityEditor.Editor
    {
        // 项目根目录
        public static readonly string RootPath = Application.dataPath;
        // 打包配置输出路径
        public static readonly string BundlePathConfigPath = RootPath + "/Editor/AssetBundleTools/bundlePath.xml";
        // ab包输出路径
        public static readonly string BundleOutputPath = Application.streamingAssetsPath;
        
        
    }
}
