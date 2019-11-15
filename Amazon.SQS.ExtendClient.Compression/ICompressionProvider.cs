namespace Amazon.SQS.ExtendClient.Compression
{
    public interface ICompressionProvider
    {
        /// <summary>
        /// Compresses a given message
        /// </summary>
        /// <param name="message">The message</param>
        /// <returns>The message as compressed bytes</returns>
        byte[] Compress(string message);

        /// <summary>
        /// Decompresses a given message
        /// </summary>
        /// <param name="compressedMessage">Compressed bytes</param>
        /// <returns>The message as string</returns>
        string Decompress(byte[] compressedMessage);


        /// <summary>
        /// The level of compression
        /// </summary>
        CompressionLevel CompressionLevel { get; }
    }
}
