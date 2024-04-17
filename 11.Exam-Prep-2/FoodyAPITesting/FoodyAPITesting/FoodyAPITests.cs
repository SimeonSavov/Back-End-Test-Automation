using FoodyAPITesting.Models;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;
using System.Text.Json;

namespace FoodyAPITesting
{
    public class FoodyAPITests
    {
        private RestClient client;
        private const string BASE_URL = "http://softuni-qa-loadbalancer-2137572849.eu-north-1.elb.amazonaws.com:86";
        private const string USERNAME = "Your Username Here...";
        private const string PASSWORD = "Your Password Here...";

        private static string foodId;

        [OneTimeSetUp]
        public void Setup()
        {
            string jwtToken = GetJwtToken(USERNAME, PASSWORD);

            var options = new RestClientOptions(BASE_URL)
            {
                Authenticator = new JwtAuthenticator(jwtToken)
            };

            client = new RestClient(options);
        }

        private string GetJwtToken(string username, string password)
        {
            RestClient authClient = new RestClient(BASE_URL);
            var request = new RestRequest("/api/User/Authentication");
            request.AddJsonBody(new AuthenticationRequest
            {
                Username = username,
                Password = password
            });

            var response = authClient.Execute(request, Method.Post);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = JsonSerializer.Deserialize<AuthenticationResponse>(response.Content);

                var token = content.AccessToken;

                if (string.IsNullOrWhiteSpace(token))
                {
                    throw new InvalidOperationException("Token is null or white space");
                }

                return token;
            }
            else
            {
                throw new InvalidOperationException("Authentication failed!");
            }
        }

        [Test, Order(1)]
        public void CreateANewFood_WithRquiredFields_ShouldSucceed()
        {
            // Arrange
            var newFood = new FoodDTO
            {
                Name = "NewFood",
                Description = "Description for the NewFood"
            };

            var request = new RestRequest("/api/Food/Create");
            request.AddJsonBody(newFood);

            // Act
            var response = client.Execute(request, Method.Post);
            var data = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response.Content, Does.Contain(data.FoodId));

            foodId = data.FoodId;
        }

        [Test, Order(2)]
        public void EditFood_WithNeedTitle_ShouldSucceed()
        {
            // Arrange
            var request = new RestRequest($"api/Food/Edit/{foodId}");

            request.AddJsonBody(new[]
            {
                new
                {
                    path = "/name",
                    op = "replace",
                    value = "New Food Edited Title"
                },
            });

            // Act
            var response = client.Execute(request, Method.Patch);
            var data = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(data.Message, Is.EqualTo("Successfully edited"));
        }

        [Test, Order(3)]
        public void GetAllFoods_ShouldReturnAnArrayOfItems()
        {
            // Arrange
            var request = new RestRequest("/api/Food/All");

            // Act
            var response = client.Execute(request, Method.Get);
            var data = JsonSerializer.Deserialize<List<ApiResponseDTO>>(response.Content);

            // Assert
            Assert.IsNotEmpty(data);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test, Order(4)]
        public void DeleteTheFood_WithCorrectId_ShouldSucceed()
        {
            // Arrange
            var request = new RestRequest($"/api/Food/Delete/{foodId}");

            // Act
            var response = client.Execute(request, Method.Delete);
            var data = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(data.Message, Is.EqualTo("Deleted successfully!"));
        }

        [Test, Order(5)]
        public void CreateAFood_WithIncorrectData_ShouldFail()
        {
            // Arrange
            var newFood = new FoodDTO
            {
                Name = "NewFood",
            };

            var request = new RestRequest("/api/Food/Create");
            request.AddJsonBody(newFood);

            // Act
            var response = client.Execute(request, Method.Post);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test, Order(6)]
        public void EditFood_WithInvalidId_ShouldFail()
        {
            // Arrange
            var request = new RestRequest("api/Food/Edit/XXXXXX");

            request.AddJsonBody(new[]
            {
                new
                {
                    path = "/name",
                    op = "replace",
                    value = "New Food Edited Title"
                },
            });

            // Act
            var response = client.Execute(request, Method.Patch);
            var data = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(data.Message, Is.EqualTo("No food revues..."));
        }

        [Test, Order(7)]
        public void DeleteAFood_WithInvalidId_ShouldFail()
        {
            // Arrange
            var request = new RestRequest("/api/Food/Delete/INVALIDID");

            // Act
            var response = client.Execute(request, Method.Delete);
            var data = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(data.Message, Is.EqualTo("No food revues..."));
        }
    }
}