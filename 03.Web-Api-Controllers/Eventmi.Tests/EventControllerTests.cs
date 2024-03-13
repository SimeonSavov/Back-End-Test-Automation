using Eventmi.Core.Models.Event;
using Eventmi.Infrastructure.Data.Contexts;
using Eventmi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System.Net;

namespace Eventmi.Tests
{
    public class Tests
    {
        private RestClient? _client;
        private string _baseUrl = "https://localhost:7236";

        [SetUp]
        public void Setup()
        {
            _client = new RestClient(_baseUrl);
        }

        [Test]
        public async Task GetAllEvents_ReturnsSuccessStatusCode()
        {
            // Arrange
            var request = new RestRequest("/Event/All");

            // Act
            var response = await _client.ExecuteAsync(request, Method.Get);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Add_GetRequest_ReturnsAddView()
        {
            // Arrange
            var request = new RestRequest("/Event/Add");

            // Act
            var response = await _client.ExecuteAsync(request, Method.Get);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task Add_PostRequest_AddsEventAndRedirects()
        {
            // Arrange
            var newEvent = new EventFormModel
            {
                Name = "BuiltByBarz",
                Start = new DateTime(2024, 03, 12, 19, 0, 0),
                End = new DateTime(2024, 03, 12, 22, 0, 0),
                Place = "Shumen"
            };

            var request = new RestRequest("/Event/Add");

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("Name", newEvent.Name);
            request.AddParameter("Start", newEvent.Start
                .ToString("MM/dd/yyyy hh:mm tt"));
            request.AddParameter("End", newEvent.End
                .ToString("MM/dd/yyyy hh:mm tt"));
            request.AddParameter("Place", newEvent.Place);

            // Act
            var response = await _client.ExecuteAsync(request, Method.Post);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(CheckIfEventExists(newEvent.Name), "Event was not added to the Database.");
        }

        [Test]
        public async Task GetEventDetails_ReturnsSuccessAndExpectedConten()
        {
            // Arrange
            int eventId = 1;
            var request = new RestRequest($"/Event/Details/{eventId}");

            // Act
            var response = await _client.ExecuteAsync(request, Method.Get);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task GetEventDetails_ReturnsNotFoundIfNoIdIsGiven()
        {
            // Arrange
            var request = new RestRequest($"/Event/Details/");

            // Act
            var response = await _client.ExecuteAsync(request, Method.Get);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task EditAction_ReturnsViewForValidId()
        {
            // Arrange
            int eventId = 5;
            var request = new RestRequest($"/Event/Edit/{eventId}");

            // Act
            var response = await _client.ExecuteAsync(request, Method.Get);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task EditAction_ReturnsNotFoundIfNoIdIsGiven()
        {
            // Arrange
            var request = new RestRequest($"/Event/Edit/");

            // Act
            var response = await _client.ExecuteAsync(request, Method.Get);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task Edit_PostRequest_ReturnsSuccessAndViewWithUpdatedEvent()
        {
            // Arrange
            int eventId = 10;
            var dbEvent = await GetEventByIdAsync(eventId);

            var inputEvent = new EventFormModel()
            {
                Id = dbEvent.Id,
                Name = $"{dbEvent.Name} UPDATED",
                Start = dbEvent.Start,
                End = dbEvent.End,
                Place = dbEvent.Place
            };

            var request = new RestRequest($"/Event/Edit/{dbEvent.Id}");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("Id", inputEvent.Id);
            request.AddParameter("Name", inputEvent.Name);
            request.AddParameter("Start", inputEvent.Start
                .ToString("MM/dd/yyyy hh:mm tt"));
            request.AddParameter("End", inputEvent.End
                .ToString("MM/dd/yyyy hh:mm tt"));
            request.AddParameter("Place", inputEvent.Place);

            // Act
            var response = await _client.ExecuteAsync(request, Method.Post);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updatedDbEvent = await GetEventByIdAsync(eventId);
            Assert.That(updatedDbEvent.Name, Is.EqualTo(inputEvent.Name));
        }

        [Test]
        public async Task EditPostAction_WithIdMismatch_ReturnsNotFound()
        {
            // Arrange
            int eventId = 10;
            var dbEvent = await GetEventByIdAsync(eventId);

            var inputEvent = new EventFormModel()
            {
                Id = 446,
                Name = $"{dbEvent.Name} UPDATED",
                Start = dbEvent.Start,
                End = dbEvent.End,
                Place = dbEvent.Place
            };


            var request = new RestRequest($"/Event/Edit/{eventId}");

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("Id", inputEvent.Id);
            request.AddParameter("Name",inputEvent.Name);
            request.AddParameter("Start", inputEvent.Start
                .ToString("MM/dd/yyyy hh:mm tt"));
            request.AddParameter("End", inputEvent.End
                .ToString("MM/dd/yyyy hh:mm tt"));
            request.AddParameter("Place", inputEvent.Place);

            // Act
            var respone = await _client.ExecuteAsync(request, Method.Post);

            // Assert
            Assert.That(respone.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task EditPostAction_WithInvalidModel_RetunrsViewWithModel()
        {
            // Arrange
            int eventId = 10;
            var dbEvent = await GetEventByIdAsync(eventId);

            var inputEvent = new EventFormModel()
            {
                Id = dbEvent.Id,
                Place = dbEvent.Place
            };


            var request = new RestRequest($"/Event/Edit/{eventId}");

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("Id", inputEvent.Id);
            request.AddParameter("Name", inputEvent.Name);

            // Act
            var respone = await _client.ExecuteAsync(request, Method.Post);

            // Assert
            Assert.That(respone.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public async Task DeleteAction_WithValidId_RedirectsToAllEvents()
        {
            // Arrange
            var newEvent = new EventFormModel
            {
                Name = "Test Event For Deleting",
                Start = new DateTime(2024, 03, 12, 19, 0, 0),
                End = new DateTime(2024, 03, 12, 22, 0, 0),
                Place = "Sofia"
            };

            var request = new RestRequest("/Event/Add");

            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("Name", newEvent.Name);
            request.AddParameter("Start", newEvent.Start
                .ToString("MM/dd/yyyy hh:mm tt"));
            request.AddParameter("End", newEvent.End
                .ToString("MM/dd/yyyy hh:mm tt"));
            request.AddParameter("Place", newEvent.Place);

            await _client.ExecuteAsync(request, Method.Post);

            var eventInDb = GetEventByName(newEvent.Name);
            var eventIdToDelete = eventInDb.Id;

            var deletedRequest = new RestRequest($"/Event/Delete/{eventIdToDelete}");

            // Act
            var response = await _client.ExecuteAsync(deletedRequest, Method.Post);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }


        private bool CheckIfEventExists(string eventName)
        {
            // Define your DbContext options
            var options = new DbContextOptionsBuilder<EventmiContext>()
                .UseSqlServer("Server=SIMEON-PC\\SQLEXPRESS;Database=Eventmi;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            // Instantiate the context with the options
            using (var context = new EventmiContext(options))
            {
                // Perform the check
                return context.Events.Any(x => x.Name == eventName);
            }
        }

        private Event? GetEventByName(string eventName)
        {
            // Define your DbContext options
            var options = new DbContextOptionsBuilder<EventmiContext>()
                .UseSqlServer("Server=SIMEON-PC\\SQLEXPRESS;Database=Eventmi;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            // Instantiate the context with the options
            using (var context = new EventmiContext(options))
            {
                // Perform the check
                return context.Events.FirstOrDefault(x => x.Name == eventName);
            }
        }

        private async Task<Event> GetEventByIdAsync(int id)
        {
            var options = new DbContextOptionsBuilder<EventmiContext>()
                .UseSqlServer("Server=SIMEON-PC\\SQLEXPRESS;Database=Eventmi;Trusted_Connection=True;MultipleActiveResultSets=true")
                .Options;

            using (var context = new EventmiContext(options))
            {
                return await context.Events.FirstOrDefaultAsync(x => x.Id == id);
            }
        }
    }

    
}