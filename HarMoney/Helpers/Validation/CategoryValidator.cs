using System.Collections.Generic;
using HarMoney.Models;


namespace HarMoney.Helpers.Validation
{
    public class CategoryValidator
    {
        private List<Category?> _incomes = new List<Category?>()
        {
            Category.Extra, Category.Gift, Category.Heritage, Category.Increment,
            Category.Other, Category.Prize, Category.Salary
        };

        private List<Category?> _expenditures = new List<Category?>()
        {
            Category.Education, Category.Entertainment, Category.Groceries,
            Category.Health, Category.Household, Category.Insurance, Category.Investment,
            Category.Kids, Category.Other, Category.Pets, Category.Sport, Category.Transportation
        };
        public bool CategoryIsValid(Transaction transaction)
        {
            Category? category = transaction.Category;
            if (category != null && transaction.Direction == Direction.Income)
            {
                return _incomes.Contains(category);
            }

            if (category != null && transaction.Direction == Direction.Expenditure)
            {
                return _expenditures.Contains(category);
            }

            return false;
        }

        public string Message(Direction? direction)
        {
            string incomeMessage = "Income category must be Extra, Gift, Heritage, Increment, " +
                                   "Other, Prize or Salary.";
            string expenditureMessage = "Expenditure category must be Education, Entertainment, Groceries, " +
                                        "Health, Household, Insurance, Investment, Kids, Other, " +
                                        "Pets, Sport, Transportation.";

            if (direction == Direction.Income)
            {
                return incomeMessage;
            }

            if (direction == Direction.Expenditure)
            {
                return expenditureMessage;
            }

            return "Direction is required and it must be either Income or Expenditure.";
        }
    }
}
