namespace DogoFinance.DataAccess.Layer.Models.Base
{
    /// <summary>
    /// Information about the current authenticated user/operator.
    /// </summary>
    public class OperatorInfo
    {
        public long Id { get; set; }
        public string? UserName { get; set; }
        public string? ApiToken { get; set; }
        public long? TenantId { get; set; }
        public int? CompanyId { get; set; }
        public short? BranchId { get; set; }
    }
}
