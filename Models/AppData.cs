using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ms2.Models
{
    public class AppData
    {
        public List<User> Users { get; set; } = new();
        public List<Transaction> Transactions { get; set; } = new List<Transaction>();
        public List<Debt> Debts { get; set; } = new List<Debt>();
        public decimal MainBalance { get; set; }
    }
}
