using System;
using System.Collections.Generic;

namespace psiim.Models
{
    public partial class Club
    {
        public Club()
        {
            Admins = new HashSet<Admin>();
            Tables = new HashSet<Table>();
        }

        public int ClubId { get; set; }
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual ICollection<Admin> Admins { get; set; }
        public virtual ICollection<Table> Tables { get; set; }
    }
}
