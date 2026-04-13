namespace Progress.Model.Dto.Order
{
    public class OrderLineQuery
    {
        public string? PoNumber { get; set; }
        public string? ProjectCode { get; set; }
        public string? DrawingNumber { get; set; }
        public string? PartName { get; set; }
        public int? SupplierId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class OrderLineDto
    {
        public int Id { get; set; }
        public int LineNo { get; set; }
        public string PoNumber { get; set; } = "";
        public string ProjectCode { get; set; } = "";
        public string DrawingNumber { get; set; } = "";
        public string PartName { get; set; } = "";
        public string? Material { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public int? ReceivedQuantity { get; set; }
        public DateTime? RequiredDeliveryDate { get; set; }
        public string? LatestCraftCode { get; set; }
        public DateTime? VendorUpdatedAt { get; set; }
        public DateTime? VendorEstimatedDeliveryDate { get; set; }
        public decimal? ShippedQuantity { get; set; }
        public int ShippingStatus { get; set; }
        public string? SupplierNotes { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public int RepairStatus { get; set; }
        public DateTime? RepairCreatedAt { get; set; }
        public DateTime? RepairStartedAt { get; set; }
        public DateTime? RepairShippedAt { get; set; }
        public DateTime CreateTime { get; set; }
    }

    public class OrderLineCreateOrEdit
    {
        public int? Id { get; set; }
        public string PoNumber { get; set; } = "";
        public string ProjectCode { get; set; } = "";
        public string DrawingNumber { get; set; } = "";
        public string PartName { get; set; } = "";
        public string? Material { get; set; }
        public int SupplierId { get; set; }
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public int? ReceivedQuantity { get; set; }
        public DateTime? RequiredDeliveryDate { get; set; }
        public string? LatestCraftCode { get; set; }
        public int ShippingStatus { get; set; }
        public string? SupplierNotes { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
    }

    public class SupplierLineUpdate
    {
        public string? SupplierNotes { get; set; }
        public string? LatestCraftCode { get; set; }
        public int? ShippingStatus { get; set; }
        public DateTime? ActualDeliveryDate { get; set; }
        public DateTime? VendorEstimatedDeliveryDate { get; set; }
        public decimal? ShippedQuantity { get; set; }
        public DateTime? RepairStartedAt { get; set; }
        public DateTime? RepairShippedAt { get; set; }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int Total { get; set; }
    }

    public class AlertLineDto
    {
        public int Id { get; set; }
        public int LineNo { get; set; }
        public string PoNumber { get; set; } = "";
        public string PartName { get; set; } = "";
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public DateTime? RequiredDeliveryDate { get; set; }
        public int LeadDays { get; set; }
    }

    public class SupplierStatsDto
    {
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public int TotalLines { get; set; }
        public int ShippedLines { get; set; }
        public int OverdueNotShipped { get; set; }
        public double CompletionRate { get; set; }
    }

    public class HomeDashboardDto
    {
        public List<AlertLineDto> Alerts { get; set; } = new();
        public List<SupplierStatsDto> SupplierStats { get; set; } = new();
    }

    public class RepairCreateRequest
    {
        public int OrderLineId { get; set; }
        public string Description { get; set; } = "";
    }

    public class AlertSettingDto
    {
        public int Id { get; set; }
        public int LeadDays { get; set; }
        public bool Enabled { get; set; }
    }
}
