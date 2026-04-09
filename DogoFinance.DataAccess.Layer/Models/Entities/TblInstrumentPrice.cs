using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_INSTRUMENT_PRICE")]
    public partial class TblInstrumentPrice
    {
        [Key]
        public int Id { get; set; }
        public int InstrumentId { get; set; }
        public DateTime PriceDate { get; set; }
        [Column(TypeName = "decimal(18, 6)")]
        public decimal NAV { get; set; }
        [StringLength(50)]
        public string? PriceSource { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("InstrumentId")]
        public virtual TblInstrument Instrument { get; set; } = null!;
    }
}
