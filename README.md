# Amazon.SQS.ExtendClient.Compression
Add message compression to Amazon SQS Extended Client

This library is extension to [Amazon Extended Client Library for .NET](https://github.com/raol/amazon-sqs-net-extended-client-lib). 
It automatically compresses and decompresses any message sent or retrieved through the client.

## Installation

```PowerShell
Install-Package AWSSQS.ExtendClient.Compression -Version 1.1.0-aplha1
```

## Usage

### Basic

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

### Configuration

#### Size threshold

Per default the compressing client will decide whether to compress a message based on a message size threshold of 256 KB, which is the SQS size limit for a message.

You can specify another threshold as such:

```csharp
var compressingClient = new AmazonSQSCompressingClient(
    extendedClient,
    new CompressingClientConfiguration().WithCompressionSizeThreshold(1000)
);
```

When using a size threshold, all messages will be preced by a marker indicating whether the message has been compressed.

#### Always compress

If you'd rather compress all messages no matter their size, you can use:

```csharp
var compressingClient = new AmazonSQSCompressingClient(
    extendedClient,
    new CompressingClientConfiguration().WithAlwaysCompress(true)
);
```

Please note that setting a size threshold together with the always compress option will have no effect. In this case "always compress" will receive precedence.

Contrary to the messages created with size thresholds, always compressed messages will not be preceded by a marker. This is **important** to keep in mind. It means that messages compressed with "always compress" should also be decompressed with this option.

"Always compress" was the default for version 1.0.0, so beware when upgrading.

#### Compression level

You can specify a compression level as such:

```csharp
var compressingClient = new AmazonSQSCompressingClient(
    extendedClient,
    new CompressingClientConfiguration().WithCompressionLevel(CompressionLevel.Highest);
);
```

The default compression level is `CompressionLevel.High`.

The default compression provider uses gzip compression. The compression levels correspond to gzip compression levels as such:


```
Lowest = 1,
Low = 3,
Medium = 5,
High = 7,
Highest = 9
```
