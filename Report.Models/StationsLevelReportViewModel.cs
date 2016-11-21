using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Models
{
    public class StationsLevelReportViewModel
    {
        public string RiverBasedStationNames { get; set; }
        
        public string ReportType { get; set; }
        
        public List<StationBasedRiverMeasureViewModel> StationBasedRiverLevels { get; set; }

        public StationsLevelReportViewModel()
        {
            StationBasedRiverLevels = new List<StationBasedRiverMeasureViewModel>();
        }
    }
}
