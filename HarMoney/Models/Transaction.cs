using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HarMoney.Models
{
    public class Transaction
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public int Amount { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Frequency Frequency { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Direction Direction { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Category Category { get; set; }
    }

    public enum Frequency
    {
        Single,
        Weekly,
        Monthly,
        Yearly
    }

    public enum Direction
    {
        Income,
        Expenditure
    }

    public enum Category
    {
        Education,
        Entertainment,
        Groceries,
        Health,
        Household,
        Insurance,
        Investment,
        Kids,
        Other,
        Pets,
        Sport,
        Transportation,
        Gift,
        Heritage,
        Increment,
        Prize,
        Salary
    }
}
