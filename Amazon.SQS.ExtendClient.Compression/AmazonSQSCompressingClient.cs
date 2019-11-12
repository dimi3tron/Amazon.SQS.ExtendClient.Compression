using Amazon.SQS.ExtendedClient;
using Amazon.SQS.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.SQS.ExtendClient.Compression
{
    public class AmazonSQSCompressingClient : AmazonSQSExtendedClientBase
    {
        private readonly CompressingClientConfiguration configuration;
        private readonly IAmazonSQS sqsClient;

        public AmazonSQSCompressingClient(IAmazonSQS sqsClient)
            : this(sqsClient, new CompressingClientConfiguration()) { }

        public AmazonSQSCompressingClient(IAmazonSQS sqsClient, CompressingClientConfiguration configuration)
            : base(sqsClient)
        {
            this.sqsClient = sqsClient;
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

        private bool ShouldCompress(SendMessageRequest sendMessageRequest)
            => ShouldCompress(sendMessageRequest.MessageBody);

        private bool ShouldCompress(SendMessageBatchRequestEntry batchEntry)
            => ShouldCompress(batchEntry.MessageBody);



        private bool ShouldCompress(string message)
            => Encoding.UTF8.GetBytes(message).LongCount() > configuration.CompressionSizeThreshold;

        public override Task<SendMessageResponse> SendMessageAsync(SendMessageRequest sendMessageRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (ShouldCompress(sendMessageRequest))
            {
                sendMessageRequest.MessageBody = Compress(sendMessageRequest.MessageBody);
            }

            return sqsClient.SendMessageAsync(sendMessageRequest, cancellationToken);
        }

        public override Task<SendMessageResponse> SendMessageAsync(string queueUrl, string messageBody, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendMessageAsync(new SendMessageRequest(queueUrl, messageBody), cancellationToken);
        }

        public override Task<SendMessageBatchResponse> SendMessageBatchAsync(SendMessageBatchRequest sendMessageBatchRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var entry in sendMessageBatchRequest.Entries)
            {
                if (ShouldCompress(entry))
                {
                    entry.MessageBody = Compress(entry.MessageBody);
                }
            }

            return sqsClient.SendMessageBatchAsync(sendMessageBatchRequest, cancellationToken);
        }

        public override Task<SendMessageBatchResponse> SendMessageBatchAsync(string queueUrl, List<SendMessageBatchRequestEntry> entries, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendMessageBatchAsync(new SendMessageBatchRequest(queueUrl, entries), cancellationToken);
        }

        public override async Task<ReceiveMessageResponse> ReceiveMessageAsync(ReceiveMessageRequest receiveMessageRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await sqsClient.ReceiveMessageAsync(receiveMessageRequest, cancellationToken).ConfigureAwait(false);
            foreach (var message in result.Messages)
            {
                message.Body = Decompress(message.Body);
            }

            return result;
        }

        public override Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ReceiveMessageAsync(new ReceiveMessageRequest(queueUrl), cancellationToken);
        }
    }
}
