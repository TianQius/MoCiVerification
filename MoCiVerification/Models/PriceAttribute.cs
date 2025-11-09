using System.ComponentModel.DataAnnotations;

namespace MoCiVerification.Models;

public class PriceAttribute(double min = 0, double max = 999999.99) : ValidationAttribute
{
    private readonly decimal _min = (decimal)min;
    private readonly decimal _max = (decimal)max;

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null || value is decimal price && price >= _min && price <= _max)
            return ValidationResult.Success;
        
        return new ValidationResult($"价格必须在 {_min} 到 {_max} 之间");
    }
}