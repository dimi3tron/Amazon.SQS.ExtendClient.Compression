using Amazon.SQS.ExtendClient.Compression.Test.Extensions;
 using NUnit.Framework;
 
 namespace Amazon.SQS.ExtendClient.Compression.Test
 {
     public class CompressingClientConfigurationTests
     {
         [Test]
         public void Initialize_ObjectFromEmptyCtorAndObjectFromOther_AreEquivalent()
         {
             var subject1 = new CompressingClientConfiguration();
             var subject2 = new CompressingClientConfiguration(subject1);

             Assert.AreEqual(subject1.ToJson(), subject2.ToJson());
         }
     }
 }