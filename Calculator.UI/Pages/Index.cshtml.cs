using Calculator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Calculator.UI.Pages
{
    [BindProperties]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public IndexModel(ILogger<IndexModel> logger,
            HttpClient httpClient,
            IConfiguration configuration)
        {
            _logger = logger;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        public decimal Num1 { get; set; }
        public decimal Num2 { get; set; }
        public char Operator { get; set; }
        public decimal? Result { get; set; }
        public bool HasApiError { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostCalculate()
        {
            // By default AntiforgeryToken is turned on in .Net Core apps - if not enable this for security
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Need to remove the Result from Model state - else UI is not updated
            ModelState.Remove(nameof(Result));
            ModelState.Remove(nameof(HasApiError));

            var request = new CalculatorRequest { Num1 = Num1, Num2 = Num2, Operator = Operator };
            var response = await _httpClient.PostAsJsonAsync(ApiUrl, request);
            if (!response.IsSuccessStatusCode)
            {
                HasApiError = true;
                return Page();
            }
            var calcResponse = await  response.Content.ReadFromJsonAsync<CalculatorResponse>();
            if (calcResponse == null) return Page();
            Num1 = calcResponse.Num1;
            Num2 = calcResponse.Num2;
            Operator = calcResponse.Operator;
            Result = calcResponse.Result;

            return Page();
        }

        public void OnPostClear()
        {
            Num1 = Num2 = default;
            Operator = default;
            Result = null;
            HasApiError = false;
            ModelState.Clear();
            RedirectToPage("Index");
        }

        private string ApiUrl => _configuration.GetValue<string>("ApiUrl");
    }
}