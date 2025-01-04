namespace ms2.Models
{
    public class Debt
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingDebt => Amount - PaidAmount;  // Automatically calculate remaining debt
        public DateTime Date { get; set; }
        public string Description { get; set; }

        // Public setter for IsCleared property
        public bool IsCleared { get; set; }  // Use this property to mark if the debt is paid/cleared
    }
}
