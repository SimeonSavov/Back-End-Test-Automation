using ApiTestingLab;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;

//WeatherForecast weatherForecast = new WeatherForecast()
//{
//    Date = DateTime.Now,
//    TemperatureC = 32,
//    Summary = "New random summary"
//};

//// This is how to serialize with Newtonsoft.json
//string weatherForecastJson = JsonConvert.SerializeObject(weatherForecast, Formatting.Indented);

//Console.WriteLine(weatherForecastJson);


// This is how to deserialize with Newtonsoft.json
//string jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory) + "/../../../People.json");

//// Anonymous object
//var person = new
//{
//    FirstName = string.Empty,
//    LastName = string.Empty,
//    JobTitle = string.Empty
//};

//var peopleObj = JsonConvert.DeserializeAnonymousType(jsonString, person);


// Deserialize WeatherForecast
//string jsonString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory) + "/../../../WeatherForecast.json");

//var weatherForecastObject = JsonConvert.DeserializeObject<List<WeatherForecast>>(jsonString);

//WeatherForecast weatherForecast = new WeatherForecast()
//{
//    Date = DateTime.Now,
//    TemperatureC = 32,
//    Summary = "New random summary"
//};

//DefaultContractResolver contractResolver =
//new DefaultContractResolver()
//{
//    NamingStrategy = new SnakeCaseNamingStrategy()
//};
//var serialized = JsonConvert.SerializeObject(weatherForecast, new JsonSerializerSettings()
//{
//    ContractResolver = contractResolver,
//    Formatting = Formatting.Indented
//});


// JSON with LINQ
//string jsonPeopleString = File.ReadAllText(Path.Combine(Environment.CurrentDirectory) + "/../../../People.json");

//var person = JObject.Parse(jsonPeopleString);

//Console.WriteLine(person["firstName"]);
//Console.WriteLine(person["lastName"]);


// JObject query with LINQ
var json = JObject.Parse(@"{'products': [
{'name': 'Fruits', 'products': ['apple', 'banana']},
{'name': 'Vegetables', 'products': ['cucumber']}]}");

var products = json["products"]
    .Select(t => string.Format("{0} ({1})", t["name"], string.Join(", ", t["products"])));