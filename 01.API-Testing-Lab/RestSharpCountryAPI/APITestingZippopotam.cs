using Newtonsoft.Json;
using RestSharp;

namespace RestSharpCountryAPI
{
    public class RestSharpAPIZippopotam
    {
        [SetUp]
        public void Setup()
        {
        }

        [TestCase("BG", "1000", "Sofija")]
        [TestCase("BG", "5000", "Veliko Turnovo")]
        [TestCase("Ca", "M5S", "Toronto")]
        [TestCase("GB", "B1", "Birmingham")]
        [TestCase("DE", "01067", "Dresden")]
        public void Test_GetMethod_Zippopotam(string countryCode, string zipCode, string expectedPlace)
        {
            // Arrange
            var restClient = new RestClient("https://api.zippopotam.us");
            var httpRequest = new RestRequest(countryCode + "/" + zipCode, Method.Get);

            // Act
            var httpResponse = restClient.Execute(httpRequest);
            var location = JsonConvert.DeserializeObject<Location>(httpResponse.Content);

            // Assert
            Assert.NotNull(location);
            Assert.NotNull(location.Places);
            Assert.IsNotEmpty(location.Places);
        }
    }
}