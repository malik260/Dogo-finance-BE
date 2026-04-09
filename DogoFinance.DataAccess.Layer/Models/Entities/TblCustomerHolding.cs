using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_CUSTOMER_HOLDING")]
    public partial class TblCustomerHolding
    {
        [Key]
        public long Id { get; set; }
        public long CustomerId { get; set; }
        public int InstrumentId { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal Units { get; set; }
        [Column(TypeName = "decimal(18, 4)")]
        public decimal InvestedAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey("CustomerId")]
        public virtual TblCustomer Customer { get; set; } = null!;
        [ForeignKey("InstrumentId")]
        public virtual TblInstrument Instrument { get; set; } = null!;
    }
}
