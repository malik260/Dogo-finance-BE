using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DogoFinance.DataAccess.Layer.Models.Entities
{
    [Table("TBL_USER_ROLE")]
    public partial class TblUserRole
    {
        [Key]
        public long Id { get; set; }
        public long UserId { get; set; }
        public int RoleId { get; set; }

        [ForeignKey(nameof(RoleId))]
        [InverseProperty(nameof(TblRole.TblUserRoles))]
        public virtual TblRole Role { get; set; } = null!;
        [ForeignKey(nameof(UserId))]
        [InverseProperty(nameof(TblUser.TblUserRoles))]
        public virtual TblUser User { get; set; } = null!;
    }
}
