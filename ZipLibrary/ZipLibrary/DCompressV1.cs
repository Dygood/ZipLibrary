using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace YuWan
{
    namespace ZipLibrary
    {
        /// <summary>
        /// 压缩解压操作类，使用的是SharpZipLib
        /// </summary>
        public static class DCompressV1
        {
            private static object OperateLock { get; } = new object();

            /// <summary>
            /// 压缩文件
            /// </summary>
            /// <param name="srcFile">要压缩的文件路径</param>
            /// <param name="destFile">生成的压缩文件路径</param>
            /// <exception cref="ArgumentException"></exception>
            public static void CompressFile(string srcFile, string destFile)
            {
                lock (OperateLock)
                {
                    if (string.IsNullOrEmpty(srcFile) || string.IsNullOrEmpty(destFile))
                        throw new ArgumentException("参数错误");
                    using (var fileStreamIn = new FileStream(srcFile, FileMode.Open, FileAccess.Read))
                    {
                        using (var fileStreamOut = new FileStream(destFile, FileMode.Create, FileAccess.Write))
                        {
                            using (var zipOutStream = new ZipOutputStream(fileStreamOut))
                            {
                                //zipOutStream.SetLevel(6);   //设置压缩等级，默认为6
                                var buffer = new byte[4096];
                                var entry = new ZipEntry(Path.GetFileName(srcFile));
                                zipOutStream.PutNextEntry(entry);
                                int size;
                                do
                                {
                                    size = fileStreamIn.Read(buffer, 0, buffer.Length);
                                    zipOutStream.Write(buffer, 0, size);
                                } while (size > 0);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// 压缩多个文件
            /// </summary>
            /// <param name="srcFiles">多个文件路径</param>
            /// <param name="destFile">压缩文件的路径</param>
            /// <exception cref="ArgumentException"></exception>
            public static void ZipFiles(string[] srcFiles, string destFile)
            {
                if (srcFiles == null || string.IsNullOrEmpty(destFile))
                    throw new ArgumentException("参数错误");
                using (var zip = ZipFile.Create(destFile))
                {
                    zip.BeginUpdate();
                    foreach (var filePath in srcFiles)
                        zip.Add(filePath);
                    zip.CommitUpdate();
                }
            }

            /// <summary>
            /// 压缩目录
            /// </summary>
            /// <param name="dir">目录路径</param>
            /// <param name="destFile">压缩文件路径</param>
            /// <exception cref="ArgumentException">参数错误</exception>
            public static void ZipDir(string dir, string destFile)
            {
                if (string.IsNullOrEmpty(dir) || string.IsNullOrEmpty(destFile))
                    throw new ArgumentException("参数错误");
                ZipFiles(Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories), destFile);
            }

            /// <summary>
            /// 列表压缩文件里的所有文件
            /// </summary>
            /// <param name="zipPath">压缩文件路径</param>
            /// <exception cref="ArgumentException"></exception>
            /// <returns></returns>
            public static List<string> GetFileList(string zipPath)
            {
                var files = new List<string>();
                if (string.IsNullOrEmpty(zipPath))
                    throw new ArgumentException("参数错误");
                using (var zip = new ZipFile(zipPath))
                    files.AddRange(from ZipEntry entry in zip where entry.IsFile select entry.Name);
                return files;
            }

            /// <summary>
            /// 删除zip文件中的某个文件
            /// </summary>
            /// <param name="zipPath">压缩文件路径</param>
            /// <param name="files">要删除的某个文件</param>
            /// <exception cref="ArgumentException"></exception>
            public static void DeleteFileFromZip(string zipPath, string[] files)
            {
                if (string.IsNullOrEmpty(zipPath) || files == null)
                    throw new ArgumentException("参数错误");
                using (var zip = new ZipFile(zipPath))
                {
                    zip.BeginUpdate();
                    foreach (var f in files)
                        zip.Delete(f);
                    zip.CommitUpdate();
                }
            }

            /// <summary>
            /// 解压文件
            /// </summary>
            /// <param name="zipPath">要解压的文件</param>
            /// <param name="outputDir">解压后放置的目录</param>
            public static void UnZipFile(string zipPath, string outputDir) =>
                new FastZip().ExtractZip(zipPath, outputDir, "");

            /// <summary>
            /// 解压文件
            /// </summary>
            /// <param name="srcFile">压缩文件路径</param>
            /// <param name="destDir">解压后文件夹的路径</param>
            public static void Decompress(string srcFile, string destDir)
            {
                lock (OperateLock)
                {
                    using (var fileStreamIn = new FileStream(srcFile, FileMode.Open, FileAccess.Read))
                    {
                        using (var zipInStream = new ZipInputStream(fileStreamIn))
                        {
                            if (!Directory.Exists(destDir))
                                Directory.CreateDirectory(destDir);
                            ZipEntry entry;
                            while ((entry = zipInStream.GetNextEntry()) != null)
                            {
                                using (var fileStreamOut = new FileStream(destDir + @"\" + entry.Name, FileMode.Create,
                                    FileAccess.Write))
                                {
                                    int size;
                                    var buffer = new byte[4096];
                                    do
                                    {
                                        size = zipInStream.Read(buffer, 0, buffer.Length);
                                        fileStreamOut.Write(buffer, 0, size);
                                    } while (size > 0);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}