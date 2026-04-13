using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Progress.Model.Entitys
{
    /// <summary>工艺步骤表（PDF 5.2.3）：配方内工艺顺序。</summary>
    public class CraftRecipeStep
    {
        [Key] public int Id { get; set; }

        public int CraftRecipeId { get; set; }
        [ForeignKey(nameof(CraftRecipeId))] public CraftRecipe CraftRecipe { get; set; } = null!;

        public int CraftId { get; set; }
        [ForeignKey(nameof(CraftId))] public Craft Craft { get; set; } = null!;

        public int StepOrder { get; set; }
    }
}
