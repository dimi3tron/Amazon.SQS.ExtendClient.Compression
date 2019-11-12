using System;
using System.Linq;
using System.Text;

namespace Amazon.SQS.ExtendClient.Compression
{
    public class MessageService : IMessageService
    {
        private readonly ICompressingClientConfiguration configuration;

        public MessageService(ICompressingClientConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private string Compress(string message)
            => System.Convert.ToBase64String(
                configuration.CompressionProvider.Compress(message)
            );

        private string Decompress(string message)
            => configuration.CompressionProvider.Decompress(
                System.Convert.FromBase64String(message)
            );

        private bool ShouldCompress(string message)
            => Encoding.UTF8.GetBytes(message).LongCount() > configuration.CompressionSizeThreshold;

        private bool TryParseResponseBody(string value, out MessageBody body)
        {
            try
            {
                body = configuration.MessageParser.Parse(value);
            }
            catch (Exception ex)
            {
                body = default;
                System.Diagnostics.Debug.Write($"Failed to parse message. Reason: {ex.Message}");
            }

            return body.HasValue;
        }

        public string ToRequestBody(string value)
        {
            if (configuration.AlwaysCompress)
            {
                return new MessageBody(
                    true,
                    Compress(value),
                    true
                ).ToString();
            }

            var compress = ShouldCompress(value);

            return new MessageBody(
                compress,
                compress ?
                    Compress(value) :
                    value
            ).ToString();
        }

        public string ToResponseBody(string value)
        {
            if (TryParseResponseBody(value, out var body))
            {
                return body.IsCompressed ?
                    Decompress(body.Value) :
                    body.Value;
            }

            return value;
        }
    }
}
