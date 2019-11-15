namespace Amazon.SQS.ExtendClient.Compression
{
    public interface IMessageParser
    {
        MessageBody Parse(string value);
    }
}