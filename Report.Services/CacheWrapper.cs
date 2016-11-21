using System;
using System.Runtime.Caching;

namespace Report.Services
{
    public class CacheWrapper : ICacheWrapper
    {
        private readonly ILogging _logger;
        private readonly IConfigurationServices _config;
        private static ObjectCache _memoryCache;

        private CacheItemPolicy _cachePolicy
        {
            get
            {
                var duration = Convert.ToDouble(_config.GetCacheDuration());
                var policy = new CacheItemPolicy()
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddMinutes(duration)
                };

                return policy;
            }
        }

        public CacheWrapper(ILogging logger, IConfigurationServices config)
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

            _memoryCache = MemoryCache.Default;
        }

        public object Get(string key)
        {
            return _memoryCache.Get(key);
        }

        public object AddOrGetExisting(string key, object value)
        {
            return _memoryCache.AddOrGetExisting(key, value, _cachePolicy);
        }
    }
}
