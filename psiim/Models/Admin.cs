using System;
using System.Collections.Generic;

namespace psiim.Models
{
    public partial class Admin
    {
        public int AdminId { get; set; }
        public int ClubId { get; set; }
        public int PersonDataId { get; set; }

        public virtual Club Club { get; set; } = null!;
        public virtual PeopleDatum PersonData { get; set; } = null!;
    }
}
