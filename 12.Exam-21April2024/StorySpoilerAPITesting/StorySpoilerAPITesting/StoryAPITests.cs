using RestSharp;
using RestSharp.Authenticators;
using StorySpoilerAPITesting.Models;
using System.Net;
using System.Text.Json;

namespace StorySpoilerAPITesting
{
    public class StoryAPITests
    {
        private RestClient client;
        private const string BASE_URL = "https://d5wfqm7y6yb3q.cloudfront.net";
        private const string USERNAME = "Your username here...";
        private const string PASSWORD = "Your password here...";

        private static string lastStoryId;
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
            var authClient = new RestClient(BASE_URL);
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
        public void CreateANewStory_WithCorrectData_ShouldSucceed()
        {
            // Arrange
            var newStory = new StoryDTO
            {
                Title = "New Title",
                Description = "Description of the New Story"
            };

            var request = new RestRequest("/api/Story/Create");
            request.AddJsonBody(newStory);

            // Act
            var response = client.Execute(request, Method.Post);
            var responseData = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
            Assert.That(response.Content, Does.Contain(responseData.StoryId));
            Assert.That(responseData.Message, Is.EqualTo("Successfully created!"));

            lastStoryId = responseData.StoryId;
        }

        [Test, Order(2)]
        public void EditAStory_WithCorrectData_ShouldSucceed()
        {
            // Arrange
            var editStory = new StoryDTO
            {
                Title = "New Title EDITED",
                Description = "Description of the New Story EDITED"
            };

            var request = new RestRequest($"/api/Story/Edit/{lastStoryId}");
            request.AddJsonBody(editStory);

            // Act
            var response = client.Execute(request, Method.Put);
            var responseData = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseData.Message, Is.EqualTo("Successfully edited"));
        }

        [Test, Order(3)]
        public void DeleteAStory_WithValidId_ShouldSucceed()
        {
            // Arrange
            var request = new RestRequest($"/api/Story/Delete/{lastStoryId}");

            // Act
            var response = client.Execute(request, Method.Delete);
            var responseData = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(responseData.Message, Is.EqualTo("Deleted successfully!"));
        }

        [Test, Order(4)]
        public void CreateANewStory_WithInvalidData_ShouldFail()
        {
            // Arrange
            var newInvalidStory = new StoryDTO
            {
                Title = "New Title"
            };

            var request = new RestRequest("/api/Story/Create");
            request.AddJsonBody(newInvalidStory);

            // Act
            var response = client.Execute(request, Method.Post);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test, Order(5)]
        public void EditAStory_WithInvalidId_ShouldFail()
        {
            // Arrange
            var editStory = new StoryDTO
            {
                Title = "New Title EDITED",
                Description = "Description of the New Story EDITED"
            };

            var request = new RestRequest($"/api/Story/Edit/11111");
            request.AddJsonBody(editStory);

            // Act
            var response = client.Execute(request, Method.Put);
            var responseData = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(responseData.Message, Is.EqualTo("No spoilers..."));
        }

        [Test, Order(6)]
        public void DeleteAStory_WithInvalidId_ShouldFail()
        {
            // Arrange
            var request = new RestRequest($"/api/Story/Delete/12356789");

            // Act
            var response = client.Execute(request, Method.Delete);
            var responseData = JsonSerializer.Deserialize<ApiResponseDTO>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(responseData.Message, Is.EqualTo("Unable to delete this story spoiler!"));
        }
    }
}