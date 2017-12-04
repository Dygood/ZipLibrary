using YuWan.ZipLibrary;

namespace ZipLibTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /// <summary>
            /// 压缩文件
            /// </summary>
            /// <param name="srcFile">要压缩的文件路径</param>
            /// <param name="destFile">生成的压缩文件路径</param>
            /// 测试CompressFile(string srcFile, string destFile)
            string srcFile = @"D:\TeamViewer\TeamViewer_StaticRes.dll";
            string destFile = @"D:\test.zip";//路径需根据实际情况自己设定
            DCompressV1.CompressFile(srcFile, destFile);
            //解压也没问题.
            //DCompressV1.Decompress(destFile, @"D:\");
            //V2版本的使用方式差不多.
        }
    }
}
