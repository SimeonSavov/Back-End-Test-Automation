using System.Text.Json.Serialization;

namespace RestSharpCountryAPI
{
    public class Place
    {
        [JsonPropertyName("place name")]
        public string PlaceName { get; set; }
        public string State { get; set; }
        public string StateAbbreviation { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}