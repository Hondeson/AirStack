using System.ComponentModel.DataAnnotations;

namespace AirStack.Client.Validation
{
    public class ComboBoxRequired : ValidationAttribute
    {
        public string GetErrorMessage() => $"Zvolte hodnotu z nabídky!";

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return new ValidationResult(GetErrorMessage());

            return ValidationResult.Success;
        }
    }
}
