using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokeGym.Data
{
    public class PokeGymRepository
    {
        private readonly PokeGymContext context;

        public PokeGymRepository(PokeGymContext context)
        {
            this.context = context;
        }

        public async Task<List<Class>> GetClassesAsync() => await context.Classes.Include(x => x.Instructor).ToListAsync();

        public async Task<List<Instructor>> GetInstructorsAsync() => await context.Instructors.ToListAsync();

        public async Task<List<Reservation>> GetReservationsAsync(int trainerId) => await context.Reservations.Where(x => x.TrainerId == trainerId).ToListAsync();

        public async Task CreateReservationAsync(int trainerId, int classId)
        {
            await context.Reservations.AddAsync(new Reservation { ClassId = classId, TrainerId = trainerId });
            await context.SaveChangesAsync();
        }

        public async Task<List<Class>> GetReservedClassesAsync(int trainerId)
        {
            var reservations = await context.Reservations
                .Where(x => x.TrainerId == trainerId)
                .Include(x => x.Class)
                .ThenInclude(x => x.Instructor).ToListAsync();

            return reservations.Select(x => x.Class).ToList();
        }
    }
}
