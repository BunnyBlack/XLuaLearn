using System;
using System.IO;
using UnityEngine;

namespace CommonCs.Utils
{
    public class FileUtil
    {
        /// <summary>
        /// 将文件数据写入到本地的文件中
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <param name="filename">写入文件的名称（带后缀）</param>
        /// <param name="data">写入数据的字节流</param>
        public static void WriteFileToPath(string dirPath, string filename, byte[] data)
        {
            var fullPath = CommonUtil.GetStandardPath(Path.Combine(dirPath, filename));
            // 由于bundle名称可能还带了路径，所以我们要再解一次directoryName
            var finalDirName = CommonUtil.GetStandardPath(Path.GetDirectoryName(fullPath));
            if (!Directory.Exists(finalDirName))
            {
                Directory.CreateDirectory(finalDirName);
            }
            
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            try
            {
                using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(data, 0, data.Length);
                    fs.Close();
                }
            }
            catch (IOException e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
