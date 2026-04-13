using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Progress.Model.Entitys
{
    /// <summary>???????????? + ????????</summary>
    public class AlertNotificationLog
    {
        [Key] public int Id { get; set; }
        public int OrderLineId { get; set; }
        [MaxLength(32)] public string AlertDay { get; set; } = "";
        public DateTime SentAt { get; set; }
        [ForeignKey(nameof(OrderLineId))] public WorkpieceOrderLine OrderLine { get; set; } = null!;
    }
}
