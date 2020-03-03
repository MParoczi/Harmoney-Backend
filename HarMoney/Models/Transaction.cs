using System;

namespace HarMoney.Models
{
    public class Transaction
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public Frequency Frequency { get; set; }
    }

    public enum Frequency
    {
        Single,
        Weekly,
        Monthly,
        Yearly
    }
}
