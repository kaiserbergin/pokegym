using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PokeGym.Constants;
using PokeGym.Data;
using PokeGymTests.DataTests;
using RestEase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;
using static PokeGym.Controllers.PokeGymController;

namespace PokeGymTests.IntegrationTests
{
    public class IntegrationTests : IDisposable
    {
        private readonly TestServer server;
        private readonly HttpClient client;
        private readonly PokeGymContext context;
        private readonly FluentMockServer fluentMockServer;
        public IntegrationTests()
        {
            server = new TestServer(new WebHostBuilder().UseStartup<TestStartup>());
            client = server.CreateClient();
            context = (PokeGymContext)server.Host.Services.GetService(typeof(PokeGymContext));
            fluentMockServer = (FluentMockServer)server.Host.Services.GetService(typeof(FluentMockServer));
            DataHelper.SeedDatabase(context);
        }

        public void Dispose()
        {
            context.Database.EnsureDeleted();
            context.SaveChanges();
            context.Dispose();
        }

        [Fact]
        public async void GetClassesTest()
        {
            // Arrange
            var expected = await context.Classes.ToListAsync();

            // Act
            var response = await client.GetAsync(RouteConstants.CLASSES_ROUTE);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            var responseObject = JsonConvert.DeserializeObject<List<Class>>(await response.Content.ReadAsStringAsync());

            Assert.Equal(expected.Count, responseObject.Count);
        }

        [Fact]
        public async void GetInstructorsTest()
        {
            // Arrange
            var expected = await context.Instructors.ToListAsync();

            // Act
            var response = await client.GetAsync(RouteConstants.INSTRUCTORS_ROUTE);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            var responseObject = JsonConvert.DeserializeObject<List<Instructor>>(await response.Content.ReadAsStringAsync());

            Assert.Equal(expected.Count, responseObject.Count);
        }

        [Fact]
        public async void GetReservedClassesTest()
        {
            // Arrange
            var trainerId = (await context.Reservations.FirstOrDefaultAsync()).trainerId;
            var expected = await context.Reservations.Where(x => x.trainerId == trainerId).Select(x => x.Class).ToListAsync();

            // Act
            var response = await client.GetAsync(RouteConstants.RESERVATIONS_ROUTE + $"/{trainerId}");

            // Assert
            Assert.Equal(StatusCodes.Status200OK, (int)response.StatusCode);
            var responseObject = JsonConvert.DeserializeObject<List<Class>>(await response.Content.ReadAsStringAsync());

            Assert.Equal(expected.Count, responseObject.Count);
            expected.All(x => responseObject.Any(y => y.ClassId == x.ClassId));
        }

        [Fact]
        public async void AddReservationTest()
        {
            // Arrange
            var trainerId = (await context.Reservations.FirstOrDefaultAsync()).trainerId;
            var beginningReservations = await context.Reservations.Where(x => x.trainerId == trainerId).ToListAsync();

            var content = new AddReservationRequest()
            {
                trainerId = trainerId,
                ClassId = 3
            };
            var requestBodyContent = new StringContent(
                    JsonConvert.SerializeObject(content),
                    Encoding.UTF8,
                    "application/json"
                );

            fluentMockServer
              .Given(
                Request.Create().WithPath($"/trainers/{trainerId}").UsingGet()
              )
              .RespondWith(
                Response.Create()
                  .WithStatusCode(200)
                  .WithHeader("Content-Type", "application/json")
                  .WithBody("[\"Joto\", \"Indigo\"]")
              );

            // Act BuildJSONContent

            var response = await client.PostAsync(RouteConstants.RESERVATIONS_ROUTE, requestBodyContent);

            // Assert
            Assert.Equal(StatusCodes.Status204NoContent, (int)response.StatusCode);

            var updatedReservations = await context.Reservations.Where(x => x.trainerId == trainerId).ToListAsync();

            Assert.Equal(beginningReservations.Count + 1, updatedReservations.Count);
        }

        [Fact]
        public async void AddReservationNotValidTest()
        {
            // Arrange
            var trainerId = (await context.Reservations.FirstOrDefaultAsync()).trainerId;
            var beginningReservations = await context.Reservations.Where(x => x.trainerId == trainerId).ToListAsync();

            var content = new AddReservationRequest()
            {
                trainerId = trainerId,
                ClassId = 3
            };
            var requestBodyContent = new StringContent(
                    JsonConvert.SerializeObject(content),
                    Encoding.UTF8,
                    "application/json"
                );

            fluentMockServer
              .Given(
                Request.Create().WithPath($"/trainers/{trainerId}").UsingGet()
              )
              .RespondWith(
                Response.Create()
                  .WithStatusCode(200)
                  .WithHeader("Content-Type", "application/json")
                  .WithBody("[\"Joto\"]")
              );

            // Act BuildJSONContent

            var response = await client.PostAsync(RouteConstants.RESERVATIONS_ROUTE, requestBodyContent);

            // Assert
            Assert.Equal(412, (int)response.StatusCode);
            var updatedReservations = await context.Reservations.Where(x => x.trainerId == trainerId).ToListAsync();

            Assert.Equal(beginningReservations.Count, updatedReservations.Count);
        }
    }
}
