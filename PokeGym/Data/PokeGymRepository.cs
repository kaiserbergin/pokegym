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

        public async Task<List<Reservation>> GetReservationsAsync(int studentId) => await context.Reservations.Where(x => x.StudentId == studentId).ToListAsync();

        public async Task CreateReservationAsync(int studentId, int classId)
        {
            await context.Reservations.AddAsync(new Reservation { ClassId = classId, StudentId = studentId });
            await context.SaveChangesAsync();
        }

        public async Task<List<Class>> GetReservedClassesAsync(int studentId)
        {
            var reservations = await context.Reservations
                .Where(x => x.StudentId == studentId)
                .Include(x => x.Class)
                .ThenInclude(x => x.Instructor).ToListAsync();

            return reservations.Select(x => x.Class).ToList();
        }
    }
}
