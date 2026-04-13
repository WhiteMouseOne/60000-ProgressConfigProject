namespace Progress.Model.Dto.SystemManagement
{
    public class RoleGetRequest
    {
        public int page { get; set; } = 1;
        public int size { get; set; } = 10;
        public string? roleName { get; set; }
    }

    public class RoleListRow
    {
        public int id { get; set; }
        public string roleName { get; set; } = "";
        public string? description { get; set; }
        public int roleSort { get; set; }
        public int enable { get; set; }
        public string? createBy { get; set; }
        public DateTime? createTime { get; set; }
        public string? updateBy { get; set; }
        public DateTime? updateTime { get; set; }
    }

    public class RoleAddDto
    {
        public string roleName { get; set; } = "";
        public int roleSort { get; set; }
        public int enable { get; set; } = 1;
        public string? createBy { get; set; }
    }

    public class RoleUpdateDto
    {
        public int id { get; set; }
        public string roleName { get; set; } = "";
        public int roleSort { get; set; }
        public int enable { get; set; }
        public string? updateBy { get; set; }
    }

    public class RoleDeleteDto
    {
        public int id { get; set; }
    }

    public class RoleChangeEnableDto
    {
        public int id { get; set; }
        public int enable { get; set; }
    }

    public class RoleMenuScopeDto
    {
        public int roleId { get; set; }
        public List<int> menuIds { get; set; } = new();
    }

    /// <summary>启用角色下拉 / 用户编辑角色选择（与 GetRoleList 返回项一致）。</summary>
    public class RoleDropdownDto
    {
        public int id { get; set; }
        public string roleName { get; set; } = "";
        public string? description { get; set; }
        public int enable { get; set; }
    }
}
