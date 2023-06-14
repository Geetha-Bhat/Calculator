using System.ComponentModel.DataAnnotations;

namespace Calculator.Models
{
    public class CalculatorRequest : IValidatableObject
    {
        [Required]
        public decimal? Num1 { get; set; }
        [Required]
        public decimal? Num2 { get; set; }
        [Required]
        public char? Operator { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Operator is '/' && Num2 is 0)
                yield return new ValidationResult("Invalid operation and values.", new List<string>{nameof(Num2)});

            if (Operator.HasValue && !OperatorList.Contains(Operator.Value))
                yield return new ValidationResult("Invalid operation", new List<string> {nameof(Operator) });
        }

        public static char[] OperatorList => new[] { '+', '-', '*', '/' };
    }

    public class CalculatorResponse 
    {

        
        public decimal Num1 { get; set; }
        public decimal Num2 { get; set; }
        public char Operator { get; set; }
        public decimal Result { get; set; }
    }
}
