using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Report.Services
{
    public class ExternalServiceWrapper : IExternalServiceWrapper
    {
        private readonly ILogging _logger;
        private readonly IConfigurationServices _config;

        public ExternalServiceWrapper(ILogging logger, IConfigurationServices config)
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
        }

        public async Task<string> GetAsync(string requestUrl)
        {
            try
            {
                var responseData = "";

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(new ProductHeaderValue("FloodMonitoring")));

                    var response = await client.GetAsync(new Uri(requestUrl));

                    if (response.IsSuccessStatusCode)
                    {
                        responseData = await response.Content.ReadAsStringAsync();
                    }
                }

                return responseData;
            }
            catch (Exception ex)
            {
                _logger.LogException(ex, _config.GetDefaultLoggingPath());
                return null;
            }
        }
    }
}
