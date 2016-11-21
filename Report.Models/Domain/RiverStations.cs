using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Report.Models.Domain.Base;

namespace Report.Models.Domain
{
    public class RiverStations
    {
        [JsonProperty("@context")]
        public string Context { get; set; }

        [JsonProperty("meta")]
        public Meta MetaData { get; set; }

        [JsonProperty("items")]
        public List<Station> Stations { get; set; }
    }
}
