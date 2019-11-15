using System;
using System.Linq;
using System.Text;
using Gibberish;
using ICSharpCode.SharpZipLib.GZip;
using Moq;
using NUnit.Framework;

namespace Amazon.SQS.ExtendClient.Compression.Test
{
    [TestFixture]
    public class MessageServiceTests
    {
        [Test]
        public void ToRequestBody_WithAlwaysCompressAndAnyValue_CallsCompressWithValueOnce()
        {
            var configMock = new Mock<ICompressingClientConfiguration>();
            var compressionProviderMock = new Mock<ICompressionProvider>();
            var value = (string) new Sentence();
            var subject = new MessageService(configMock.Object);
            configMock
                .SetupGet(x => x.CompressionProvider)
                .Returns(compressionProviderMock.Object);
            configMock
                .SetupGet(x => x.AlwaysCompress)
                .Returns(true);

            var result = subject.ToRequestBody(value);

            compressionProviderMock
                .Verify(
                    x => x.Compress(value),
                    Times.Once
                );
        }

        [Test]
        public void ToRequestBody_WithAlwaysCompressAndAnyValue_ReturnsCompressedValueAsBase64WithoutCompressionMarker()
        {
            var configMock = new Mock<ICompressingClientConfiguration>();
            var compressionProviderMock = new Mock<ICompressionProvider>();
            var value = (string) new Sentence();
            var compressedValue = Encoding.UTF8.GetBytes("compressed");
            var subject = new MessageService(configMock.Object);
            configMock
                .SetupGet(x => x.CompressionProvider)
                .Returns(compressionProviderMock.Object);
            configMock
                .SetupGet(x => x.AlwaysCompress)
                .Returns(true);
            compressionProviderMock
                .Setup(x => x.Compress(value))
                .Returns(compressedValue);

            var result = subject.ToRequestBody(value);

            Assert.AreEqual(Convert.ToBase64String(compressedValue), result);
        }

        [Test]
        public void ToRequestBody_ValueSizeLowerThanCompressionSizeThreshold_DoesNotCallCompress()
        {
            var configMock = new Mock<ICompressingClientConfiguration>();
            var compressionProviderMock = new Mock<ICompressionProvider>();
            var value = (string) new Sentence();
            var subject = new MessageService(configMock.Object);
            configMock
                .SetupGet(x => x.CompressionProvider)
                .Returns(compressionProviderMock.Object);
            configMock
                .SetupGet(x => x.CompressionSizeThreshold)
                .Returns(value.Length + 5);

            var result = subject.ToRequestBody(value);

            compressionProviderMock
                .Verify(
                    x => x.Compress(value),
                    Times.Never
                );
        }

        [Test]
        public void
            ToRequestBody_ValueSizeLowerThanCompressionSizeThreshold_ReturnsValueAsBase64WithCompressionMarkerZero()
        {
            var configMock = new Mock<ICompressingClientConfiguration>();
            var compressionProviderMock = new Mock<ICompressionProvider>();
            var value = (string) new Sentence();
            var subject = new MessageService(configMock.Object);
            configMock
                .SetupGet(x => x.CompressionProvider)
                .Returns(compressionProviderMock.Object);
            configMock
                .SetupGet(x => x.CompressionSizeThreshold)
                .Returns(value.Length + 5);

            var result = subject.ToRequestBody(value);

            Assert.AreEqual($"0|{value}", result);
        }

        [Test]
        public void ToRequestBody_ValueSizeGreaterThanCompressionSizeThreshold_CallsCompressWithValueOnce()
        {
            var configMock = new Mock<ICompressingClientConfiguration>();
            var compressionProviderMock = new Mock<ICompressionProvider>();
            var value = (string) new Sentence();
            var subject = new MessageService(configMock.Object);
            configMock
                .SetupGet(x => x.CompressionProvider)
                .Returns(compressionProviderMock.Object);
            configMock
                .SetupGet(x => x.CompressionSizeThreshold)
                .Returns(value.Length - 5);

            var result = subject.ToRequestBody(value);

            compressionProviderMock
                .Verify(
                    x => x.Compress(value),
                    Times.Once
                );
        }

