using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Report.Models;
using Report.Models.Domain;
using Report.Models.Domain.Base;

namespace Report.Services.Tests
{
    [TestFixture]
    public class FloodMonitoringServicesTests
    {
        private static Mock<IConfigurationServices> _mockConfig;
        private static Mock<ILogging> _mockLogger;
        private static Mock<ICacheWrapper> _mockCache;
        private static Mock<IExternalServiceWrapper> _mockExternalService;
        private static FloodMonitoringServices _monitor;
        private static readonly string _stationRef = "131313";
        private static CommandLineViewModel _commandObj;
        
        [SetUp]
        public void Init()
        {
            _mockConfig = new Mock<IConfigurationServices>();
            _mockLogger = new Mock<ILogging>();
            _mockCache = new Mock<ICacheWrapper>();
            _mockExternalService = new Mock<IExternalServiceWrapper>();

            _monitor = new FloodMonitoringServices(
                _mockLogger.Object, _mockConfig.Object, _mockCache.Object, _mockExternalService.Object);

            _commandObj = new CommandLineViewModel
            {
                RiverName = "Testttt",
                Parameter = "ew",
                Limit = 1300,
                Days = 3
            };
        }
        
        [Test]
        public void Given_EmptyRiverName_When_CallingGetStationReferencesAndNamesForRiver_Then_ArgumentNullExceptionReturned()
        {
            var emptyRiverName = string.Empty;

            _mockLogger.Setup(m => m.LogException(It.IsAny<ArgumentNullException>(), It.IsAny<string>()))
                .Throws(new ArgumentNullException("emptyRiverName"));

            Assert.ThrowsAsync<ArgumentNullException>(async () => await _monitor.GetStationReferencesAndNamesForRiver(emptyRiverName));
        }

        [Test]
        public void Given_EmptyRiverName_When_CallingGetStationReferencesAndNamesForRiver_Then_NullReturned()
        {
            var emptyRiverName = string.Empty;

            Assert.That(async () => await _monitor.GetStationReferencesAndNamesForRiver(emptyRiverName), Is.Null);
        }

        [Test]
        public void Given_ValidRiverNameWithExistingCacheEntry_When_CallingGetStationReferencesAndNamesForRiver_Then_CachedDataReturned()
        {
            var validRiverName = "LittleRiver";            
            var mockRiverStations = new RiverStations
            {
                Context = "myContext",
                MetaData = new Meta(),
                Stations = new List<Station>
                {
                    new Station { StationReference = "5112TH", Label = "Clavering" },
                    new Station { StationReference = "1313VT", Label = "Dummy Station" }
                }
            };

            _mockCache.Setup(m => m.Get(validRiverName)).Returns(mockRiverStations);

            var returnedData = _monitor.GetStationReferencesAndNamesForRiver(validRiverName).Result;

            Assert.That(returnedData, Is.Not.Null);
            Assert.That(returnedData["5112TH"], Is.EqualTo("Clavering"));
            Assert.That(returnedData["1313VT"], Is.EqualTo("Dummy Station"));
        }

        [Test]
        public void Given_ValidComplexRiverNameWithExistingCacheEntry_When_CallingGetStationReferencesAndNamesForRiver_Then_CachedDataReturned()
        {
            var validComplexRiverName = "Complex Little River";
            var mockRiverStations = new RiverStations
            {
                Context = "myContext",
                MetaData = new Meta(),
                Stations = new List<Station>
                {
                    new Station { StationReference = "5112TH", Label = "Clavering" },
                    new Station { StationReference = "1313VT", Label = "Dummy Station" }
                }
            };

            _mockCache.Setup(m => m.Get(validComplexRiverName)).Returns(mockRiverStations);

            var returnedData = _monitor.GetStationReferencesAndNamesForRiver(validComplexRiverName).Result;

            Assert.That(returnedData, Is.Not.Null);
            Assert.That(returnedData["5112TH"], Is.EqualTo("Clavering"));
            Assert.That(returnedData["1313VT"], Is.EqualTo("Dummy Station"));
        }

        [Test]
        public void Given_ValidRiverNameWithoutExistingCacheEntry_When_CallingGetStationReferencesAndNamesForRiver_Then_CorrectDataReturned()
        {
            var testRequestUrl = "http://environment.data.gov.uk/flood-monitoring/id/stations?riverName={0}";
            var validRiverName = "Test";
            var mockResponseData = @"{
                '@context': 'http://environment.data.gov.uk/flood-monitoring/meta/context.jsonld',
                'meta': {
                    'publisher': 'Environment Agency',
                    'licence': 'http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/',
                    'documentation': 'http://environment.data.gov.uk/flood-monitoring/doc/reference',
                    'version': '0.7',
                    'comment': 'Status: Beta service',
                    'hasFormat': [ 
			            'http://environment.data.gov.uk/flood-monitoring/id/stations.csv?riverName=Stort', 
			            'http://environment.data.gov.uk/flood-monitoring/id/stations.rdf?riverName=Stort', 
			            'http://environment.data.gov.uk/flood-monitoring/id/stations.ttl?riverName=Stort', 
			            'http://environment.data.gov.uk/flood-monitoring/id/stations.html?riverName=Stort' 
		            ]
                },
                'items': [
                    {
                        '@id': 'http://environment.data.gov.uk/flood-monitoring/id/stations/5151TH',
                        'RLOIid': '7351',
                        'catchmentName': 'Stort',
                        'dateOpened': '2002-01-01',
                        'easting': 548906,
                        'gridReference': 'TL4890614817',
                        'label': 'Sawbridgeworth',
                        'lat': 51.812151,
                        'long': 0.158674,
                        'measures': [
                            {
                                '@id': 'http://environment.data.gov.uk/flood-monitoring/id/measures/5151TH-level-stage-i-15_min-mASD',
                                'parameter': 'level',
                                'parameterName': 'Water Level',
                                'period': 900,
                                'qualifier': 'Stage',
                                'unitName': 'mASD'
                            }
                        ],
                        'northing': 214817,
                        'notation': '5151TH',
                        'riverName': 'Stort',
                        'stageScale': 'http://environment.data.gov.uk/flood-monitoring/id/stations/5151TH/stageScale',
                        'stationReference': '5151TH',
                        'status': 'http://environment.data.gov.uk/flood-monitoring/def/core/statusActive',
                        'town': 'Sawbridgeworth',
                        'wiskiID': '5151TH'
                    }
                ]
            }";

