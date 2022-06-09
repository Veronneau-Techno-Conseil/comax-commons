using System;
using CommunAxiom.Commons.Ingestion.Configuration;
using CommunAxiom.Commons.Ingestion.Ingestor;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using FluentAssertions;
using CommunAxiom.Commons.Ingestion.Validators;

namespace CommunAxiom.Commons.Ingestion.Tests.Ingestor
{
    [TestFixture]
	public class IngestorFactoryTest
	{
		private MockRepository _mockRepository;
		private Mock<IServiceProvider> _serviceProvider;
		private Mock<IFieldValidatorLookup> _fieldValidatorLookup;

		private IngestorFactory _ingestorFactory;

		[SetUp]
		public void SetUp()
		{
			_mockRepository = new MockRepository(MockBehavior.Strict);
			_serviceProvider = _mockRepository.Create<IServiceProvider>();
			_fieldValidatorLookup = _mockRepository.Create<IFieldValidatorLookup>();

			SetupMock();

			_ingestorFactory = new IngestorFactory(_serviceProvider.Object);
		}

		[TearDown]
		public void TearDown()
		{
			_mockRepository.Verify();
		}

		[Test]
		public void WhenIngestorTypeIsJSONThenIngestorFactoryShouldJsonIngestor()
		{
			var jsonIngestor = new JsonIngestor(_fieldValidatorLookup.Object);

			_serviceProvider.Setup(x => x.GetService(typeof(JsonIngestor))).Returns(jsonIngestor);

			var reader = _ingestorFactory.Create(IngestorType.JSON);
			reader.Should().BeOfType<JsonIngestor>();
		}

		[Test]
		public void WhenIngestorTypeIsNotFoundThenIngestorFactoryShouldThrowArgumentException()
		{
			var ingestorType = IngestorType.CSV;
			var jsonIngestor = new JsonIngestor(_fieldValidatorLookup.Object);

			_serviceProvider.Setup(x => x.GetService(typeof(JsonIngestor))).Returns(jsonIngestor);

			Action act = () => _ingestorFactory.Create(ingestorType);

			act.Should()
				.Throw<ArgumentException>()
				.WithMessage($"No IngestionType with name {Enum.GetName(ingestorType)} could be found");
		}

		[Test]
		public void WhenServiceProviderIsResolvedNullThenIngestorFactoryShouldThrowNullReferenceException()
		{
			var ingestorType = IngestorType.JSON;

			_serviceProvider.Setup(x => x.GetService(typeof(JsonIngestor))).Returns(null);

			Action act = () => _ingestorFactory.Create(ingestorType);

			act.Should()
				.Throw<NullReferenceException>()
				.WithMessage($"No IngestionType resolved with type {typeof(JsonIngestor).FullName}");
		}


		private void SetupMock()
		{

			var serviceScope = new Mock<IServiceScope>();
			serviceScope.Setup(x => x.ServiceProvider).Returns(_serviceProvider.Object);

			var serviceScopeFactory = new Mock<IServiceScopeFactory>();
			serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);

			_serviceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);
		}
	}
}

