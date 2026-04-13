using System.ComponentModel.DataAnnotations;

namespace Progress.Model.Entitys
{
    /// <summary>菜单/路由表（PDF 5.4）。与角色的关联仅体现在中间表 <see cref="MenuRole"/> 上。</summary>
    public class Menu
    {
        [Key] public int Id { get; set; }

        public int ParentId { get; set; }
        [MaxLength(64)] public string? Name { get; set; }
        [MaxLength(128)] public string? Title { get; set; }
        [MaxLength(256)] public string? Path { get; set; }
        [MaxLength(64)] public string? ElIcon { get; set; }
        [MaxLength(256)] public string? Url { get; set; }
        [MaxLength(8)] public string? MenuType { get; set; }
        [MaxLength(16)] public string? MenuSort { get; set; }
        public int KeepAlive { get; set; }

        [MaxLength(64)] public string? CreateBy { get; set; }
        public DateTime? CreateTime { get; set; }
        [MaxLength(64)] public string? UpdateBy { get; set; }
        public DateTime? UpdateTime { get; set; }

        public int Enable { get; set; } = 1;
        [MaxLength(128)] public string? Redirect { get; set; }
        public int? AlwaysShow { get; set; }
    }
}
