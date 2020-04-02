using System;
using System.ComponentModel.DataAnnotations;
using HarMoney.Helpers.Validation;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace HarMoney.Models
{
    public class Transaction
    {
        public long Id { get; set; }

        [Required]
        [StringLength(30, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 1)]
        public string Title { get; set; }

        [Required]
        [ValidDateRange(ErrorMessage = "You shouldn't plan 10 years ahead")]
        public DateTime? DueDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "The amount must be between 1 and 2147483647")]
        public int Amount { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        [EnumDataType(typeof(Frequency))]
        public Frequency? Frequency { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        [EnumDataType(typeof(Direction))]
        public Direction? Direction { get; set; }

        [Required]
        [JsonConverter(typeof(StringEnumConverter))]
        [EnumDataType(typeof(Category))]
        public Category? Category { get; set; }
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
        Extra,
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
