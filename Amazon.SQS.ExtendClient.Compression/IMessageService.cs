namespace Amazon.SQS.ExtendClient.Compression
{
    public interface IMessageService
    {
        string ToRequestBody(string value);
        string ToResponseBody(string value);
    }
}