using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SQS.Model;
using Gibberish;
using Moq;
using NUnit.Framework;

namespace Amazon.SQS.ExtendClient.Compression.Test
{
    [TestFixture]
    public class AmazonSQSCompressingClientTests
    {
        private class AmazonSQSCompressingClientWithTestableMessageService : AmazonSQSCompressingClient
        {
            public AmazonSQSCompressingClientWithTestableMessageService(IAmazonSQS sqsClient,
                IMessageService messageService)
                : base(sqsClient, messageService)
            {
            }
        }


        [Test]
        public void
            SendMessageAsync_AnySendMessageRequest_CallsSqsClientSendMessageAsyncWithSendMessageRequestAndCancellationTokenOnce()
        {
            var clientMock = new Mock<IAmazonSQS>();
            var value = new SendMessageRequest("url", new Sentence());
            var token = new CancellationToken();
            var subject = new AmazonSQSCompressingClient(clientMock.Object);

            var result = subject.SendMessageAsync(value, token);

            clientMock.Verify(
                x => x.SendMessageAsync(value, token),
                Times.Once
            );
        }

        [Test]
        public void
            SendMessageAsync_AnyQueueUrlAndMessageBody_CallsSqsClientSendMessageAsyncWithSendMessageRequestAndCancellationTokenOnce()
        {
            var clientMock = new Mock<IAmazonSQS>();
            var value1 = "url";
            var value2 = (string) new Sentence();
            var token = new CancellationToken();
            var subject = new AmazonSQSCompressingClient(clientMock.Object);

            var result = subject.SendMessageAsync(value1, value2, token);

            clientMock.Verify(
                x => x.SendMessageAsync(It.IsAny<SendMessageRequest>(), token),
                Times.Once
            );
        }

        [Test]
        public void SendMessageAsync_AnySendMessageRequest_CallsMessageServiceToRequestBodyOnce()
        {
            var clientMock = new Mock<IAmazonSQS>();
            var messageServiceMock = new Mock<IMessageService>();
            var value = (string) new Sentence();
            var request = new SendMessageRequest("url", value);
            var token = new CancellationToken();
            var subject = new AmazonSQSCompressingClientWithTestableMessageService(
                clientMock.Object,
                messageServiceMock.Object
            );

            var result = subject.SendMessageAsync(request, token);

            messageServiceMock.Verify(
                x => x.ToRequestBody(value),
                Times.Once
            );
        }

        [Test]
        public void
            SendMessageBatchAsync_AnySendMessageBatchRequest_CallsSqsClientSendMessageBatchAsyncWithSendMessageBatchRequestAndCancellationTokenOnce()
        {
            var clientMock = new Mock<IAmazonSQS>();
            var value = new SendMessageBatchRequest(
                "url",
                new List<SendMessageBatchRequestEntry>
                {
                    new SendMessageBatchRequestEntry("id", new Sentence())
                }
            );
            var token = new CancellationToken();
            var subject = new AmazonSQSCompressingClient(clientMock.Object);

            var result = subject.SendMessageBatchAsync(value, token);

            clientMock.Verify(
                x => x.SendMessageBatchAsync(value, token),
                Times.Once
            );
        }

        [Test]
        public void
            SendMessageBatchAsync_AnyQueueUrlAndListOfSendMessageBatchRequestEntry_CallsSqsClientSendMessageBatchAsyncWithSendMessageBatchRequestAndCancellationTokenOnce()
        {
            var clientMock = new Mock<IAmazonSQS>();
            var value1 = "url";
            var value2 = new List<SendMessageBatchRequestEntry>
            {
                new SendMessageBatchRequestEntry("id", new Sentence())
            };
            var token = new CancellationToken();
            var subject = new AmazonSQSCompressingClient(clientMock.Object);

            var result = subject.SendMessageBatchAsync(value1, value2, token);

            clientMock.Verify(
                x => x.SendMessageBatchAsync(It.IsAny<SendMessageBatchRequest>(), token),
                Times.Once
            );
        }

        [Test]
        public void SendMessageBatchAsync_AnySendMessageBatchRequest_CallsMessageServiceToRequestBodyForEachEntry()
        {
            var clientMock = new Mock<IAmazonSQS>();
            var messageServiceMock = new Mock<IMessageService>();
            var values = new[] {(string) new Sentence(), (string) new Sentence()};
            var entries = new List<SendMessageBatchRequestEntry>
            {
                new SendMessageBatchRequestEntry("id1", values[0]),
                new SendMessageBatchRequestEntry("id2", values[1])
            };
            var request = new SendMessageBatchRequest("url", entries);
            var token = new CancellationToken();
            var subject = new AmazonSQSCompressingClientWithTestableMessageService(
                clientMock.Object,
                messageServiceMock.Object
            );

            var result = subject.SendMessageBatchAsync(request, token);

            messageServiceMock.Verify(
                x => x.ToRequestBody(It.IsIn(values)),
                Times.Exactly(entries.Count)
            );
        }

        [Test]
        public void
            ReceiveMessageAsync_AnyReceiveMessageRequest_CallsSqsClientReceiveMessageAsyncWithReceiveMessageRequestAndCancellationTokenOnce()
        {
            var clientMock = new Mock<IAmazonSQS>();
            var value = new ReceiveMessageRequest("url");
            var token = new CancellationToken();
            var subject = new AmazonSQSCompressingClient(clientMock.Object);

            var result = subject.ReceiveMessageAsync(value, token);

            clientMock.Verify(
                x => x.ReceiveMessageAsync(value, token),
                Times.Once
            );
        }

        [Test]
        public void
            ReceiveMessageAsync_AnyQueueUrl_CallsSqsClientReceiveMessageAsyncWithReceiveMessageRequestAndCancellationTokenOnce()
        {
            var clientMock = new Mock<IAmazonSQS>();
            var value = "url";
            var token = new CancellationToken();
            var subject = new AmazonSQSCompressingClient(clientMock.Object);

            var result = subject.ReceiveMessageAsync(value, token);

            clientMock.Verify(
                x => x.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), token),
                Times.Once
            );
        }
        
        [Test]
        public async Task ReceiveMessageAsync_AnyReceiveMessageRequest_CallsMessageServiceToResponseBodyForEachMessage()
        {
            var clientMock = new Mock<IAmazonSQS>();
            var messageServiceMock = new Mock<IMessageService>();
            var values = new[] {(string) new Sentence(), (string) new Sentence()};
            var request = new ReceiveMessageRequest("url");
            var token = new CancellationToken();
            var subject = new AmazonSQSCompressingClientWithTestableMessageService(
                clientMock.Object,
                messageServiceMock.Object
            );
            clientMock
                .Setup(x => x.ReceiveMessageAsync(request, token))
                .Returns(
                    Task.FromResult(
                        new ReceiveMessageResponse()
                        {
                            Messages = new List<Message>()
                            {
                                new Message(){ Body = values[0] },
                                new Message(){ Body = values[1] }
                            }
                        }
                    )
                );    

            var result = await subject.ReceiveMessageAsync(request, token);

            messageServiceMock.Verify(
                x => x.ToResponseBody(It.IsIn(values)),
                Times.Exactly(values.Length)
            );
        }
    }
}