namespace Amazon.SQS.ExtendClient.Compression
{
    public class AlwaysCompressedMessageParser : IMessageParser
    {
        public MessageBody Parse(string value)
            => new MessageBody(false, value, true);
    }
}