using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_STATE")]
    public partial class TblState
    {
        [Key]
        public int Id { get; set; }

        public int CountryId { get; set; }

        [StringLength(100)]
        public string Name { get; set; } = null!;

        [ForeignKey(nameof(CountryId))]
        [InverseProperty(nameof(Entities.TblCountry.TblStates))]
        public virtual TblCountry Country { get; set; } = null!;
    }
}
