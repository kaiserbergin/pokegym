using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PokeGym.Constants;
using PokeGym.Data;
using PokeGymTests.DataTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xunit;
using static PokeGym.Controllers.PokeGymController;

namespace PokeGymTests.IntegrationTests
{
    public class IntegrationTests : IDisposable
    {
        private readonly TestServer server;
        private readonly HttpClient client;
        private readonly PokeGymContext context;
        public IntegrationTests()
        {
            server = new TestServer(new WebHostBuilder().UseStartup<TestStartup>());
            client = server.CreateClient();
            context = (PokeGymContext)server.Host.Services.GetService(typeof(PokeGymContext));
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
            var studentId = (await context.Reservations.FirstOrDefaultAsync()).StudentId;
            var expected = await context.Reservations.Where(x => x.StudentId == studentId).Select(x => x.Class).ToListAsync();

            // Act
            var response = await client.GetAsync(RouteConstants.RESERVATIONS_ROUTE + $"/{studentId}");

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
            var studentId = (await context.Reservations.FirstOrDefaultAsync()).StudentId;
            var beginningReservations = await context.Reservations.Where(x => x.StudentId == studentId).ToListAsync();

            var content = new AddReservationRequest()
            {
                StudentId = studentId,
                ClassId = 3
            };
            var requestBodyContent = new StringContent(
                    JsonConvert.SerializeObject(content),
                    Encoding.UTF8,
                    "application/json"
                );

            // Act BuildJSONContent

            var response = await client.PostAsync(RouteConstants.RESERVATIONS_ROUTE, requestBodyContent);

            // Assert
            Assert.Equal(StatusCodes.Status204NoContent, (int)response.StatusCode);
            var responseObject = JsonConvert.DeserializeObject<List<Class>>(await response.Content.ReadAsStringAsync());

            var updatedReservations = await context.Reservations.Where(x => x.StudentId == studentId).ToListAsync();

            Assert.Equal(beginningReservations.Count + 1, updatedReservations.Count);
        }
    }
}
