using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Progress.Model.Entitys
{
    /// <summary>账号角色表（PDF 5.8）：用户与角色多对多。</summary>
    public class UserRole
    {
        [Key] public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        [ForeignKey(nameof(UserId))] public Users User { get; set; } = null!;
        [ForeignKey(nameof(RoleId))] public Role Role { get; set; } = null!;
    }
}
