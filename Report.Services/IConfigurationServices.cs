using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Services
{
    public interface IConfigurationServices
    {
        string GetDefaultLoggingPath();
        int GetCacheDuration();
        string GetApiRiverStationsUrl();
        string GetApiStationReadingsUrl();
        int GetDateRange();
        string GetDateFormat();
        int GetMaxReadLimit();
        int GetDefaultReadLimit();
        string GetDefaultParameter();
        Dictionary<string, string> GetAllApplicationSettings();
    }
}
