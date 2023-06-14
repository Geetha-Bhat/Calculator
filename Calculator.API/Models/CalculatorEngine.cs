using Calculator.Models;

namespace Calculator.API.Models
{
    public interface ICalculatorEngine
    {
        CalculatorResponse Calculate(CalculatorRequest request);
    }
    public class CalculatorEngine : ICalculatorEngine
    {
        private static Func<decimal, decimal, decimal> Add => (num1, num2) => num1 + num2;
        private static Func<decimal, decimal, decimal> Subtract => (num1, num2) => num1 - num2;
        private static Func<decimal, decimal, decimal> Multiply => (num1, num2) => num1 * num2;
        private static Func<decimal, decimal, decimal> Divide => (num1, num2) => num1 / num2;

        public CalculatorResponse Calculate(CalculatorRequest request)
        {
            // Not validating the request here - assuming the controller is validating the request before calling

            decimal result = 0;
            var num1 = request.Num1.GetValueOrDefault();
            var num2 = request.Num2.GetValueOrDefault();

            switch (request.Operator)
            {
                case '+':
                    result =  Add(num1, num2);
                    break;
                case '-':
                    result = Subtract(num1, num2);
                    break;
                case '*':
                    result = Multiply(num1, num2);
                    break;
                case '/':
                    result = Divide(num1, num2);
                    break;
            }

            return new CalculatorResponse{Num1 = num1, Num2 = num2, Operator = request.Operator.GetValueOrDefault(), Result = result};
        }
    }
}
