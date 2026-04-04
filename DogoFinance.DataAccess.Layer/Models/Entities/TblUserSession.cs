using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_USER_SESSION")]
    public class TblUserSession
    {
        [Key]
        public long SessionId { get; set; }
        
        public long UserId { get; set; }
        
        [StringLength(500)]
        public string RefreshToken { get; set; } = null!;
        
        public DateTime RefreshTokenExpiry { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime LastUsedAt { get; set; }
        
        [StringLength(200)]
        public string? DeviceName { get; set; }
        
        [StringLength(500)]
        public string? UserAgent { get; set; }
        
        [StringLength(50)]
        public string? IpAddress { get; set; }
        
        public bool IsRevoked { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual TblUser User { get; set; } = null!;
    }
}
