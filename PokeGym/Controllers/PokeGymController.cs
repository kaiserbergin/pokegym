using Microsoft.AspNetCore.Mvc;
using PokeGym.Constants;
using PokeGym.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokeGym.Controllers
{
    [ApiController]
    public class PokeGymController
    {
        private readonly PokeGymRepository pokeGymRepository;
        public PokeGymController(PokeGymRepository pokeGymRepository)
        {
            this.pokeGymRepository = pokeGymRepository;
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

        [HttpGet(RouteConstants.RESERVATIONS_ROUTE + @"/{studentId}")]
        public async Task<IActionResult> GetReservedClasses(int studentId)
        {
            var reservedCasses = await pokeGymRepository.GetReservedClassesAsync(studentId);
            return new OkObjectResult(reservedCasses);
        }

        [HttpPost(RouteConstants.RESERVATIONS_ROUTE)]
        public async Task<IActionResult> AddReservation([FromBody]AddReservationRequest addReservationRequest)
        {
            await pokeGymRepository.CreateReservationAsync(addReservationRequest.StudentId, addReservationRequest.ClassId);
            return new NoContentResult();
        }

        
    }
}
