# Amazon.SQS.ExtendClient.Compression
Add message compression to Amazon SQS Extended Client

This library is extension to [Amazon Extended Client Library for .NET](https://github.com/raol/amazon-sqs-net-extended-client-lib). 
It automatically compresses and decompresses any message sent or retrieved through the client.

## Installation

```PowerShell
Install-Package AWSSQS.ExtendClient.Compression -Version 1.0.0-beta1
```

## Usage

```csharp
var s3Client = new AmazonS3Client(new BasicAWSCredentials("<key>", "<secret>"), "<region>")
var sqsClient = new AmazonSQSClient(new BasicAWSCredentials("<key>", "<secret>"), "<region>");
var extendedClient = new AmazonSQSExtendedClient(
    sqsClient, 
    new ExtendedClientConfiguration().WithLargePayloadSupportEnabled(s3Client, "<s3bucketname>"));
var compressingClient = new AmazonSQSCompressingClient(extendedClient);    
    
compressingClient.SendMessage(queueUrl, "MessageBody")
```

The constructor takes in any `IAmazonSQS` so you can also use `AmazonSQSClient` if you don't want to use the extended client's functionality.
There is however still a dependency on the Amazon Extended Client Library for .NET package which serves as a base for this project.

At this point it is not possible to set any options such as compression ratio, or size treshold. These will be added in the near future. 
