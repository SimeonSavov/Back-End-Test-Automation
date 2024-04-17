using System.Text.Json.Serialization;

namespace FoodyAPITesting.Models
{
    public class FoodDTO
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("url")]
        public string URL { get; set; }
    }
}
