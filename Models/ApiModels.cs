

namespace Will_Website.Models
{
    public class ApiPaymentRequest
    {
        public decimal Amount { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Description { get; set; }
        public string? ItemName { get; set; }
        public string BaseUrl { get; set; } = "";
        public string ClientType { get; set; } = "web";
    }

    public class ApiPaymentResponse
    {
        public bool Success { get; set; }
        public string OrderId { get; set; } = "";
        public string PaymentUrl { get; set; } = "";
        public Dictionary<string, string> PaymentData { get; set; } = new();
        public string Message { get; set; } = "";
    }

    public class PaymentStatusResponse
    {
        public string OrderId { get; set; } = "";
        public string Status { get; set; } = "";
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
    }
}