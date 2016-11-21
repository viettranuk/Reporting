using System.Collections.Generic;
using Newtonsoft.Json;
using Report.Models.Domain.Base;

namespace Report.Models.Domain
{
    public class StationReadings
    {
        [JsonProperty("@context")]
        public string Context { get; set; }

        [JsonProperty("meta")]
        public Meta MetaData { get; set; }

        [JsonProperty("items")]
        public List<Reading> Readings { get; set; }
    }
}
