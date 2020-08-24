using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EgyptTourism.Models
{


    public class PasswordPolicyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (Regex.Matches((string)value, "[A-Za-z]").Count < 1 || (Regex.Matches((string)value, "[0-9]").Count < 1 || (Regex.Matches((string)value, @"[!@#$%^&*/_+=()]").Count < 1)))
            {
                return new ValidationResult("Password must be contain at least 1 letter, 1 number and 1 symbol (!@#$%^&*/_+=])");
            }
            return ValidationResult.Success;
        }
    }
}