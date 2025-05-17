using System.Reflection;
using System.Text.Json.Serialization;

namespace CarRenter.Models
{
    public class NowyModelDlaEdycjiAuta
    {
        public int id { get; set; }
        public string marka { get; set; }
        [JsonPropertyName("model")]
        public string nazwaModel { get; set; }
        public int rok { get; set; }
        public int kwotaZaDzien { get; set; }
        public string opis { get; set; }
        public string urlObrazka { get; set; }

    }

}

