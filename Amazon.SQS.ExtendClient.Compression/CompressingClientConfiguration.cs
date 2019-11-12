using Amazon.SQS.ExtendedClient;

namespace Amazon.SQS.ExtendClient.Compression
{
    public class CompressingClientConfiguration : ExtendedClientConfiguration
    {
        public CompressingClientConfiguration()
            : base()
        {
            //
        }

        public CompressingClientConfiguration(CompressingClientConfiguration other)
            : base(other)
        {
            //
        }



    }
}
