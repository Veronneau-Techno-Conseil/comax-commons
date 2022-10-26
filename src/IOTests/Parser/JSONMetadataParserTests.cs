using CommunAxiom.Commons.Ingestion.Ingestor;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommunAxiom.Commons.Ingestion.Tests.Parser
{
    [TestFixture]
    public class JSONMetadataParserTests
    {
        [Test]
        public void TestParse()
        {
            var txt = File.ReadAllText("Samples/Files/DataFile.json");
            JSONMetadataParser jSONMetadataParser = new JSONMetadataParser();
            var res = jSONMetadataParser.ReadMetadata(txt);
            
            res.Should().NotBeNull();
        }
    }
}
