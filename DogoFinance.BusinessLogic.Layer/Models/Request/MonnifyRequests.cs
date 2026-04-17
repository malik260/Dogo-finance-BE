namespace DogoFinance.BusinessLogic.Layer.Models.Request
{
    public class MonnifyChargeRequest
    {
        public string Reference { get; set; } = null!;
        public string CardNumber { get; set; } = null!;
        public string ExpiryMonth { get; set; } = null!;
        public string ExpiryYear { get; set; } = null!;
        public string CVV { get; set; } = null!;
        public string? Pin { get; set; }
    }

    public class MonnifyAuthorizeRequest
    {
        public string Reference { get; set; } = null!;
        public string Id { get; set; } = null!;
        public string Otp { get; set; } = null!;
    }
}
