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
        public decimal amount { get; set; }
        public string customerName { get; set; } = null!;
        public string customerEmail { get; set; } = null!;
        public string paymentReference { get; set; } = null!;
        public string paymentDescription { get; set; } = null!;
        public string currencyCode { get; set; } = "NGN";
        public string contractCode { get; set; } = null!;
        public string redirectUrl { get; set; } = null!;
        public List<string> paymentMethods { get; set; } = new () { "CARD", "ACCOUNT_TRANSFER", "USSD", "PHONE_NUMBER" };
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

    public class CardChargeRequest
    {
        public string transactionReference { get; set; } = null!;
        public CardDetails card { get; set; } = null!;
    }

    public class CardDetails
    {
        public string number { get; set; } = null!;
        public string expiryMonth { get; set; } = null!;
        public string expiryYear { get; set; } = null!;
        public string cvv { get; set; } = null!;
        public string? pin { get; set; }
    }

    public class CardChargeResponse
    {
        public bool RequestSuccessful { get; set; }
        public string ResponseMessage { get; set; } = null!;
        public string ResponseCode { get; set; } = null!;
        public CardChargeResponseBody ResponseBody { get; set; } = null!;
    }

    public class CardChargeResponseBody
    {
        public string Status { get; set; } = null!;
        public string Message { get; set; } = null!;
        public OtpData? OtpData { get; set; }
        public string TransactionReference { get; set; } = null!;
        public string PaymentReference { get; set; } = null!;
        public decimal AuthorizedAmount { get; set; }
    }

    public class OtpData
    {
        public string? Id { get; set; }
        public string? Message { get; set; }
        public string? AuthData { get; set; }
    }

    public class AuthorizeOtpRequest
    {
        public string transactionReference { get; set; } = null!;
        public string tokenId { get; set; } = null!;
        public string token { get; set; } = null!;
    }

    public class TransactionStatusResponse
    {
        public bool RequestSuccessful { get; set; }
        public TransactionStatusBody ResponseBody { get; set; } = null!;
    }

    public class TransactionStatusBody
    {
        public string TransactionReference { get; set; } = null!;
        public string PaymentReference { get; set; } = null!;
        public decimal AmountPaid { get; set; }
        public string PaymentStatus { get; set; } = null!;
        public string PaymentMethod { get; set; } = null!;
    }

    public class CreateReservedAccountRequest
    {
        public string accountReference { get; set; } = null!;
        public string accountName { get; set; } = null!;
        public string currencyCode { get; set; } = "NGN";
        public string contractCode { get; set; } = null!;
        public string customerEmail { get; set; } = null!;
        public string customerName { get; set; } = null!;
        public bool getAllAvailableBanks { get; set; } = true;
        public string? customerBvn { get; set; }
    }

    public class ReservedAccountResponse
    {
        public bool requestSuccessful { get; set; }
        public string responseMessage { get; set; } = null!;
        public string responseCode { get; set; } = null!;
        public ReservedAccountResponseBody responseBody { get; set; } = null!;
    }

    public class ReservedAccountResponseBody
    {
        public string contractCode { get; set; } = null!;
        public string accountReference { get; set; } = null!;
        public string accountName { get; set; } = null!;
        public string customerEmail { get; set; } = null!;
        public string customerName { get; set; } = null!;
        public List<ReservedAccountDetail> accounts { get; set; } = null!;
    }

    public class ReservedAccountDetail
    {
        public string bankCode { get; set; } = null!;
        public string bankName { get; set; } = null!;
        public string accountNumber { get; set; } = null!;
    }

    public class MonnifyWebhookPayload
    {
        public string eventType { get; set; } = null!;
        public WebhookEventData eventData { get; set; } = null!;
    }

    public class WebhookEventData
    {
        // For Deposits
        public string paymentReference { get; set; } = null!;
        public string accountReference { get; set; } = null!;
        public decimal amountPaid { get; set; }
        public string paymentStatus { get; set; } = null!;

        // For Disbursements
        public string reference { get; set; } = null!;
        public decimal amount { get; set; }
    }

    public class BvnMatchRequest
    {
        public string bvn { get; set; } = null!;
        public string name { get; set; } = null!;
        public string dateOfBirth { get; set; } = null!;
        public string mobileNo { get; set; } = null!;
    }

    public class BvnMatchResponse
    {
        public bool requestSuccessful { get; set; }
        public string responseMessage { get; set; } = null!;
        public BvnMatchResponseBody responseBody { get; set; } = null!;
    }

    public class BvnMatchResponseBody
    {
        public string bvn { get; set; } = null!;
        public NameMatch name { get; set; } = null!;
        public bool dateOfBirth { get; set; }
        public bool mobileNo { get; set; }
    }

    public class NameMatch
    {
        public bool match { get; set; }
        public double score { get; set; }
    }

    public class NinVerifyRequest
    {
        public string nin { get; set; } = null!;
    }

    public class NinVerifyResponse
    {
        public bool requestSuccessful { get; set; }
        public string responseMessage { get; set; } = null!;
        public NinVerifyResponseBody responseBody { get; set; } = null!;
    }

    public class NinVerifyResponseBody
    {
        public string firstname { get; set; } = null!;
        public string lastname { get; set; } = null!;
        public string middlename { get; set; } = null!;
        public string birthdate { get; set; } = null!;
        public string gender { get; set; } = null!;
        public string mobileNumber { get; set; } = null!;
    }
}
