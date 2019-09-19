using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PokeGym.Data;
using System;
using System.Linq;
using Xunit;

namespace PokeGymTests
{
    public class PokeGymRepositoryTests : IClassFixture<PokeGymDbTestFixture>
    {
        private readonly PokeGymDbTestFixture fixture;

        public PokeGymRepositoryTests(PokeGymDbTestFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        public async void GetInstructorsAsyncTest()
        {
            // Arrange
            var expected = await fixture.Context.Instructors.ToListAsync();

            // Act
            var actual = await fixture.Repository.GetInstructorsAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void GetClassesAsyncTest()
        {
            // Arrange
            var expected = await fixture.Context.Classes.ToListAsync();

            // Act
            var actual = await fixture.Repository.GetClassesAsync();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async void GetReservationsAsyncTest()
        {
            // Arrange
            var studentId = (await fixture.Context.Reservations.FirstOrDefaultAsync()).StudentId;
            var expected = await fixture.Context.Reservations.Where(x => x.StudentId == studentId).ToListAsync();

            // Act
            var actual = await fixture.Repository.GetReservationsAsync(studentId);

            // Assert
            Assert.Equal(expected, actual);
            Assert.True(actual.All(x => x.StudentId == studentId));
        }

        [Fact]
        public async void CreateReservationAsyncTest()
        {
            // Arrange
            var studentId = 99;
            var reservedClass = await fixture.Context.Classes.FirstOrDefaultAsync();
            var originalReservationCount = fixture.Context.Reservations.Count();

            // Act
            await fixture.Repository.CreateReservationAsync(studentId, reservedClass.ClassId);

            // Assert
            Assert.Equal(originalReservationCount + 1, fixture.Context.Reservations.Count());
            Assert.Equal(fixture.Context.Reservations.Where(x => x.StudentId == studentId).FirstOrDefault().Class, reservedClass);
        }
    }
}
