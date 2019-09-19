using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PokeGym.Clients;
using PokeGym.Controllers;
using PokeGym.Data;
using System;
using WireMock.Server;

namespace PokeGymTests.IntegrationTests
{
    public class TestStartup
    {
        public IConfiguration Configuration { get; }
        public FluentMockServer fluentMockServer;

        public TestStartup(IConfiguration configuration)
        {
            Configuration = configuration;
            fluentMockServer = FluentMockServer.Start();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlite()
                .BuildServiceProvider();

            

            var connection = new SqliteConnection("Datasource=:memory:");
            connection.Open();

            services.AddDbContext<PokeGymContext>(options =>
            {
                options.UseSqlite(connection);
                options.UseInternalServiceProvider(serviceProvider);
            });

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var pokeGymContext = scopedServices.GetRequiredService<PokeGymContext>();
                pokeGymContext.Database.EnsureCreated();
            }

            services.AddScoped<PokeGymRepository>();
            var settings = new PokedexClientSettings() { baseUrl = new Uri(fluentMockServer.Urls[0]) };
            services.AddSingleton(settings);
            services.AddHttpClient<PokeDexClient>();
            services.AddSingleton(fluentMockServer);

            services.AddControllers()
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddApplicationPart(typeof(PokeGymController).Assembly);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
