namespace Amazon.SQS.ExtendClient.Compression
{
    public class CompressingClientConfiguration : ICompressingClientConfiguration
    {
        public CompressingClientConfiguration()
        {
            this.CompressionProvider = new CompressionProvider();
            this.CompressionSizeThreshold = 262144;
            this.AlwaysCompress = true;
            this.MessageParser = new ImplicitCompressionMessageParser();
        }

        public CompressingClientConfiguration(CompressingClientConfiguration other)
        {
            this.CompressionProvider = other.CompressionProvider;
            this.CompressionSizeThreshold = other.CompressionSizeThreshold;
            this.AlwaysCompress = other.AlwaysCompress;
            this.MessageParser = other.MessageParser;
        }

        public ICompressionProvider CompressionProvider { get; private set; }

        public IMessageParser MessageParser { get; private set; }

        public long CompressionSizeThreshold { get; private set; }

        public bool AlwaysCompress { get; private set; }

        public CompressionLevel CompressionLevel
            => CompressionProvider.CompressionLevel;

        public CompressingClientConfiguration WithCompressionLevel(CompressionLevel level)
        {
            this.CompressionProvider = new CompressionProvider(level);

            return this;
        }

        public CompressingClientConfiguration WithCompressionSizeThreshold(long size)
        {
            this.CompressionSizeThreshold = size;

            return this;
        }

        public CompressingClientConfiguration WithAlwaysCompress(bool alwaysCompress)
        {
            // ReSharper disable once AssignmentInConditionalExpression; intentional assignment
            if ((this.AlwaysCompress = alwaysCompress))
            {
                this.MessageParser = new ImplicitCompressionMessageParser();
            }
            else
            {
                this.MessageParser = new DefaultMessageParser();
            }

            return this;
        }
    }
}
