using System;
using System.Collections.Generic;

namespace psiim.Models
{
    public partial class ReservedTable
    {
        public int ReservedTableId { get; set; }
        public int ReservationId { get; set; }
        public int TableId { get; set; }

        public virtual Reservation Reservation { get; set; } = null!;
        public virtual Table Table { get; set; } = null!;
    }
}
