using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Report.Models.Domain.Base
{
    public class Station
    {
        [JsonProperty("@id")]
        public string Id { get; set; }
        
        public string RloiId { get; set; }
        public string CatchmentName { get; set; }
        public DateTime DateOpened { get; set; }
        public int Easting { get; set; }
        public string GridReference { get; set; }
        public string Label { get; set; }

        [JsonProperty("lat")]
        public decimal Latitude { get; set; }

        [JsonProperty("long")]
        public decimal Longitude { get; set; }

        public List<Measure> Measures { get; set; }
        public int Northing { get; set; }
        public string Notation { get; set; }
        public string RiverName { get; set; }
        public string StageScale { get; set; }
        public string StationReference { get; set; }
        public string Status { get; set; }
        public string Town { get; set; }
        public string WiskiID { get; set; }
    }
}
