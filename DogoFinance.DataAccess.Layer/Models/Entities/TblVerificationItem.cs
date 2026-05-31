using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_VERIFICATION_ITEM")]
    public class TblVerificationItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Type { get; set; } = null!;

        public bool IsSystemVerified { get; set; }

        [StringLength(100)]
        public string? SystemRule { get; set; }

        [StringLength(250)]
        public string? TargetEntityTypes { get; set; }

        [StringLength(100)]
        public string? Icon { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
