using CommunAxiom.Commons.Client.Contracts.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Ingestor;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;

namespace CommunAxiom.Commons.Ingestion.Tests.Ingestor
{
    [TestFixture]
	public class IngestorFactoryTest
	{
		private MockRepository _mockRepository;
		private Mock<Func<IngestorType, IIngestor>> _serviceResolver;

		private IngestorFactory _ingestorFactory;

		[SetUp]
		public void SetUp()
		{
			_mockRepository = new MockRepository(MockBehavior.Strict);
			_serviceResolver = _mockRepository.Create<Func<IngestorType, IIngestor>>();
			
			_ingestorFactory = new IngestorFactory(_serviceResolver.Object);
		}

		[TearDown]
		public void TearDown()
		{
			_mockRepository.Verify();
		}

		[Test]
		public void WhenIngestorTypeIsJsonThenIngestorFactoryShouldJsonIngestor()
		{
			var jsonIngestor = new JsonIngestor();

			_serviceResolver.Setup(x => x.Invoke(IngestorType.JSON)).Returns(jsonIngestor);

			var reader = _ingestorFactory.Create(IngestorType.JSON);
			reader.Should().BeOfType<JsonIngestor>();
		}

		[Test]
		public void WhenServiceProviderIsResolvedNullThenIngestorFactoryShouldThrowNullReferenceException()
		{
			var ingestorType = IngestorType.JSON;

			_serviceResolver.Setup(x => x.Invoke(ingestorType)).Returns((IIngestor)null!);

			Action act = () => _ingestorFactory.Create(ingestorType);

			act.Should()
				.Throw<NullReferenceException>()
				.WithMessage("No Ingestor resolved!");
		}
		
	}
}

