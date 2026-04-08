using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_ROLE_ACCESS_RIGHT")]
    public partial class TblRoleAccessRight
    {
        [Key]
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int AccessRightId { get; set; }

        [ForeignKey(nameof(RoleId))]
        public virtual TblRole Role { get; set; } = null!;
        [ForeignKey(nameof(AccessRightId))]
        public virtual TblAccessRight AccessRight { get; set; } = null!;
    }
}
