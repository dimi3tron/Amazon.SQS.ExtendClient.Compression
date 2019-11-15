using Amazon.SQS.ExtendClient.Compression.Test.Extensions;
using NUnit.Framework;

namespace Amazon.SQS.ExtendClient.Compression.Test
{
    [TestFixture]
    public class CompressingClientConfigurationTests
    {
        [Test]
        public void Initialize_ObjectFromEmptyCtorAndObjectFromOther_AreEquivalent()
        {
            var subject = new CompressingClientConfiguration();
            var result = new CompressingClientConfiguration(subject);

            Assert.AreEqual(subject.ToJson(), result.ToJson());
        }

        [Test]
        public void WithCompressionLevel_AnyValidCompressionLevel_ReturnsSame()
        {
            var subject = new CompressingClientConfiguration();
            var result = subject.WithCompressionLevel(CompressionLevel.High);

            Assert.AreEqual(subject, result);
        }

        [Test]
        public void WithCompressionSizeThreshold_AnyValidLong_ReturnsSame()
        {
            var subject = new CompressingClientConfiguration();
            var result = subject.WithCompressionSizeThreshold(1000);

            Assert.AreSame(subject, result);
        }

        [Test]
        public void WithAlwaysCompress_AnyValidBool_ReturnsSame()
        {
            var subject = new CompressingClientConfiguration();
            var result = subject.WithAlwaysCompress(true);

            Assert.AreSame(subject, result);
        }

        [Test]
        public void WithCompressionLevel_AnyValidCompressionLevel_SetsCompressionLevel()
        {
            var value = CompressionLevel.High;
            var subject = new CompressingClientConfiguration();
            var result = subject.WithCompressionLevel(value).CompressionLevel;

            Assert.AreEqual(value, result);
        }

        [Test]
        public void WithCompressionLevel_AnyValidCompressionLevel_SetsCompressionLeveOnCompressionProvider()
        {
            var value = CompressionLevel.High;
            var subject = new CompressingClientConfiguration();
            var result = subject.WithCompressionLevel(value)
                .CompressionProvider.CompressionLevel;

            Assert.AreEqual(value, result);
        }

        [Test]
        public void WithCompressionSizeThreshold_AnyValidLong_SetsCompressionSizeThreshold()
        {
            var value = 1000;
            var subject = new CompressingClientConfiguration();
            var result = subject.WithCompressionSizeThreshold(value)
                .CompressionSizeThreshold;

            Assert.AreEqual(value, result);
        }

        [Test]
        public void WithAlwaysCompress_AnyValidBool_SetsAlwaysCompress()
        {
            var value = true;
            var subject = new CompressingClientConfiguration();
            var result = subject.WithAlwaysCompress(value)
                                 .AlwaysCompress;

            Assert.AreEqual(value, result);
        }
        
        [Test]
        public void WithAlwaysCompress_True_MessageParserIsInstanceOfImplicitCompressionMessageParser()
        {
            var subject = new CompressingClientConfiguration();
            var result = subject.WithAlwaysCompress(true)
                                .MessageParser;

            Assert.IsInstanceOf<ImplicitCompressionMessageParser>(result);
        }
        
        [Test]
        public void WithAlwaysCompress_False_MessageParserIsInstanceOfDefaultMessageParser()
        {
            var subject = new CompressingClientConfiguration();
            var result = subject.WithAlwaysCompress(false)
                .MessageParser;

            Assert.IsInstanceOf<DefaultMessageParser>(result);
        }
    }
}