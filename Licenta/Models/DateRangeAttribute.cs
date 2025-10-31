using System.ComponentModel.DataAnnotations;

namespace Licenta.Models
{
    public class DateRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null) return ValidationResult.Success;
            var date = (DateTime)value;
            var minDate = new DateTime(2025, 1, 1);
            var maxDate = new DateTime(2100, 1, 1);
            if (date < minDate || date > maxDate)
                return new ValidationResult("Date must be between 01/01/2025 and 01/01/2100.");
            return ValidationResult.Success;
        }
    }
}