            _mockConfig.Setup(m => m.GetApiRiverStationsUrl()).Returns(testRequestUrl);
            _mockExternalService.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(mockResponseData);
            _mockCache.Setup(m => m.AddOrGetExisting(It.IsAny<string>(), It.IsAny<RiverStations>())).Returns(new object());

            var returnedData = _monitor.GetStationReferencesAndNamesForRiver(validRiverName).Result;

            Assert.That(returnedData, Is.Not.Null);
            Assert.That(returnedData["5151TH"], Is.EqualTo("Sawbridgeworth"));
        }

        [Test]
        public void Given_InvalidRiverName_When_CallingGetStationReferencesAndNamesForRiver_Then_EmptyCollectionReturned()
        {
            var invalidRiverName = "Fake";
            var testRequestUrl = "http://environment.data.gov.uk/flood-monitoring/id/stations?riverName={0}";
            var mockResponseData = @"{
                '@context': 'http://environment.data.gov.uk/flood-monitoring/meta/context.jsonld',
                'meta': {
                    'publisher': 'Environment Agency',
                    'licence': 'http://www.nationalarchives.gov.uk/doc/open-government-licence/version/3/',
                    'documentation': 'http://environment.data.gov.uk/flood-monitoring/doc/reference',
                    'version': '0.7',
                    'comment': 'Status: Beta service',
                    'hasFormat': [ 
			            'http://environment.data.gov.uk/flood-monitoring/id/stations.csv?riverName=Stort', 
			            'http://environment.data.gov.uk/flood-monitoring/id/stations.rdf?riverName=Stort', 
			            'http://environment.data.gov.uk/flood-monitoring/id/stations.ttl?riverName=Stort', 
			            'http://environment.data.gov.uk/flood-monitoring/id/stations.html?riverName=Stort' 
		            ]
                },
                'items': [ ]
            }";

            _mockConfig.Setup(m => m.GetApiRiverStationsUrl()).Returns(testRequestUrl);
            _mockExternalService.Setup(m => m.GetAsync(It.IsAny<string>())).ReturnsAsync(mockResponseData);

            var returnedData = _monitor.GetStationReferencesAndNamesForRiver(invalidRiverName).Result;

            Assert.That(returnedData, Is.Empty);
        }

        [Test]
        public void Given_ValidStationReferenceWithExistingCacheEntry_When_CallingGetRiverWaterLevelDataAtStation_Then_CachedDataReturned()
        {
            var resultView = new StationBasedRiverMeasureViewModel
            {
                AverageValue = 0.134M,
                MaxValue = 3.456M,
                MaxValueDate = DateTime.Today,
                MinValue = 1.234M,
                MinValueDate = DateTime.Today,
                StationName = "Whatever"
            };            

            _mockCache.Setup(m => m.Get(It.IsAny<string>())).Returns(resultView);

            var result = _monitor.GetRiverWaterLevelDataAtStation(_stationRef, _commandObj).Result;

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.EqualTo(resultView));
        }

        [Test]
        public void Given_NoCachedResultAndStationReadingsDataInCache_When_CallingGetRiverWaterLevelDataAtStation_Then_CorrectCalculatedDataReturned()
        {
            var stationReadings = new StationReadings
            {
                Context = "myContext",
                MetaData = new Meta(),
                Readings = new List<Reading>()
            };

            stationReadings.Readings.Add(new Reading
            {
                Date = DateTime.Today,
                DateTime = DateTime.Today,
                Value = 1.234M,
                Measure = new Measure { Station = new Station { Label = "Test Station 1" } }
            });

            stationReadings.Readings.Add(new Reading
            {
                Date = DateTime.Today,
                DateTime = DateTime.Today,
                Value = 5.678M,
                Measure = new Measure { Station = new Station { Label = "Test Station 2" } }
            });

            _mockCache.Setup(m => m.Get(It.IsAny<string>())).Returns(stationReadings);
            _mockCache.Setup(m => m.Get(It.Is<string>(s => s.EndsWith("-processed")))).Returns(null);
            _mockCache
                .Setup(m => m.AddOrGetExisting(It.IsAny<string>(), It.IsAny<StationBasedRiverMeasureViewModel>()))
                .Returns(new object());

            var result = _monitor.GetRiverWaterLevelDataAtStation(_stationRef, _commandObj).Result;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.MinValue, Is.EqualTo(1.234M));
            Assert.That(result.MaxValue, Is.EqualTo(5.678M));
            Assert.That(result.AverageValue, Is.EqualTo(3.456M));
        }



        // Sorry, there should be a lot more test cases here but I haven't got enough time to cover everything :)
        // ...
    }
}
