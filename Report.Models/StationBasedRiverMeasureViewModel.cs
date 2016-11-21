using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Report.Models
{
    public class StationBasedRiverMeasureViewModel
    {
        public string StationName { get; set; }
        public decimal MinValue { get; set; }
        public DateTime MinValueDate { get; set; }
        public decimal MaxValue { get; set; }
        public DateTime MaxValueDate { get; set; }
        public decimal AverageValue { get; set; }
    }
}
