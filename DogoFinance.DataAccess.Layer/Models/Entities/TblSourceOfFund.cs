using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_SOURCE_OF_FUND")]
    public partial class TblSourceOfFund
    {
        [Key]
        public int Id { get; set; }

        [StringLength(250)]
        public string Name { get; set; } = null!;
        
        public bool IsActive { get; set; } = true;
    }
}
