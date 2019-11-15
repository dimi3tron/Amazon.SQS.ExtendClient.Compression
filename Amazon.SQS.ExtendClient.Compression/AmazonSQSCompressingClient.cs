using Amazon.SQS.ExtendedClient;
using Amazon.SQS.Model;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Amazon.SQS.ExtendClient.Compression
{
    public class AmazonSQSCompressingClient : AmazonSQSExtendedClientBase
    {
        protected readonly IMessageService MessageService;
        protected readonly IAmazonSQS SQSClient;

        public AmazonSQSCompressingClient(IAmazonSQS sqsClient)
            : this(sqsClient, new CompressingClientConfiguration()) { }

        public AmazonSQSCompressingClient(IAmazonSQS sqsClient, ICompressingClientConfiguration configuration)
            : this(sqsClient, new MessageService(configuration)) { }

        protected AmazonSQSCompressingClient(IAmazonSQS sqsClient, IMessageService messageService)
            : base(sqsClient)
        {
            this.SQSClient = sqsClient;
            this.MessageService = messageService;
        }

        public override Task<SendMessageResponse> SendMessageAsync(SendMessageRequest sendMessageRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            sendMessageRequest.MessageBody = this.MessageService.ToRequestBody(sendMessageRequest.MessageBody);

            return SQSClient.SendMessageAsync(sendMessageRequest, cancellationToken);
        }

        public override Task<SendMessageResponse> SendMessageAsync(string queueUrl, string messageBody, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendMessageAsync(new SendMessageRequest(queueUrl, messageBody), cancellationToken);
        }

        public override Task<SendMessageBatchResponse> SendMessageBatchAsync(SendMessageBatchRequest sendMessageBatchRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            foreach (var entry in sendMessageBatchRequest.Entries)
            {
                entry.MessageBody = this.MessageService.ToRequestBody(entry.MessageBody);
            }

            return SQSClient.SendMessageBatchAsync(sendMessageBatchRequest, cancellationToken);
        }

        public override Task<SendMessageBatchResponse> SendMessageBatchAsync(string queueUrl, List<SendMessageBatchRequestEntry> entries, CancellationToken cancellationToken = default(CancellationToken))
        {
            return SendMessageBatchAsync(new SendMessageBatchRequest(queueUrl, entries), cancellationToken);
        }

        public override async Task<ReceiveMessageResponse> ReceiveMessageAsync(ReceiveMessageRequest receiveMessageRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            var result = await SQSClient.ReceiveMessageAsync(receiveMessageRequest, cancellationToken).ConfigureAwait(false);
            foreach (var message in result.Messages)
            {
                message.Body = this.MessageService.ToResponseBody(message.Body);
            }

            return result;
        }

        public override Task<ReceiveMessageResponse> ReceiveMessageAsync(string queueUrl, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ReceiveMessageAsync(new ReceiveMessageRequest(queueUrl), cancellationToken);
        }
    }
}
