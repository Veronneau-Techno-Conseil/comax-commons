using CommunAxiom.Commons.Ingestion.Ingestor;
using FluentAssertions;
using NUnit.Framework;
using System.IO;

namespace CommunAxiom.Commons.Ingestion.Tests.Parser
{
    [TestFixture]
    public class JsonMetadataParserTests
    {
        [Test]
        public void TestParse()
        {
            var txt = File.ReadAllText("Samples/Files/DataFile.json");
            var metadataParser = new JSONMetadataParser();
            var res = metadataParser.ReadMetadata(txt);
            
            res.Should().NotBeNull();
        }
    }
}
