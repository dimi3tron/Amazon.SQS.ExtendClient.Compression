using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;

namespace Amazon.SQS.ExtendClient.Compression
{
    /// <inheritdoc cref="ICompressionProvider"/>
    public class CompressionProvider : ICompressionProvider
    {
        public byte[] Compress(string message)
        {
            if (message == null) return null;

            using (var dataStream = new MemoryStream())
            using (var zipStream = new GZipOutputStream(dataStream))
            {
                zipStream.SetLevel(9);
                var rawBytes = Encoding.UTF8.GetBytes(message);

                zipStream.Write(rawBytes, 0, rawBytes.Length);

                zipStream.Flush();
                zipStream.Finish();

                var compressedBytes = new byte[dataStream.Length];
                dataStream.Seek(0, SeekOrigin.Begin);
                dataStream.Read(compressedBytes, 0, compressedBytes.Length);

                return compressedBytes;
            }
        }

        public string Decompress(byte[] compressedMessage)
        {
            if (compressedMessage == null) return null;

            using (var compressedStream = new MemoryStream(compressedMessage))
            {
                compressedStream.Position = 0;
                using (var zipStream = new GZipInputStream(compressedStream))
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