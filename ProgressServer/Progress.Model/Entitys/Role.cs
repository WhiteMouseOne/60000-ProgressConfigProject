using System.ComponentModel.DataAnnotations;

namespace Progress.Model.Entitys
{
    /// <summary>角色表（PDF 5.5）。与账号、菜单的关联仅体现在中间表 <see cref="UserRole"/>、<see cref="MenuRole"/> 上。</summary>
    public class Role
    {
        [Key] public int Id { get; set; }

        [MaxLength(64)] public string RoleName { get; set; } = "";
        [MaxLength(256)] public string? Description { get; set; }
        public int RoleSort { get; set; }

        [MaxLength(64)] public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        [MaxLength(64)] public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }

        public int Enable { get; set; } = 1;
    }
}
