using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PokeGym.Clients
{
    public class PokeDexClient
    {
        private readonly HttpClient httpClient;

        public PokeDexClient(HttpClient httpClient, PokedexClientSettings pokedexClientSettings)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = pokedexClientSettings.baseUrl;
        }

        public async Task<List<string>> GetRegisteredLeaguesForTrainer(int trainerId)
        {
            var response = await httpClient.GetAsync($"/trainers/{trainerId}"); 
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<string>>(content);
        }
    }
}
