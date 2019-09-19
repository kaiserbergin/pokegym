using Microsoft.EntityFrameworkCore;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace PokeGym.Data
{
    public class PokeGymContext : DbContext
    {
        public DbSet<Class> Classes { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        public PokeGymContext(DbContextOptions<PokeGymContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reservation>()
                .HasKey(x => new { x.trainerId, x.ClassId });
        }
    }

    public class Class
    {
        public int ClassId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Instructor Instructor { get; set; }
        public List<Reservation> Reservations { get; set; }
    }

    public class Instructor
    {
        public int InstructorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GymRank { get; set; }
    }

    public class Reservation
    {
        public int trainerId { get; set; }

        public int ClassId { get; set; }

        [JsonIgnore]
        public Class Class { get; set; }
    }
}
