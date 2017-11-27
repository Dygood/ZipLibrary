using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;

namespace YuWan
{
    namespace ZipLibrary
    {
        /// <summary>
        /// 压缩解压操作类，使用的是SharpZipLib
        /// </summary>
        public static class DCompressV1
        {
            private static object operateLock = new object();
            /// <summary>
            /// 压缩文件
            /// </summary>
            /// <param name="srcFile">要压缩的文件路径</param>
            /// <param name="destFile">生成的压缩文件路径</param>
            public static void CompressFile(string srcFile, string destFile)
            {
                lock (operateLock)
                {
                    if (string.IsNullOrEmpty(srcFile) || string.IsNullOrEmpty(destFile))
                    {
                        throw new ArgumentException("参数错误");
                    }
                    FileStream fileStreamIn = new FileStream(srcFile, FileMode.Open, FileAccess.Read);
                    FileStream fileStreamOut = new FileStream(destFile, FileMode.Create, FileAccess.Write);
                    ZipOutputStream zipOutStream = new ZipOutputStream(fileStreamOut);
                    //zipOutStream.SetLevel(6);   //设置压缩等级，默认为6
                    byte[] buffer = new byte[4096];
                    ZipEntry entry = new ZipEntry(Path.GetFileName(srcFile));
                    zipOutStream.PutNextEntry(entry);
                    int size;
                    do
                    {
                        size = fileStreamIn.Read(buffer, 0, buffer.Length);
                        zipOutStream.Write(buffer, 0, size);
                    } while (size > 0);
                    zipOutStream.Dispose();
                    fileStreamOut.Dispose();
                    fileStreamIn.Dispose();
                }
            }
            /// <summary>
            /// 压缩多个文件
            /// </summary>
            /// <param name="srcFiles">多个文件路径</param>
            /// <param name="destFile">压缩文件的路径</param>
            public static void ZipFiles(string[] srcFiles, string destFile)
            {
                if (srcFiles == null || string.IsNullOrEmpty(destFile))
                {
                    throw new ArgumentException("参数错误");
                }
                using (ZipFile zip = ZipFile.Create(destFile))
                {
                    zip.BeginUpdate();
                    //add file
                    foreach (string filePath in srcFiles)
                    {
                        zip.Add(filePath);
                    }
                    zip.CommitUpdate();
                }
            }
            /// <summary>
            /// 压缩目录
            /// </summary>
            /// <param name="dir">目录路径</param>
            /// <param name="destFile">压缩文件路径</param>
            public static void ZipDir(string dir, string destFile)
            {
                if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(destFile))
                {
                    throw new ArgumentException("参数错误");
                }
                string[] files = Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories);
                ZipFiles(files, destFile);
            }
            /// <summary>
            /// 列表压缩文件里的所有文件
            /// </summary>
            /// <param name="zipPath">压缩文件路径</param>
            /// <returns></returns>
            public static List<string> GetFileList(string zipPath)
            {
                List<string> files = new List<string>();
                if (string.IsNullOrEmpty(zipPath))
                {
                    throw new ArgumentException("参数错误");
                }
                using (ZipFile zip = new ZipFile(zipPath))
                {
                    string list = string.Empty;
                    foreach (ZipEntry entry in zip)
                    {
                        if (entry.IsFile)
                        {
                            files.Add(entry.Name);
                        }
                    }
                }
                return files;
            }
            /// <summary>
            /// 删除zip文件中的某个文件
            /// </summary>
            /// <param name="zipPath">压缩文件路径</param>
            /// <param name="files">要删除的某个文件</param>
            public static void DeleteFileFromZip(string zipPath, string[] files)
            {
                if (string.IsNullOrEmpty(zipPath) || files == null)
                {
                    throw new ArgumentException("参数错误");
                }
                using (ZipFile zip = new ZipFile(zipPath))
                {
                    zip.BeginUpdate();
                    foreach (string f in files)
                    {
                        zip.Delete(f);
                    }
                    zip.CommitUpdate();
                }
            }
            /// <summary>
            /// 解压文件
            /// </summary>
            /// <param name="zipPath">要解压的文件</param>
            /// <param name="outputDir">解压后放置的目录</param>
            public static void UnZipFile(string zipPath, string outputDir)
            {
                (new FastZip()).ExtractZip(zipPath, outputDir, "");
            }
            /// <summary>
            /// 解压文件
            /// </summary>
            /// <param name="srcFile">压缩文件路径</param>
            /// <param name="destDir">解压后文件夹的路径</param>
            public static void Decompress(string srcFile, string destDir)
            {
                lock (operateLock)
                {
                    FileStream fileStreamIn = new FileStream(srcFile, FileMode.Open, FileAccess.Read);
                    ZipInputStream zipInStream = new ZipInputStream(fileStreamIn);
                    if (!Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }
                    ZipEntry entry;
                    while ((entry = zipInStream.GetNextEntry()) != null)
                    {
                        FileStream fileStreamOut = new FileStream(destDir + @"\" + entry.Name, FileMode.Create, FileAccess.Write);
                        int size;
                        byte[] buffer = new byte[4096];
                        do
                        {
                            size = zipInStream.Read(buffer, 0, buffer.Length);
                            fileStreamOut.Write(buffer, 0, size);
                        } while (size > 0);
                        fileStreamOut.Dispose();
                    }
                    zipInStream.Dispose();
                    fileStreamIn.Dispose();
                }
            }
        }
    }
}