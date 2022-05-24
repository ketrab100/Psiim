using System;
using System.Collections.Generic;

namespace psiim.Models
{
    public partial class Client
    {
        public Client()
        {
            Reservations = new HashSet<Reservation>();
        }

        public int ClientId { get; set; }
        public int PersonDataId { get; set; }

        public virtual PeopleDatum PersonData { get; set; } = null!;
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
