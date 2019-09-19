using PokeGym.Data;
using System;
using System.Collections.Generic;

namespace PokeGymTests.DataTests
{
    public class DataHelper
    {
        public static void SeedDatabase(PokeGymContext context)
        {
            var instructorBrock = new Instructor
            {
                FirstName = "Brock",
                LastName = "Takeshi",
                GymRank = "Gym Leader"
            };

            var instructorMisty = new Instructor
            {
                FirstName = "Misty",
                LastName = "Kasumi",
                GymRank = "Gym Leader"
            };


            var rockOutClass = new Class
            {
                ClassId = 1,
                Name = "Rocking Out",
                Description = "Electric guitar lessons for all ages.",
                Instructor = instructorBrock,
                Reservations = new List<Reservation>()
                {
                    new Reservation() { StudentId = 1 },
                    new Reservation() { StudentId = 2 },
                    new Reservation() { StudentId = 3 }
                }
            };

            var swimClass = new Class
            {
                ClassId = 2,
                Name = "Aquatic Adventures",
                Description = "Swimming lessons for children.",
                Instructor = instructorMisty,
                Reservations = new List<Reservation>()
                {
                    new Reservation() { StudentId = 1 },
                    new Reservation() { StudentId = 2 },
                    new Reservation() { StudentId = 3 },
                    new Reservation() { StudentId = 4 }
                }
            };

            var introToPokemonBattles = new Class
            {
                ClassId = 3,
                Name = "Casting the First Stone",
                Description = "Introductory class for Pokemon battles.",
                Instructor = instructorBrock,
                Reservations = new List<Reservation>()
                {
                    new Reservation() { StudentId = 5 },
                    new Reservation() { StudentId = 2 },
                    new Reservation() { StudentId = 3 },
                    new Reservation() { StudentId = 6 },
                    new Reservation() { StudentId = 7 },
                    new Reservation() { StudentId = 8 }
                }
            };

            context.Classes.AddRange(new List<Class> { rockOutClass, swimClass, introToPokemonBattles });
            context.SaveChanges();
        }
    }
}
