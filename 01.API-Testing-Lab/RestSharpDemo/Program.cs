using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;
using RestSharpDemo;

// First steps with RestSharp
//var client = new RestClient("https://api.github.com");
//var request = new RestRequest("/users/softuni/repos", Method.Get);

//var response = client.Execute(request);

//Console.WriteLine(response.StatusCode);
//Console.WriteLine(response.Content);


// RestSharp Segments
//var client = new RestClient("https://api.github.com");
//var request = new RestRequest("/repos/{user}/{repo}/issues/{id}", Method.Get);

//request.AddUrlSegment("user", "testnakov");
//request.AddUrlSegment("repo", "test-nakov-repo");
//request.AddUrlSegment("id", 1);

//var response = client.Execute(request);

//Console.WriteLine(response.StatusCode);
//Console.WriteLine(response.Content);


// RestSharp Deserialize responses
//var client = new RestClient("https://api.github.com");
//var request = new RestRequest("/users/softuni/repos", Method.Get);

//var response = client.Execute(request);

//var repoObj = JsonConvert.DeserializeObject<List<Repo>>(response.Content);


// GitHub Authentication demo - Method POST
//var client = new RestClient(new RestClientOptions("https://api.github.com")
//{
//    Authenticator = new HttpBasicAuthenticator("SimeonSavov", "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")
//});

//var request = new RestRequest
//("/repos/testnakov/test-nakov-repo/issues", Method.Post);

//request.AddHeader("Content-Type", "application/json");

//request.AddJsonBody(new { title = "New Testing Title Issue", body = "New Testing Body from Simeon" });

//var response = client.Execute(request);

//Console.WriteLine(response.StatusCode);
