using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Report.Models;
using Report.Models.Domain;

namespace Report.Services
{
    public class FloodMonitoringServices : IFloodMonitoringServices
    {
        private readonly ILogging _logger;
        private readonly IConfigurationServices _config;
        private readonly ICacheWrapper _memoryCache;
        private readonly IExternalServiceWrapper _externalService;

        public FloodMonitoringServices(ILogging logger, 
            IConfigurationServices config, ICacheWrapper cache, IExternalServiceWrapper externalService)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this._logger = logger;

            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            this._config = config;

            if (cache == null)
            {
                throw new ArgumentNullException("cache");
            }

            this._memoryCache = cache;

            if (externalService == null)
            {
                throw new ArgumentNullException("externalService");
            }

            this._externalService = externalService;
        }

        public async Task<Dictionary<string, string>> GetStationReferencesAndNamesForRiver(string riverName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(riverName))
                {
                    throw new ArgumentNullException("riverName");
                }

                var refNamePairs = new Dictionary<string, string>();
                var riverStationsFromCache = (RiverStations)_memoryCache.Get(riverName);

                if (riverStationsFromCache != null)
                {
                    if (riverStationsFromCache.Stations != null && riverStationsFromCache.Stations.Any())
                    {
                        riverStationsFromCache.Stations.ForEach(s => refNamePairs.Add(s.StationReference, s.Label));
                    }
                }
                else
                {
                    var riverNameTokens = riverName.Split(' ');
                    var newRiverName = (riverNameTokens.Length > 1) ? string.Join("+", riverNameTokens) : riverName;
                    var requestUrl = string.Format(_config.GetApiRiverStationsUrl(), newRiverName);

                    var responseData = await _externalService.GetAsync(requestUrl);
                    var riverStations = JsonConvert.DeserializeObject<RiverStations>(responseData);

                    if (riverStations != null)
                    {
                        if (riverStations.Stations != null && riverStations.Stations.Any())
                        {
                            // Add data to cache memory
                            _memoryCache.AddOrGetExisting(riverName, riverStations);

                            riverStations.Stations.ForEach(s => refNamePairs.Add(s.StationReference, s.Label));
                        }
                    }
                }

                return refNamePairs;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, _config.GetDefaultLoggingPath());
                return null;
            }
        }

        public async Task<StationBasedRiverMeasureViewModel> GetRiverWaterLevelDataAtStation(string stationReference, CommandLineViewModel commandObj)
        {
            try
            {
                var cacheKey = stationReference.ToLower() + JoinFilters(commandObj);
                var processedCacheKey = cacheKey + "-processed";
                var riverMeasureView = new StationBasedRiverMeasureViewModel();
                
                riverMeasureView = (StationBasedRiverMeasureViewModel)_memoryCache.Get(processedCacheKey);

                if (riverMeasureView != null)
                {
                    return riverMeasureView;
                }
                else
                {
                    var readingsFromCache = (StationReadings)_memoryCache.Get(cacheKey);

                    if (readingsFromCache != null)
                    {
                        riverMeasureView = ProcessReadings(readingsFromCache);

                        // Add processed data to cache memory
                        _memoryCache.AddOrGetExisting(processedCacheKey, riverMeasureView);

                        return riverMeasureView;
                    }
                    else
                    {
                        var responseData = await _externalService.GetAsync(BuildApiStationReadingsUrl(stationReference, commandObj));
                        var readings = JsonConvert.DeserializeObject<StationReadings>(responseData);

                        if (readings != null)
                        {
                            if (readings.Readings != null && readings.Readings.Any())
                            {
                                // Add readings data to cache memory
                                _memoryCache.AddOrGetExisting(cacheKey, readings);

                                riverMeasureView = ProcessReadings(readings);

                                // Add processed data to cache memory
                                _memoryCache.AddOrGetExisting(processedCacheKey, riverMeasureView);

                                return riverMeasureView;
                            }
                        }
                    }                    
                }

                return riverMeasureView;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, _config.GetDefaultLoggingPath());
                return null;
            }
        }

        private string JoinFilters(CommandLineViewModel commandObj)
        {
            var joinedFilters = new StringBuilder();

            joinedFilters.Append("-");

            joinedFilters.Append(string.IsNullOrEmpty(commandObj.Parameter) ? 
                _config.GetDefaultParameter() : commandObj.Parameter.ToLower());
            
            joinedFilters.Append("-");
            joinedFilters.Append(commandObj.Limit);
            joinedFilters.Append("-");
            joinedFilters.Append(commandObj.Days);

            return joinedFilters.ToString();
        }

        private StationBasedRiverMeasureViewModel ProcessReadings(StationReadings readings)
        {
            try
            {
                var riverMeasureView = new StationBasedRiverMeasureViewModel();

                riverMeasureView.StationName = readings.Readings.FirstOrDefault().Measure.Station.Label;

                riverMeasureView.MinValue = readings.Readings.Min(r => r.Value);

                riverMeasureView.MinValueDate = readings.Readings
                    .Where(r => r.Value == riverMeasureView.MinValue)
                    .OrderByDescending(i => i.DateTime)
                    .ToList()[0].DateTime;

                riverMeasureView.MaxValue = readings.Readings.Max(r => r.Value);

                riverMeasureView.MaxValueDate = readings.Readings
                    .Where(r => r.Value == riverMeasureView.MaxValue)
                    .OrderByDescending(i => i.DateTime)
                    .ToList()[0].DateTime;

                riverMeasureView.AverageValue = readings.Readings.Average(r => r.Value);

                return riverMeasureView;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, _config.GetDefaultLoggingPath());
                return null;
            }
        }

        private string BuildApiStationReadingsUrl(string stationReference, CommandLineViewModel commandObj)
        {
            try
            {
                var urlBuilder = new StringBuilder();

                urlBuilder.Append(string.Format(_config.GetApiStationReadingsUrl(), stationReference));

                var supportedRange = _config.GetDateRange();
                var endDate = DateTime.Today;
                var startDate = endDate.AddDays((commandObj.Days > supportedRange) ? (-(double)supportedRange) : (-(double)commandObj.Days));

                urlBuilder.Append("&startdate=");
                urlBuilder.Append(startDate.ToString(_config.GetDateFormat()));

                urlBuilder.Append("&enddate=");
                urlBuilder.Append(endDate.ToString(_config.GetDateFormat()));

                urlBuilder.Append(!string.IsNullOrEmpty(commandObj.Parameter) ? ("&parameter=" + commandObj.Parameter) : "");

                var limitFilter = 
                    ((commandObj.Limit <= 0) || (commandObj.Limit > _config.GetMaxReadLimit())) ? 
                        ("&_limit=" + _config.GetDefaultReadLimit()) : 
                        ("&_limit=" + commandObj.Limit);

                urlBuilder.Append(limitFilter);

                return urlBuilder.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, _config.GetDefaultLoggingPath());
                return null;
            }
        }
    }
}
