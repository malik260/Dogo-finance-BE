using DogoFinance.Integration.Models.Monnify;

namespace DogoFinance.Integration.Interfaces
{
    public interface IMonnifyService
    {
        Task<InitializeTransactionResponse?> InitializeTransaction(InitializeTransactionRequest request);
        Task<SingleTransferResponse?> SingleTransfer(SingleTransferRequest request);
    }
}
