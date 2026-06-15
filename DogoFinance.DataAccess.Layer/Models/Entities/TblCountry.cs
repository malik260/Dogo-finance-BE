using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_COUNTRY")]
    public partial class TblCountry
    {
        public TblCountry()
        {
            TblStates = new HashSet<TblState>();
        }

        [Key]
        public int Id { get; set; }
        
        [StringLength(100)]
        public string Name { get; set; } = null!;
        
        [StringLength(10)]
        public string? Code { get; set; }

        [InverseProperty(nameof(TblState.Country))]
        public virtual ICollection<TblState> TblStates { get; set; }
    }
}
