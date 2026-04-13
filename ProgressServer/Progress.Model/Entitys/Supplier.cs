using System.ComponentModel.DataAnnotations;

namespace Progress.Model.Entitys
{
    /// <summary>
    /// 供应商组织主数据：业务唯一编号（文本，可配置）+ 名称。登录账号在 <see cref="Users"/>。
    /// </summary>
    public class Supplier
    {
        [Key] public int Id { get; set; }

        /// <summary>供应商业务编号（唯一，管理员可配置）。</summary>
        [MaxLength(64)] public string SupplierNumber { get; set; } = "";

        [MaxLength(256)] public string Name { get; set; } = "";

        //0-禁用，1-启用
        public int Enable { get; set; } = 1;
        //创建时间
        public DateTime? CreateTime { get; set; }
        //创建人
        [MaxLength(64)] public string? CreateBy { get; set; }
        //更新时间
        public DateTime? UpdateTime { get; set; }
        //更新人
        [MaxLength(64)] public string? UpdateBy { get; set; }
    }
}
