namespace Amazon.SQS.ExtendClient.Compression
{
    public class ImplicitCompressionMessageParser : IMessageParser
    {
        public MessageBody Parse(string value)
            => new MessageBody(false, value, true);
    }
}