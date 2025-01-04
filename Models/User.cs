using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ms2.Models
{
    public class User
    {
        public int Id { get; set; } // Unique identifier for the user
        public string Name { get; set; } // Name of the user
        public string Email { get; set; } // Email address
        public string Phone { get; set; } // Contact number
        public string Address { get; set; } // Address (optional)
        public DateTime CreatedOn { get; set; } = DateTime.Now; // Date the user was added
    }

}
