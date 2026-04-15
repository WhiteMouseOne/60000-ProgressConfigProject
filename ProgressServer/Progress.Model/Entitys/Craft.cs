using System.ComponentModel.DataAnnotations;

namespace Progress.Model.Entitys
{
    /// <summary>工艺主数据（PDF 5.2.1）；结构化配方见 <see cref="CraftRecipe"/>、<see cref="CraftRecipeStep"/>。</summary>
    public class Craft
    {
        [Key] public int Id { get; set; }
        /// <summary>业务编码（整型，唯一）。</summary>
        public int Code { get; set; }
        [MaxLength(256)] public string Name { get; set; } = "";
        /// <summary>可选：兼容旧版 JSON 或说明文本。</summary>
        public string? RecipeBody { get; set; }
    }
}
