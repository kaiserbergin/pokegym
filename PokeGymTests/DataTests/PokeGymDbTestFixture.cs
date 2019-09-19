using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PokeGym.Data;
using PokeGymTests.DataTests;
using System;
using System.Collections.Generic;

namespace PokeGymTests
{
    public class PokeGymDbTestFixture : IDisposable
    {
        public readonly SqliteConnection Connection;
        public readonly PokeGymContext Context;
        public readonly PokeGymRepository Repository;

        public PokeGymDbTestFixture()
        {
            Connection = new SqliteConnection("Datasource=:memory:");
            Connection.Open();

            var options = new DbContextOptionsBuilder<PokeGymContext>()
                .UseSqlite(Connection)
                .Options;

            Context = new PokeGymContext(options);
            Context.Database.EnsureCreated();

            DataHelper.SeedDatabase(Context);

            Repository = new PokeGymRepository(Context);
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.SaveChanges();
            Context.Dispose();
            Connection.Close();
        }

        
    }
}
