namespace ms2.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public DateTime Date { get; set; }
        public List<string> Tags { get; set; }
        public string Description { get; set; }
        public string DebtClearedNotes { get; set; }
    } 
}
