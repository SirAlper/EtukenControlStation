using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ControlStation.Models
{
    public class GpsSaati
    {
        [JsonPropertyName("saat")]
        public int Saat { get; set; }

        [JsonPropertyName("dakika")]
        public int Dakika { get; set; }

        [JsonPropertyName("saniye")]
        public int Saniye { get; set; }

        [JsonPropertyName("milisaniye")]
        public int Milisaniye { get; set; }
    }
}
