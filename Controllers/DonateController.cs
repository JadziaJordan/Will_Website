using Microsoft.AspNetCore.Mvc;
using Will_Website.Models;
using Will_Website.Services;
using System.Text;
using System.Text.Json;

namespace Will_Website.Controllers
{
    public class DonateController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IConfiguration _configuration;

        public DonateController(
            ApiService apiService,
            IConfiguration configuration)
        {
            _apiService = apiService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Donate(decimal amount, string name, string email, string message)
        {
            try
            {
                var baseUrl = $"{Request.Scheme}://{Request.Host}";
                
                var paymentRequest = new ApiPaymentRequest
                {
                    Amount = amount,
                    Name = name,
                    Email = email,
                    Description = message ?? $"Donation from {name}",
                    ItemName = $"Donation by {name}",
                    BaseUrl = baseUrl,
                    ClientType = "web"
                };

                var response = await _apiService.InitiatePaymentAsync(paymentRequest);

                if (response.Success)
                {
                    // Generate HTML form to auto-submit to PayFast
                    var htmlForm = GenerateAutoSubmitForm(response.PaymentUrl, response.PaymentData);
                    return Content(htmlForm, "text/html");
                }
                else
                {
                    ViewBag.Error = response.Message;
                    return View("Index");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"An error occurred: {ex.Message}";
                return View("Index");
            }
        }

        private string GenerateAutoSubmitForm(string paymentUrl, Dictionary<string, string> paymentData)
        {
            var formFields = paymentData.Select(kv => 
                $"<input type='hidden' name='{kv.Key}' value='{kv.Value}' />")
                .ToList();

            return $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Redirecting to PayFast...</title>
    <style>
        body {{
            font-family: Arial, sans-serif;
            text-align: center;
            padding: 50px;
            background-color: #f8f9fa;
        }}
        .loading {{
            margin: 20px 0;
        }}
        .spinner {{
            border: 3px solid #f3f3f3;
            border-top: 3px solid #28a745;
            border-radius: 50%;
            width: 30px;
            height: 30px;
            animation: spin 1s linear infinite;
            margin: 0 auto;
        }}
        @keyframes spin {{
            0% {{ transform: rotate(0deg); }}
            100% {{ transform: rotate(360deg); }}
        }}
    </style>
    <script type='text/javascript'>
        window.onload = function() {{
            document.getElementById('payfast_form').submit();
        }};
    </script>
</head>
<body>
    <h2>Redirecting you to PayFast...</h2>
    <div class='loading'>
        <div class='spinner'></div>
        <p>Please wait while we redirect you to complete your donation...</p>
    </div>
    <p>If you're not redirected automatically, <a href='#' onclick='document.getElementById(""payfast_form"").submit();'>click here</a>.</p>
    
    <form id='payfast_form' action='{paymentUrl}' method='POST'>
        {string.Join("\n        ", formFields)}
        <input type='submit' value='Pay Now' style='display:none;' />
    </form>
</body>
</html>";
        }

        [HttpGet("donate/return")]
        public IActionResult Return([FromQuery] string orderId)
        {
            if (!string.IsNullOrEmpty(orderId))
            {
                ViewBag.OrderId = orderId;
            }
            return View("Success");
        }

        [HttpGet("donate/cancel")]
        public IActionResult Cancel([FromQuery] string orderId)
        {
            if (!string.IsNullOrEmpty(orderId))
            {
                ViewBag.OrderId = orderId;
            }
            return View("Cancel");
        }

        // This endpoint is no longer needed since your API handles notifications
        // But keeping it for backward compatibility in case PayFast sends notifications here
        [HttpPost("donate/notify")]
        public IActionResult Notify()
        {
            // Just return OK - your API handles the real notifications
            return Ok();
        }
    }
}