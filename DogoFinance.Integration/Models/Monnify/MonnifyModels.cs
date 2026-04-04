namespace DogoFinance.Integration.Models.Monnify
{
    public class MonnifySettings
    {
        public string BaseUrl { get; set; } = null!;
        public string ApiKey { get; set; } = null!;
        public string SecretKey { get; set; } = null!;
        public string ContractCode { get; set; } = null!;
    }

    public class InitializeTransactionRequest
    {
        public decimal Amount { get; set; }
        public string CustomerName { get; set; } = null!;
        public string CustomerEmail { get; set; } = null!;
        public string PaymentReference { get; set; } = null!;
        public string PaymentDescription { get; set; } = null!;
        public string CurrencyCode { get; set; } = "NGN";
        public string ContractCode { get; set; } = null!;
        public string RedirectUrl { get; set; } = null!;
        public List<string> PaymentMethods { get; set; } = new () { "CARD", "ACCOUNT_TRANSFER", "USSD", "PHONE_NUMBER" };
    }

    public class InitializeTransactionResponse
    {
        public bool RequestSuccessful { get; set; }
        public string ResponseMessage { get; set; } = null!;
        public string ResponseCode { get; set; } = null!;
        public ResponseBody ResponseBody { get; set; } = null!;
    }

    public class ResponseBody
    {
        public string TransactionReference { get; set; } = null!;
        public string PaymentReference { get; set; } = null!;
        public string MerchantName { get; set; } = null!;
        public string CheckoutUrl { get; set; } = null!;
    }

    public class SingleTransferRequest
    {
        public decimal Amount { get; set; }
        public string Reference { get; set; } = null!;
        public string Narration { get; set; } = null!;
        public string DestinationBankCode { get; set; } = null!;
        public string DestinationAccountNumber { get; set; } = null!;
        public string Currency { get; set; } = "NGN";
        public string SourceAccountNumber { get; set; } = null!; // Monnify Wallet ID
    }

    public class SingleTransferResponse
    {
        public bool RequestSuccessful { get; set; }
        public string ResponseMessage { get; set; } = null!;
        public string ResponseCode { get; set; } = null!;
        public TransferResponseBody ResponseBody { get; set; } = null!;
    }

    public class TransferResponseBody
    {
        public string TransferReference { get; set; } = null!;
        public string Status { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Reference { get; set; } = null!;
    }
}
