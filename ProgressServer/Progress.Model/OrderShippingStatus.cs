namespace Progress.Model
{
    /// <summary>订单行发货状态（PDF 5.3：0 未填写；1 已发货；2 未发货）。</summary>
    public static class OrderShippingStatus
    {
        public const int NotFilled = 0;
        public const int Shipped = 1;
        public const int NotShipped = 2;
    }
}
