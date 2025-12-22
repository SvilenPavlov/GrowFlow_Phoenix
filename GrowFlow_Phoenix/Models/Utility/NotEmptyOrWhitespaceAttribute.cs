using System.ComponentModel.DataAnnotations;

namespace GrowFlow_Phoenix.Models.Utility
{
    public class NotEmptyOrWhitespaceAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string s && string.IsNullOrWhiteSpace(s))
            {
                return new ValidationResult($"{validationContext.MemberName} cannot be empty or whitespace.");
            }
            return ValidationResult.Success;
        }
    }
}
