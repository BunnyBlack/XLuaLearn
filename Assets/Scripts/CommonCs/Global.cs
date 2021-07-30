using UnityEngine;

namespace CommonCs
{
    public static class Global
    {
        // 项目根目录
        public static readonly string RootPath = Application.dataPath;

        // 打包配置输出路径
        public static readonly string BundlePathConfigPath = RootPath + "/Editor/AssetBundleTools/bundlePath.xml";

        // ab包输出路径
        public static readonly string BundleOutputPath = Application.streamingAssetsPath;

        // 文件索引配置文件的路径
        public static readonly string FileIndexConfigPath = BundleOutputPath + "/fileIndex.xml";

        // 包依赖配置文件的路径
        public static readonly string BundleDependencyConfigPath = BundleOutputPath + "/bundleDependency.xml";
    }
}
