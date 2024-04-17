using System.Text.Json.Serialization;

namespace FoodyAPITesting.Models
{
    public class ApiResponseDTO
    {
        [JsonPropertyName("msg")]
        public string Message { get; set; }


        [JsonPropertyName("foodId")]
        public string? FoodId { get; set; }
    }
}
