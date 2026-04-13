using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Progress.Model.Entitys
{
    /// <summary>菜单角色表（PDF 5.7）：菜单与角色多对多。</summary>
    public class MenuRole
    {
        [Key] public int Id { get; set; }
        public int MenuId { get; set; }
        public int RoleId { get; set; }

        [ForeignKey(nameof(MenuId))] public Menu Menu { get; set; } = null!;
        [ForeignKey(nameof(RoleId))] public Role Role { get; set; } = null!;
    }
}
