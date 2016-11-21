using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Report.Services
{
    public class ConfigurationServices : IConfigurationServices
    {
        private readonly ILogging _logger;

        private static Lazy<IDictionary<string, string>> _appSettings;
        public static IEnumerable<KeyValuePair<string, string>> AppSettings
        {
            get { return _appSettings.Value; }
        }

        public ConfigurationServices(ILogging logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException("logger");
            }

            this._logger = logger;

            _appSettings = new Lazy<IDictionary<string, string>>(this.LoadApplicationSettings);
        }

        public Dictionary<string, string> GetAllApplicationSettings()
        {
            if (AppSettings != null && AppSettings.Any())
            {
                var settings = AppSettings.ToDictionary(x => x.Key, x => x.Value);
                
                return settings;
            }

            return null;
        }

        public string GetDefaultLoggingPath()
        {
            try
            {
                if (AppSettings != null && AppSettings.Any())
                {
                    return AppSettings.Where(s => s.Key == "defaultLoggingPath").FirstOrDefault().Value;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, null);
                return null;
            }
        }

        public int GetCacheDuration()
        {
            try
            {
                var cacheDuration = 0;
                
                if (AppSettings != null && AppSettings.Any())
                {
                    cacheDuration = Convert.ToInt32(AppSettings.
                        Where(s => s.Key == "cacheDuration").FirstOrDefault().Value);
                }

                return cacheDuration;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, null);
                return 0;
            }
        }

        public string GetApiRiverStationsUrl()
        {
            try
            {
                if (AppSettings != null && AppSettings.Any())
                {
                    return AppSettings.Where(s => s.Key == "apiRiverStationsUrl").FirstOrDefault().Value;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, null);
                return null;
            }
        }

        public string GetApiStationReadingsUrl()
        {
            try
            {
                if (AppSettings != null && AppSettings.Any())
                {
                    return AppSettings.Where(s => s.Key == "apiStationReadingsUrl").FirstOrDefault().Value;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, null);
                return null;
            }
        }

        public int GetDateRange()
        {
            try
            {
                var range = 0;

                if (AppSettings != null && AppSettings.Any())
                {
                    range = Convert.ToInt32(AppSettings.
                        Where(s => s.Key == "dateRange").FirstOrDefault().Value);
                }

                return range;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, null);
                return 0;
            }
        }

        public string GetDateFormat()
        {
            try
            {
                if (AppSettings != null && AppSettings.Any())
                {
                    return AppSettings.Where(s => s.Key == "dateFormat").FirstOrDefault().Value;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, null);
                return null;
            }
        }

        public int GetMaxReadLimit()
        {
            try
            {
                var maxLimit = 0;

                if (AppSettings != null && AppSettings.Any())
                {
                    maxLimit = Convert.ToInt32(AppSettings.
                        Where(s => s.Key == "maxReadLimit").FirstOrDefault().Value);
                }

                return maxLimit;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, null);
                return 0;
            }
        }

        public int GetDefaultReadLimit()
        {
            try
            {
                var defaultLimit = 0;

                if (AppSettings != null && AppSettings.Any())
                {
                    defaultLimit = Convert.ToInt32(AppSettings.
                        Where(s => s.Key == "defaultReadLimit").FirstOrDefault().Value);
                }

                return defaultLimit;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, null);
                return 0;
            }
        }

        public string GetDefaultParameter()
        {
            try
            {
                if (AppSettings != null && AppSettings.Any())
                {
                    return AppSettings.Where(s => s.Key == "defaultParameter").FirstOrDefault().Value;
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, null);
                return null;
            }
        }

        private Dictionary<string, string> LoadApplicationSettings()
        {
            try
            {
                var settings = new Dictionary<string, string>() 
                {
                    { "defaultLoggingPath", ConfigurationManager.AppSettings["defaultLoggingPath"] },    
                    { "cacheDuration", ConfigurationManager.AppSettings["cacheDuration"] },
                    { "apiRiverStationsUrl", ConfigurationManager.AppSettings["apiRiverStationsUrl"] },
                    { "apiStationReadingsUrl", ConfigurationManager.AppSettings["apiStationReadingsUrl"] },
                    { "dateRange", ConfigurationManager.AppSettings["dateRange"] },
                    { "dateFormat", ConfigurationManager.AppSettings["dateFormat"] },
                    { "maxReadLimit", ConfigurationManager.AppSettings["maxReadLimit"] },
                    { "defaultReadLimit", ConfigurationManager.AppSettings["defaultReadLimit"] },
                    { "defaultParameter", ConfigurationManager.AppSettings["defaultParameter"] }
                };

                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, null);
                return null;
            }
        }
    }
}
