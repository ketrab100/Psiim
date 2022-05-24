using System;
using System.Collections.Generic;

namespace psiim.Models
{
    public partial class PeopleDatum
    {
        public PeopleDatum()
        {
            Admins = new HashSet<Admin>();
            Clients = new HashSet<Client>();
        }

        public int PersonDataId { get; set; }
        public string FirstName { get; set; } = null!;
        public string SecondName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public string Login { get; set; } = null!;
        public string HashPassword { get; set; } = null!;

        public virtual ICollection<Admin> Admins { get; set; }
        public virtual ICollection<Client> Clients { get; set; }
    }
}
