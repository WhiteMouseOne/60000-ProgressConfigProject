namespace Progress.Model.Dto.SystemManagement
{
    /// <summary>供应商登录后可见的组织信息（只读）。</summary>
    public class SupplierOrgDto
    {
        public int Id { get; set; }
        public string SupplierNumber { get; set; } = "";
        public string Name { get; set; } = "";
    }

    /// <summary>当前登录账号信息（供应商用户）。</summary>
    public class SupplierAccountDto
    {
        /// <summary>登录名，不可自改。</summary>
        public string EmployeeNumber { get; set; } = "";
        public string UserName { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? HeadPortrait { get; set; }
    }

    public class SupplierProfileDto
    {
        public SupplierOrgDto Org { get; set; } = new();
        public SupplierAccountDto Account { get; set; } = new();
    }

    /// <summary>供应商可更新：账号基本信息；组织名称与编号由管理员维护。</summary>
    public class SupplierProfileUpdateDto
    {
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? HeadPortrait { get; set; }
    }

    /// <summary>管理员维护供应商组织：名称与业务编号。</summary>
    public class SupplierAdminUpdateDto
    {
        public string? Name { get; set; }
        public string? SupplierNumber { get; set; }

        public int? Enable { get; set; }
    }
}
