﻿using UnityEngine;

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

        // 可读写的文件夹路径
        public static readonly string PersistentDataPath = Application.persistentDataPath;

        // ab包下载地址
        public static readonly string ResourceUrl = "http://127.0.0.1/AssetBundles";
        
        public static readonly string BundleExtension = ".ab";

        public static GameMode GameMode = GameMode.Editor;
    }

    public enum GameMode
    {
        Editor,
        Bundle,
        Update
    }
}