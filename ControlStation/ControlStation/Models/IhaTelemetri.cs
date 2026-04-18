using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace ControlStation.Models
{
    public class IhaTelemetri
    {
        [JsonPropertyName("takim_numarasi")]
        public int TakimNumarasi { get; set; }

        [JsonPropertyName("iha_enlem")]
        public double IhaEnlem { get; set; }

        [JsonPropertyName("iha_boylam")]
        public double IhaBoylam { get; set; }

        [JsonPropertyName("iha_irtifa")]
        public double IhaIrtifa { get; set; }

        [JsonPropertyName("iha_dikilme")]
        public double IhaDikilme { get; set; } 

        [JsonPropertyName("iha_yonelme")]
        public double IhaYonelme { get; set; } 

        [JsonPropertyName("iha_yatis")]
        public double IhaYatis { get; set; } 

        [JsonPropertyName("iha_hiz")]
        public double IhaHiz { get; set; }

        [JsonPropertyName("iha_batarya")]
        public int IhaBatarya { get; set; }

        [JsonPropertyName("iha_otonom")]
        public int IhaOtonom { get; set; } 

        [JsonPropertyName("iha_kilitlenme")]
        public int IhaKilitlenme { get; set; } 

        [JsonPropertyName("hedef_merkez_X")]
        public int HedefMerkezX { get; set; }

        [JsonPropertyName("hedef_merkez_Y")]
        public int HedefMerkezY { get; set; }

        [JsonPropertyName("hedef_genislik")]
        public int HedefGenislik { get; set; }

        [JsonPropertyName("hedef_yukseklik")]
        public int HedefYukseklik { get; set; }

        [JsonPropertyName("gps_saati")]
        public GpsSaati GpsSaati { get; set; }
    }
}
