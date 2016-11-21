using System.Collections.Generic;
using Newtonsoft.Json;

namespace Report.Models.Domain.Base
{
    public class Measure
    {
        [JsonProperty("@id")]
        public string Id { get; set; }
        public string DatumType { get; set; }
        public string Label { get; set; }
        public Reading LatestReading { get; set; }
        public string Notation { get; set; }
        public string Parameter { get; set; }
        public string ParameterName { get; set; }
        public decimal Period { get; set; }
        public string Qualifier { get; set; }
        public Station Station { get; set; }
        public string StationReference { get; set; }
        public List<string> Type { get; set; }
        public string Unit { get; set; }
        public string UnitName { get; set; }
        public string ValueType { get; set; }
    }
}
