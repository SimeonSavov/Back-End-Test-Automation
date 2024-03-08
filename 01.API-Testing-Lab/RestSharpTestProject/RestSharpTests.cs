using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using System.Net;

namespace RestSharpTestProject
{
    public class GitHubAPITests
    {
        private RestClient _client;

        [SetUp]
        public void Setup()
        {
            var options = new RestClientOptions("https://api.github.com")
            {
                Authenticator = new HttpBasicAuthenticator("SimeonSavov", "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")
            };

            _client = new RestClient(options);
        }

        [Test]
        public void Test_GitHubAPI_GetAllIssues()
        {
            // Arrange
            var request = new RestRequest("/repos/testnakov/test-nakov-repo/issues", Method.Get);

            // Act
            var response = _client.Execute(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void Test_GitHubAPI_GetAllIssues_Deserialize()
        {
            // Arrange
            var request = new RestRequest("/repos/testnakov/test-nakov-repo/issues", Method.Get);

            // Act
            var response = _client.Execute(request);
            var issuesObj = JsonConvert.DeserializeObject<List<Issue>>(response.Content);

            // Assert
            Assert.That(issuesObj.Count > 1);

            foreach (var issue in issuesObj)
            {
                Assert.That(issue.id, Is.GreaterThan(0));
                Assert.That(issue.number, Is.GreaterThan(0));
                Assert.That(issue.title, Is.Not.Empty);
            }
        }


        private Issue CreateIssue(string title, string body)
        {
            var request = new RestRequest("/repos/testnakov/test-nakov-repo/issues", Method.Post);
            request.AddBody(new { body, title });
            var response = _client.Execute(request);
            var issuesObj = JsonConvert.DeserializeObject<Issue>(response.Content);

            return issuesObj;
        }

        [Test]
        public void Test_GitHubAPI_CreateGitHubIssue()
        {
            // Arrange
            string title = "This is title from VS";
            string body = "This is custom body from VS";

            // Act
            var createdIssue = CreateIssue(title, body);

            // Assert
            Assert.That(createdIssue.id, Is.GreaterThan(0));
            Assert.That(createdIssue.number, Is.GreaterThan(0));
            Assert.That(createdIssue.title, Is.Not.Empty);  
            Assert.That(createdIssue.body, Is.Not.Empty);
        }

        [Test]
        public void Test_GitHubAPI_EditGitHubExistingIssue()
        {
            // Arrange
            var request = new RestRequest("repos/testnakov/test-nakov-repo/issues/5057", Method.Patch);
            request.AddBody(new
            {
                title = "UPDATED Title with PATCH method"
            });

            // Act
            var response = _client.Execute(request);
            var issueObject = JsonConvert.DeserializeObject<Issue>(response.Content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(issueObject.id, Is.GreaterThan(0), "Id Should be greather than 0.");
            Assert.That(response.Content, Is.Not.Empty, "The response content should not be empty.");
            Assert.That(issueObject.number, Is.GreaterThan(0), "Issue number should be greather than 0.");
            Assert.That(issueObject.title, Is.EqualTo("UPDATED Title with PATCH method"), "Updated title that I created.");
        }
    }
}