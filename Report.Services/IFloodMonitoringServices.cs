using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Report.Models;

namespace Report.Services
{
    public interface IFloodMonitoringServices
    {
        Task<Dictionary<string, string>> GetStationReferencesAndNamesForRiver(string riverName);
        Task<StationBasedRiverMeasureViewModel> GetRiverWaterLevelDataAtStation(string stationReference, CommandLineViewModel commandObj);
    }
}
