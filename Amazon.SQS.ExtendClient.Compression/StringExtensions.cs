using ICSharpCode.SharpZipLib.GZip;
using System.IO;
using System.Text;

namespace Amazon.SQS.ExtendClient.Compression
{
    internal static class StringExtensions
    {
        public static byte[] Compress(this string me)
        {
            if (me == null) return null;

            using (var dataStream = new MemoryStream())
            using (var zipStream = new GZipOutputStream(dataStream))
            {
                zipStream.SetLevel(9);
                var rawBytes = Encoding.UTF8.GetBytes(me);

                zipStream.Write(rawBytes, 0, rawBytes.Length);

                zipStream.Flush();
                zipStream.Finish();

                var compressedBytes = new byte[dataStream.Length];
                dataStream.Seek(0, SeekOrigin.Begin);
                dataStream.Read(compressedBytes, 0, compressedBytes.Length);

                return compressedBytes;
            }
        }

        public static string Decompress(this byte[] me)
        {
            if (me == null) return null;

            using (var compressedStream = new MemoryStream(me))
            {
                compressedStream.Position = 0;
                using (var zipStream = new GZipInputStream(compressedStream))
                {
                    using (var dataStream = new MemoryStream())
                    {
                        var buffer = new byte[1024];
                        int result;
                        while ((result = zipStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            dataStream.Write(buffer, 0, result);
                        }

                        zipStream.Close();

                        return Encoding.UTF8.GetString(dataStream.ToArray());
                    }
                }
            }
        }
    }
}