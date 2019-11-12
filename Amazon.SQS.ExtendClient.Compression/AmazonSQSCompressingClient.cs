using Amazon.SQS.ExtendedClient;
using Amazon.SQS.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.SQS.ExtendClient.Compression
{
    public class AmazonSQSCompressingClient : AmazonSQSExtendedClientBase
    {
        private readonly IMessageService messageService;
        private readonly IAmazonSQS sqsClient;

        public AmazonSQSCompressingClient(IAmazonSQS sqsClient)
            : this(sqsClient, new CompressingClientConfiguration()) { }

        public AmazonSQSCompressingClient(IAmazonSQS sqsClient, ICompressingClientConfiguration configuration)
            : base(sqsClient)
        {
            this.sqsClient = sqsClient;
            this.messageService = new MessageService(configuration);
        }

        public override Task<SendMessageResponse> SendMessageAsync(SendMessageRequest sendMessageRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            sendMessageRequest.MessageBody = this.messageService.ToRequestBody(sendMessageRequest.MessageBody);

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
                entry.MessageBody = this.messageService.ToRequestBody(entry.MessageBody);
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
                message.Body = this.messageService.ToResponseBody(message.Body);
            }

            return result;
        }

        public override Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ReceiveMessageAsync(new ReceiveMessageRequest(queueUrl), cancellationToken);
        }
    }
}
