using System.ComponentModel.DataAnnotations;

namespace Progress.Model.Entitys
{
    /// <summary>工艺主数据（PDF 5.2.1）；结构化配方见 <see cref="CraftRecipe"/>、<see cref="CraftRecipeStep"/>。</summary>
    public class Craft
    {
        [Key] public int Id { get; set; }
        [MaxLength(64)] public string Code { get; set; } = "";
        [MaxLength(256)] public string Name { get; set; } = "";
        /// <summary>可选：兼容旧版 JSON 或说明文本。</summary>
        public string? RecipeBody { get; set; }
    }
}
