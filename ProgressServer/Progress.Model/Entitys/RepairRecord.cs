using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Progress.Model.Entitys
{
    public class RepairRecord
    {
        [Key] public int Id { get; set; }
        public int OrderLineId { get; set; }
        [ForeignKey(nameof(OrderLineId))] public WorkpieceOrderLine OrderLine { get; set; } = null!;
        public string Description { get; set; } = "";
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? EmailSentAt { get; set; }
    }
}
