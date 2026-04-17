using DogoFinance.Integration.Models.Monnify;

namespace DogoFinance.Integration.Interfaces
{
    public interface IMonnifyService
    {
        Task<InitializeTransactionResponse?> InitializeTransaction(InitializeTransactionRequest request);
        Task<SingleTransferResponse?> SingleTransfer(SingleTransferRequest request);
        Task<CardChargeResponse?> ChargeCard(CardChargeRequest request);
        Task<bool> AuthorizeOtp(AuthorizeOtpRequest request);
        Task<ReservedAccountResponse?> CreateReservedAccount(CreateReservedAccountRequest request);
        Task<TransactionStatusResponse?> VerifyTransaction(string reference);
        Task<BvnMatchResponse?> VerifyBvnMatch(BvnMatchRequest request);
        Task<NinVerifyResponse?> VerifyNin(NinVerifyRequest request);
    }
}
