namespace Progress.Model
{
    /// <summary>订单行返修状态（与 WorkpieceOrderLine.RepairStatus 对应）。</summary>
    public static class OrderRepairStatus
    {
        public const int None = 0;
        /// <summary>已发起返修，等待供应商填写开始/发货日期。</summary>
        public const int PendingSupplier = 1;
        /// <summary>供应商已填返修开始日期，尚未填返修发货日期。</summary>
        public const int InProgress = 2;
        /// <summary>供应商已填返修发货日期。</summary>
        public const int RepairedShipped = 3;
    }
}
