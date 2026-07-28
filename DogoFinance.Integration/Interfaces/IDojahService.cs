using DogoFinance.Integration.Models.Dojah;

namespace DogoFinance.Integration.Interfaces
{
    public interface IDojahService
    {
        Task<DojahResponse<DojahBvnData>?> ValidateBvnAsync(string bvn, string? firstName = null, string? lastName = null, string? dateOfBirth = null);
        Task<DojahResponse<DojahNinData>?> LookupNinAsync(string nin, string? firstName = null, string? lastName = null, string? dateOfBirth = null);
    }
}
