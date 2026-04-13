namespace Progress.IService
{
    public interface ICurrentUser
    {
        int UserId { get; }
        string EmployeeNumber { get; }
        string UserName { get; }
        IReadOnlyList<string> Roles { get; }
        int? SupplierId { get; }
        /// <summary>0 非供应商账号，1 供应商账号（与 JWT Claim 一致）。</summary>
        int IsSupplierAccount { get; }
        bool IsAdmin { get; }
    }
}
