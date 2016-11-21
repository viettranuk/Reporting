using System;
using Newtonsoft.Json;
using Report.Models.Domain.Base;

namespace Report.Models.Domain.Base
{
    public class Reading
    {
        [JsonProperty("@id")]
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public DateTime DateTime { get; set; }
        public Measure Measure { get; set; }
        public decimal Value { get; set; }
    }
}
