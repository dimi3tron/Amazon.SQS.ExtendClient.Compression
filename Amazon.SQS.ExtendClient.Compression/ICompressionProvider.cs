namespace Amazon.SQS.ExtendClient.Compression
{
    public interface ICompressionProvider
    {
        /// <summary>
        /// Compresses a given messsage
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        byte[] Compress(string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compressedMessage"></param>
        /// <returns></returns>
        string Decompress(byte[] compressedMessage);
    }
}
