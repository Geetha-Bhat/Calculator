using Calculator.API.Models;
using Calculator.Models;
using Microsoft.AspNetCore.Mvc;

namespace Calculator.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculatorController : ControllerBase
    {
        private readonly ILogger<CalculatorController> _logger;
        private readonly ICalculatorEngine _calculatorEngine;
        private readonly ICacheService _cacheService;
        public CalculatorController(ILogger<CalculatorController> logger,
            ICalculatorEngine calculatorEngine,
            ICacheService cacheService)
        {
            _logger = logger;
            _calculatorEngine = calculatorEngine;
            _cacheService = cacheService;
        }

        [HttpPost]
        public ActionResult<CalculatorResponse> Post([FromBody] CalculatorRequest request)
        {
            // Do not trust incoming request - always validate
            // For ease have used Data Annotations for validation - Can use Fluent Validation library for bigger and complex models
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Calculate
            try
            {
                var cacheKey = $"{request.Num1.GetValueOrDefault()}-{request.Num2.GetValueOrDefault()}-{request.Operator.GetValueOrDefault()}";
                // check if the calculation is already solved?
                var item = _cacheService.Get<CalculatorResponse>(cacheKey);
                if (item != null)
                    return Ok(item);

                // not solved yet
                var response = _calculatorEngine.Calculate(request);

                // add to cache
                _cacheService.Set(cacheKey, response);

                // return the response
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "When doing calculation", request);
                return StatusCode(500);
            }
            
        }
    }
}
