using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ms2.Models
{ 
public class User {
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public DateTime CreatedOn { get; set; } = DateTime.Now; // Date the user was added
    }
}