using System;
using System.Collections.Generic;

namespace psiim.Models
{
    public partial class Table
    {
        public Table()
        {
            ReservedTables = new HashSet<ReservedTable>();
        }

        public int TableId { get; set; }
        public int ClubId { get; set; }
        public int Number { get; set; }
        public string Type { get; set; } = null!;
        public double Price { get; set; }

        public virtual Club Club { get; set; } = null!;
        public virtual ICollection<ReservedTable> ReservedTables { get; set; }
    }
}
