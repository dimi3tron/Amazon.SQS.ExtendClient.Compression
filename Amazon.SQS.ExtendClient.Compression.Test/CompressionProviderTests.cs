using Gibberish;
using NUnit.Framework;
using System.Linq;
using Amazon.SQS.ExtendClient.Compression.Test.Extensions;

namespace Amazon.SQS.ExtendClient.Compression.Test
{
    public class CompressionProviderTests
    {
        [Test]
        public void Compress_ObjectsWithDifferentContent_HaveDifferentResultBytes()
        {
            var subject1 = (string)new Word();
            var subject2 = (string)new Word();
            var provider = new CompressionProvider();

            var result1 = provider.Compress(subject1);
            var result2 = provider.Compress(subject2);

            CollectionAssert.AreNotEquivalent(result1, result2);
        }

        [Test]
        public void CompressDecompress_ResultingAndOriginalObject_AreEquivalent()
        {
            var subject = new
            {
                String = "√abcdef€z$*%´´ràéµ£",
                List = new[]
                {
                    123, 456
                },
                Object = new { Value = 123 }
            }.ToJson();
            var provider = new CompressionProvider();

            var result = provider.Decompress(provider.Compress(subject));

            Assert.AreEqual(subject, result);
        }

        [Test]
        public void Compress_CompressedContentSize_IsLessThanOriginal()
        {
            var subject = string.Join(
                " ",
                Enumerable.Range(0, 5).Select(_ => new Paragraph(Tongue.Babylonian))
            );
            var provider = new CompressionProvider();

            var result = provider.Compress(subject);

            Assert.Less(result.Length, subject.Length);
        }
    }



}
