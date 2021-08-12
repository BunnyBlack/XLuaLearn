using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;

namespace CommonCs.HotFixMgr
{
    public class ZipHelper
    {
        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="zipPath">压缩文件的路径</param>
        /// <param name="dirPath">输出路径</param>
        public static void Unzip(string zipPath, string dirPath)
        {
            try
            {
                if (!Directory.Exists(dirPath))
                    Directory.CreateDirectory(dirPath);

                var s = new ZipInputStream(File.OpenRead(zipPath));

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    var directoryName = Path.GetDirectoryName(theEntry.Name);
                    var fileName = Path.GetFileName(theEntry.Name);

                    if (directoryName != string.Empty)
                        Directory.CreateDirectory(dirPath + "/" + directoryName);

                    if (fileName == string.Empty)
                        continue;

                    var streamWriter = File.Create(dirPath + "/" + theEntry.Name);

                    var data = new byte[2048];
                    while (true)
                    {
                        var size = s.Read(data, 0, data.Length);
                        if (size > 0)
                        {
                            streamWriter.Write(data, 0, size);
                        }
                        else
                        {
                            break;
                        }
                    }

                    streamWriter.Close();
                }
                s.Close();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                throw;
            }
        }
    }
}
