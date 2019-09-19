using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PokeGym.Data
{
    public class AddReservationRequest
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
    }
}
