using System.IO;
using CommonCs.Utils;
using UnityEngine;

namespace CommonCs
{
    public static class Global
    {
        // 项目根目录
        public static readonly string RootPath = Application.dataPath;

        // 打包配置输出路径
        public static readonly string BundlePathConfigPath = $"{RootPath}/Editor/AssetBundleTools/bundlePath.xml";

        // ab包输出路径
        public static readonly string BundleOutputPath = Application.streamingAssetsPath;

        // 全文件索引配置的文件名称
        public static readonly string FileIndexName = "fileIndex.xml";

        // 包依赖配置的文件名称
        public static readonly string BundleDependenciesName = "bundleDependencies.xml";

        // 文件索引配置文件的路径
        public static readonly string FileIndexConfigPath = $"{BundleOutputPath}/{FileIndexName}";

        // 包依赖配置文件的路径
        public static readonly string BundleDependencyConfigPath = $"{BundleOutputPath}/{BundleDependenciesName}";

        // 可读写的文件夹路径
        public static readonly string PersistentDataPath = Application.persistentDataPath;

        // ab包导出路径
        public static readonly string ExportBundlePath =
            $"{CommonUtil.GetStandardPath(Path.GetDirectoryName(RootPath))}/Output";

        // 需要上传服务器的文件夹地址
        public static readonly string ReleasePath = $"{CommonUtil.GetStandardPath(Path.GetDirectoryName(RootPath))}/Release";

        // 更新包下载地址
        public static readonly string ResourceServerAddress = "http://127.0.0.1/AssetBundles";

        // 服务器最新版本配置文件的名称
        public static readonly string ServerVersionConfigName = "version_config.txt";

        // ab包后缀名
        public static readonly string BundleExtension = ".ab";

        // 更新模式（暂时不完善）
        public static GameMode GameMode = GameMode.Editor;

        // 打包用版本号 本地的真实版本号是由持久化目录中的fileIndex的版本号决定
        public static readonly int BuildVersion = 3;
    }

    public enum GameMode
    {
        Editor,
        Bundle,
        Update
    }
}
