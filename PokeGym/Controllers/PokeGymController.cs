using Microsoft.AspNetCore.Mvc;
using PokeGym.Clients;
using PokeGym.Constants;
using PokeGym.Data;
using System.Threading.Tasks;

namespace PokeGym.Controllers
{
    [ApiController]
    public class PokeGymController
    {
        private readonly PokeGymRepository pokeGymRepository;
        private readonly PokeDexClient pokeDexClient;
        public PokeGymController(PokeGymRepository pokeGymRepository, PokeDexClient pokeDexClient)
        {
            this.pokeGymRepository = pokeGymRepository;
            this.pokeDexClient = pokeDexClient;
        }

        [HttpGet(RouteConstants.CLASSES_ROUTE)]
        public async Task<IActionResult> GetClasses()
        {
            var classes = await pokeGymRepository.GetClassesAsync();
            return new OkObjectResult(classes);
        }

        [HttpGet(RouteConstants.INSTRUCTORS_ROUTE)]
        public async Task<IActionResult> GetInstructors()
        {
            var instructors = await pokeGymRepository.GetInstructorsAsync();
            return new OkObjectResult(instructors);
        }

        [HttpGet(RouteConstants.RESERVATIONS_ROUTE + @"/{trainerId}")]
        public async Task<IActionResult> GetReservedClasses(int trainerId)
        {
            var reservedCasses = await pokeGymRepository.GetReservedClassesAsync(trainerId);
            return new OkObjectResult(reservedCasses);
        }

        [HttpPost(RouteConstants.RESERVATIONS_ROUTE)]
        public async Task<IActionResult> AddReservation([FromBody]AddReservationRequest addReservationRequest)
        {
            var registeredLeagues = await pokeDexClient.GetRegisteredLeaguesForTrainer(addReservationRequest.trainerId);
            if (!registeredLeagues.Contains("Indigo"))
                return new StatusCodeResult(412);

            await pokeGymRepository.CreateReservationAsync(addReservationRequest.trainerId, addReservationRequest.ClassId);
            return new NoContentResult();
        }

        
    }
}
