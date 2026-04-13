using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Progress.Model.Entitys
{
    /// <summary>订单行（PDF 5.3）：同一 PO 下多行加工件；主键为行 Id + LineNo。</summary>
    public class WorkpieceOrderLine
    {
        [Key] public int Id { get; set; }
        public int LineNo { get; set; }

        [MaxLength(64)] public string PoNumber { get; set; } = "";
        [MaxLength(64)] public string ProjectCode { get; set; } = "";
        [MaxLength(128)] public string DrawingNumber { get; set; } = "";
        [MaxLength(256)] public string PartName { get; set; } = "";

        [MaxLength(128)] public string? Material { get; set; }

        public int SupplierId { get; set; }
        [ForeignKey(nameof(SupplierId))] public Supplier Supplier { get; set; } = null!;

        [Column(TypeName = "decimal(18,4)")] public decimal Quantity { get; set; }
        [MaxLength(16)] public string? Unit { get; set; }

        /// <summary>收货数量（管理员维护，PDF）。</summary>
        public int? ReceivedQuantity { get; set; }

        public DateTime? RequiredDeliveryDate { get; set; }

        [MaxLength(64)] public string? LatestCraftCode { get; set; }
        public DateTime? VendorUpdatedAt { get; set; }
        public DateTime? VendorEstimatedDeliveryDate { get; set; }

        /// <summary>发货数量（默认等于订单数量，供应商维护，PDF）。</summary>
        [Column(TypeName = "decimal(18,4)")] public decimal? ShippedQuantity { get; set; }

        public DateTime? ActualDeliveryDate { get; set; }

        /// <summary>发货状态：见 <see cref="Progress.Model.OrderShippingStatus"/>（PDF 0/1/2）。</summary>
        public int ShippingStatus { get; set; }

        public string? SupplierNotes { get; set; }

        public int RepairStatus { get; set; }
        public DateTime? RepairCreatedAt { get; set; }
        public DateTime? RepairStartedAt { get; set; }
        public DateTime? RepairShippedAt { get; set; }

        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