        [Test]
        public void
            ToRequestBody_ValueSizeGreaterThanCompressionSizeThreshold_ReturnsCompressedValueAsBase64WithCompressionMarkerOne()
        {
            var configMock = new Mock<ICompressingClientConfiguration>();
            var compressionProviderMock = new Mock<ICompressionProvider>();
            var value = (string) new Sentence();
            var compressedValue = Encoding.UTF8.GetBytes("compressed");
            var subject = new MessageService(configMock.Object);
            configMock
                .SetupGet(x => x.CompressionProvider)
                .Returns(compressionProviderMock.Object);
            configMock
                .SetupGet(x => x.CompressionSizeThreshold)
                .Returns(value.Length - 5);
            compressionProviderMock
                .Setup(x => x.Compress(value))
                .Returns(compressedValue);

            var result = subject.ToRequestBody(value);

            Assert.AreEqual($"1|{Convert.ToBase64String(compressedValue)}", result);
        }

        [Test]
        public void ToResponseBody_WithAlwaysCompressAndAnyBase64Value_CallsDecompressWithValueOnce()
        {
            var configMock = new Mock<ICompressingClientConfiguration>();
            var compressionProviderMock = new Mock<ICompressionProvider>();
            var value = Convert.ToBase64String(Encoding.UTF8.GetBytes(new Sentence()));
            var valueBytes = Convert.FromBase64String(value);
            var subject = new MessageService(configMock.Object);
            configMock
                .SetupGet(x => x.CompressionProvider)
                .Returns(compressionProviderMock.Object);
            configMock
                .SetupGet(x => x.MessageParser)
                .Returns(new ImplicitCompressionMessageParser());
            configMock
                .SetupGet(x => x.AlwaysCompress)
                .Returns(true);

            var result = subject.ToResponseBody(value);

            compressionProviderMock
                .Verify(
                    x => x.Decompress(valueBytes),
                    Times.Once
                );
        }

        [Test]
        public void ToResponseBody_WithAlwaysCompressAndAnyCompressedValueAsBase64_ReturnsDecompressedValue()
        {
            var config = new CompressingClientConfiguration().WithAlwaysCompress(true);
            var value = (string) new Sentence();
            var compressedValue = Convert.ToBase64String(new CompressionProvider().Compress(value));
            var subject = new MessageService(config);

            var result = subject.ToResponseBody(compressedValue);
            
            Assert.AreEqual(value, result);
        }
        
        [Test]
        public void ToResponseBody_WithAlwaysCompressAndInvalidValue_ThrowsFormatException()
        {
            var config = new CompressingClientConfiguration().WithAlwaysCompress(true);
            var value = (string) new Sentence();
            var subject = new MessageService(config);

            Assert.Throws<FormatException>(() =>
            {
                var result = subject.ToResponseBody(value);
            });
        }
        
        [Test]
        public void ToResponseBody_WithAlwaysCompressAndAnyCompressedValueAsBase64AndCompressionMarker_ThrowsFormatException()
        {
            var config = new CompressingClientConfiguration().WithAlwaysCompress(true);
            var value = (string) new Sentence();
            var compressedValue = Convert.ToBase64String(new CompressionProvider().Compress(value));
            var subject = new MessageService(config);

            Assert.Throws<FormatException>(() =>
            {
                var result = subject.ToResponseBody($"1|{compressedValue}");
            });
        }

        [Test]
        public void ToResponseBody_AnyValueAsBase64AndCompressionMarkerZero_DoesNotCallDecompress()
        {
            var configMock = new Mock<ICompressingClientConfiguration>();
            var compressionProviderMock = new Mock<ICompressionProvider>();
            var value = (string)new Sentence();
            var subject = new MessageService(configMock.Object);
            configMock
                .SetupGet(x => x.CompressionProvider)
                .Returns(compressionProviderMock.Object);
            configMock
                .SetupGet(x => x.MessageParser)
                .Returns(new DefaultMessageParser());

            var result = subject.ToResponseBody($"0|{value}");

            compressionProviderMock
                .Verify(
                    x => x.Decompress(It.IsAny<byte[]>()),
                    Times.Never
                );
        }
        
