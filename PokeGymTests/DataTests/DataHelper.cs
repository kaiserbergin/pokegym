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
                    new Reservation() { trainerId = 1 },
                    new Reservation() { trainerId = 2 },
                    new Reservation() { trainerId = 3 }
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
                    new Reservation() { trainerId = 1 },
                    new Reservation() { trainerId = 2 },
                    new Reservation() { trainerId = 3 },
                    new Reservation() { trainerId = 4 }
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
                    new Reservation() { trainerId = 5 },
                    new Reservation() { trainerId = 2 },
                    new Reservation() { trainerId = 3 },
                    new Reservation() { trainerId = 6 },
                    new Reservation() { trainerId = 7 },
                    new Reservation() { trainerId = 8 }
                }
            };

            context.Classes.AddRange(new List<Class> { rockOutClass, swimClass, introToPokemonBattles });
            context.SaveChanges();
        }
    }
}
