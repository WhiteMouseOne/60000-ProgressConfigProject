using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Progress.Model.Entitys
{
    /// <summary>
    /// 账号表（PDF 5.6）。与角色的关联通过中间表 <see cref="UserRole"/> 维护。
    /// </summary>
    public class Users
    {
        [Key] public int Id { get; set; }

        [MaxLength(64)] public string EmployeeNumber { get; set; } = "";
        [MaxLength(128)] public string UserName { get; set; } = "";
        [MaxLength(64)] public string Password { get; set; } = "";
        [MaxLength(32)] public string? PhoneNumber { get; set; }
        [MaxLength(128)] public string? Email { get; set; }
        [MaxLength(512)] public string? HeadPortrait { get; set; }

        public int Enable { get; set; } = 1;
        [MaxLength(64)] public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        [MaxLength(64)] public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }

        public int IsDeleted { get; set; }
        /// <summary>Token 过期或刷新时间（PDF 5.6）。</summary>
        public DateTime? Token { get; set; }

        /// <summary>0 非供应商账号，1 供应商账号；为 1 时应有 <see cref="SupplierId"/>。</summary>
        public int IsSupplierAccount { get; set; }

        public int? SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))] public Supplier? Supplier { get; set; }
    }
}