        [Test]
        public void ToResponseBody_AnyCompressedValueAsBase64AndCompressionMarkerOne_CallsDecompressWithValueOnce()
        {
            var configMock = new Mock<ICompressingClientConfiguration>();
            var compressionProviderMock = new Mock<ICompressionProvider>();
            var value = Convert.ToBase64String(Encoding.UTF8.GetBytes(new Sentence()));
            var valueBytes = Convert.FromBase64String(value);
            var subject = new MessageService(configMock.Object);
            configMock
                .SetupGet(x => x.CompressionProvider)
                .Returns(compressionProviderMock.Object);
            configMock
                .SetupGet(x => x.MessageParser)
                .Returns(new DefaultMessageParser());

            var result = subject.ToResponseBody($"1|{value}");

            compressionProviderMock
                .Verify(
                    x => x.Decompress(valueBytes),
                    Times.Once
                );
        }
        
        [Test]
        public void ToResponseBody_AnyCompressedValueAsBase64WithoutCompressionMarker_ThrowsFormatException()
        {
            var config = new CompressingClientConfiguration();
            var value = (string) new Sentence();
            var compressedValue = Convert.ToBase64String(new CompressionProvider().Compress(value));
            var subject = new MessageService(config);

            Assert.Throws<FormatException>(() =>
            {
                var result = subject.ToResponseBody(compressedValue);
            });
        }
        
        [Test]
        public void ToResponseBody_AnyCompressedValueAsBase64WithCompressionMarkerOne_ReturnsDecompressedValue()
        {
            var config = new CompressingClientConfiguration();
            var value = (string) new Sentence();
            var compressedValue = Convert.ToBase64String(new CompressionProvider().Compress(value));
            var subject = new MessageService(config);

            var result = subject.ToResponseBody($"1|{compressedValue}");
            
            Assert.AreEqual(value, result);
        }
        
        [Test]
        public void ToResponseBody_AnyValueWithCompressionMarkerOne_ThrowsFormatException()
        {
            var config = new CompressingClientConfiguration();
            var value = (string) new Sentence();
            var subject = new MessageService(config);
            
            Assert.Throws<FormatException>(() =>
            {
                var result = subject.ToResponseBody($"1|{value}");
            });
        }
        
        [Test]
        public void ToResponseBody_EmptyValue_ThrowsFormatException()
        {
            var config = new CompressingClientConfiguration();
            var value = string.Empty;
            var subject = new MessageService(config);
            
            Assert.Throws<FormatException>(() =>
            {
                var result = subject.ToResponseBody(value);
            });
        }
        
        [Test]
        public void ToResponseBody_NullValue_ThrowsFormatException()
        {
            var config = new CompressingClientConfiguration();
            var value = null as string;
            var subject = new MessageService(config);
            
            Assert.Throws<FormatException>(() =>
            {
                var result = subject.ToResponseBody(value);
            });
        }
        
        [Test]
        public void ToResponseBody_AnyValueAsBase4WithCompressionMarkerOne_ThrowsGZipException()
        {
            var config = new CompressingClientConfiguration();
            var value = Convert.ToBase64String(Encoding.UTF8.GetBytes(new Sentence())); 
            var subject = new MessageService(config);
            
            Assert.Throws<GZipException>(() =>
            {
                var result = subject.ToResponseBody($"1|{value}");
            });
        }
        
        [Test]
        public void ToResponseBody_AnyValueWithCompressionMarkerZero_ReturnsValue()
        {
            var config = new CompressingClientConfiguration();
            var value = (string) new Sentence();
            var subject = new MessageService(config);

            var result = subject.ToResponseBody($"0|{value}");
            
            Assert.AreEqual(value, result);
        }
    }
}