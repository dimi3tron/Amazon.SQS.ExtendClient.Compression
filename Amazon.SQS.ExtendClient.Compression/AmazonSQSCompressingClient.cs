using Amazon.SQS.ExtendedClient;
using Amazon.SQS.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.SQS.ExtendClient.Compression
{
    public class AmazonSQSCompressingClient : AmazonSQSExtendedClientBase
    {
        private readonly IAmazonSQS sqsClient;

        public AmazonSQSCompressingClient(IAmazonSQS sqsClient)
            : base(sqsClient)
        {
            this.sqsClient = sqsClient;
        }

        private static string Compress(string message)
            => System.Convert.ToBase64String(message.Compress());

        private static string Decompress(string message)
            => System.Convert.FromBase64String(message).Decompress();

        public override Task<SendMessageResponse> SendMessageAsync(SendMessageRequest sendMessageRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            sendMessageRequest.MessageBody = Compress(sendMessageRequest.MessageBody);

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
                entry.MessageBody = Compress(entry.MessageBody);
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
