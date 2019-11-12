using Amazon.SQS.ExtendedClient;

namespace Amazon.SQS.ExtendClient.Compression
{
    public class CompressingClientConfiguration : ExtendedClientConfiguration
    {
        public CompressingClientConfiguration()
            : base()
        {
            CompressionProvider = new CompressionProvider();
            CompressionSizeThreshold = 262144;
            AlwaysCompress = true;
        }

        public CompressingClientConfiguration(CompressingClientConfiguration other)
            : base(other)
        {
            CompressionProvider = other.CompressionProvider;
        }

        public ICompressionProvider CompressionProvider { get; private set; }

        public long CompressionSizeThreshold { get; private set; }

        public bool AlwaysCompress { get; private set; }

        public CompressionLevel CompressionLevel
            => CompressionProvider.CompressionLevel;

        public ExtendedClientConfiguration WithCompressionLevel(CompressionLevel level)
        {
            CompressionProvider = new CompressionProvider(level);

            return this;
        }
    }
}
