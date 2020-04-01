using System;
using System.ComponentModel.DataAnnotations;


namespace HarMoney.Helpers.Validation
{
    public class ValidDateRangeAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            DateTime.TryParse(value.ToString(), out DateTime inputDate);
            if (DateTime.Now.Year + 10 < inputDate.Year)
            {
                return false;
            }
            return true;
        }
    }
}
