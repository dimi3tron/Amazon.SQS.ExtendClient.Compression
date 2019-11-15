namespace Amazon.SQS.ExtendClient.Compression
{
    public interface ICompressingClientConfiguration
    {
        ICompressionProvider CompressionProvider { get; }
        IMessageParser MessageParser { get; }
        long CompressionSizeThreshold { get; }
        bool AlwaysCompress { get; }
        CompressionLevel CompressionLevel { get; }
        CompressingClientConfiguration WithCompressionLevel(CompressionLevel level);
        CompressingClientConfiguration WithCompressionSizeThreshold(long size);
        CompressingClientConfiguration WithAlwaysCompress(bool alwaysCompress);
    }
}