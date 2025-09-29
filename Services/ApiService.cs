using System.Text;
using System.Text.Json;
using Will_Website.Models;

namespace Will_Website.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl;

        public ApiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiBaseUrl = configuration["PayFastApi:BaseUrl"] ?? "https://your-api-url.com";
        }

        public async Task<ApiPaymentResponse> InitiatePaymentAsync(ApiPaymentRequest request)
        {
            try
            {
                var json = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/payment/initiate", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiPaymentResponse>(responseJson, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    return result ?? new ApiPaymentResponse { Success = false, Message = "Invalid response from API" };
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return new ApiPaymentResponse 
                    { 
                        Success = false, 
                        Message = $"API Error: {response.StatusCode} - {errorContent}" 
                    };
                }
            }
            catch (Exception ex)
            {
                return new ApiPaymentResponse 
                { 
                    Success = false, 
                    Message = $"Error calling API: {ex.Message}" 
                };
            }
        }

        public async Task<PaymentStatusResponse> GetPaymentStatusAsync(string orderId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}/api/payment/status/{orderId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<PaymentStatusResponse>(responseJson, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });
                    
                    return result ?? new PaymentStatusResponse();
                }
                else
                {
                    return new PaymentStatusResponse();
                }
            }
            catch (Exception)
            {
                return new PaymentStatusResponse();
            }
        }
    }
}