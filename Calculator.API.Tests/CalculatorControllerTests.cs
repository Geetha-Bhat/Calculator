using Calculator.API.Controllers;
using Calculator.API.Models;
using Calculator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace Calculator.API.Tests
{
    public class CalculatorControllerTests
    {
        private Mock<ILogger<CalculatorController>> _mockLogger;
        private ICalculatorEngine _calculatorEngine;
        private Mock<ICacheService> _mockCacheService;
        private CalculatorController _sut; // system under test
        [SetUp]
        public void Setup()
        {
            _mockLogger = new Mock<ILogger<CalculatorController>>();
            _mockCacheService = new Mock<ICacheService>();
            _calculatorEngine = new CalculatorEngine();

            _sut = new CalculatorController(_mockLogger.Object, _calculatorEngine, _mockCacheService.Object);
        }

        [Test]
        [TestCase(10, 5, 15)]
        [TestCase(0, 0, 0)]
        [TestCase(0, 5, 5)]
        [TestCase(5, 0, 5)]
        [TestCase(-15, 5, -10)]
        [TestCase(10.25, 5.45, 15.70)]
        [TestCase(0.0, 0.0, 0)]
        public void AddCalculationTests(decimal num1, decimal num2, decimal expected)
        {
            char op = '+';
            var request = new CalculatorRequest { Num1 = num1, Num2 = num2, Operator = op};

            var actual = _sut.Post(request);

            Assert.That(actual.Value, Is.TypeOf<CalculatorResponse>());
            Assert.That(actual.Value, Is.Not.Null);
            Assert.That(actual.Value.Result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(10, 5, 5)]
        [TestCase(0, 0, 0)]
        [TestCase(0, 5, -5)]
        [TestCase(5, 0, 5)]
        [TestCase(-15, 5, -20)]
        [TestCase(10.25, 5.45, 4.80)]
        [TestCase(0.0, 0.0, 0)]
        public void SubtractCalculationTests(decimal num1, decimal num2, decimal expected)
        {
            char op = '-';

            var request = new CalculatorRequest { Num1 = num1, Num2 = num2, Operator = op };

            var actual = _sut.Post(request);

            Assert.That(actual.Value, Is.TypeOf<CalculatorResponse>());
            Assert.That(actual.Value, Is.Not.Null);
            Assert.That(actual.Value.Result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(10, 5, 50)]
        [TestCase(0, 0, 0)]
        [TestCase(0, 5, 0)]
        [TestCase(5, 0, 0)]
        [TestCase(-15, 5, -75)]
        [TestCase(10.25, 5.45, 55.8625)]
        [TestCase(0.0, 0.0, 0)]
        public void MultiplyCalculationTests(decimal num1, decimal num2, decimal expected)
        {
            char op = '*';

            var request = new CalculatorRequest { Num1 = num1, Num2 = num2, Operator = op };

            var actual = _sut.Post(request);

            Assert.That(actual.Value, Is.TypeOf<CalculatorResponse>());
            Assert.That(actual.Value, Is.Not.Null);
            Assert.That(actual.Value.Result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(10, 5, 2)]
        [TestCase(0, 5, 0)]
        
        [TestCase(-15, 5, -3)]
        [TestCase(10.5, 2.5, 4.2)]
        
        public void DivideCalculationTests(decimal num1, decimal num2, decimal expected)
        {
            char op = '/';

            var request = new CalculatorRequest { Num1 = num1, Num2 = num2, Operator = op };

            var actual = _sut.Post(request);

            Assert.That(actual.Value, Is.TypeOf<CalculatorResponse>());
            Assert.That(actual.Value, Is.Not.Null);
            Assert.That(actual.Value.Result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(5, 0)]
        [TestCase(0.0, 0.0)]
        public void DivideByZeroTests(decimal num1, decimal num2)
        {
            char op = '/';

            var request = new CalculatorRequest { Num1 = num1, Num2 = num2, Operator = op };
            SimulateValidation(request);

            var actual = _sut.Post(request);

            Assert.That(actual.Result is BadRequestObjectResult, Is.True);
        }

        [Test]
        [TestCaseSource(nameof(TestValues))]
        public void WhenOverflowDataTypeTests(decimal num1, decimal num2, char op)
        {
            var request = new CalculatorRequest { Num1 = num1 , Num2 = num2, Operator = op };

            var actual = _sut.Post(request);

            Assert.That(actual.Result is StatusCodeResult, Is.True);
        }

        private static object[] TestValues =
        {
            new object[]{ decimal.MinValue, decimal.MinValue, '+' },
            new object[]{ decimal.MaxValue, decimal.MaxValue, '+' }
        };

        [Test]
        [TestCase(null, null, null)]
        [TestCase(10.5, null, null)]
        [TestCase(10.5, 5.5, null)]
        [TestCase(10.5, null, '+')]
        [TestCase(null, 5.5, '+')]
        [TestCase(null, 5.5, null)]
        [TestCase(null, null, '+')]
        [TestCase(10.5, 5.5, '%')]
        public void InvalidRequestTests(decimal? num1, decimal? num2, char? op)
        {
            var request = new CalculatorRequest { Num1 = num1, Num2 = num2, Operator = op };
            SimulateValidation(request);

            var actual = _sut.Post(request);

            Assert.That(actual.Result is BadRequestObjectResult, Is.True);
        }

        private void SimulateValidation(object model)
        {
            // mimic the behaviour of the model binder which is responsible for Validating the Model
            var validationContext = new ValidationContext(model, null, null);
            var validationResults = new List<ValidationResult>();
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            foreach (var validationResult in validationResults)
            {
                _sut.ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
            }
        }
    }
}