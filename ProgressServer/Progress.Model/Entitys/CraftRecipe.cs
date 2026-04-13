using System.ComponentModel.DataAnnotations;

namespace Progress.Model.Entitys
{
    /// <summary>工艺配方表（PDF 5.2.2）。</summary>
    public class CraftRecipe
    {
        [Key] public int Id { get; set; }

        /// <summary>工艺配方编号（唯一）。</summary>
        public int Code { get; set; }

        [MaxLength(256)] public string Name { get; set; } = "";

        public ICollection<CraftRecipeStep> Steps { get; set; } = new List<CraftRecipeStep>();
    }
}
