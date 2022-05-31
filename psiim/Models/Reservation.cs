using System;
using System.Collections.Generic;

namespace psiim.Models
{
    public partial class Reservation
    {
        public Reservation()
        {
            ReservedTables = new HashSet<ReservedTable>();
        }

        public int ReservationId { get; set; }
        public int ClientId { get; set; }
        public DateTime Date { get; set; }
        public double Cost { get; set; }
        public bool IsAccepted { get; set; }
        public int Duration { get; set; }

        public virtual Client Client { get; set; } = null!;
        public virtual ICollection<ReservedTable> ReservedTables { get; set; }
    }
}
