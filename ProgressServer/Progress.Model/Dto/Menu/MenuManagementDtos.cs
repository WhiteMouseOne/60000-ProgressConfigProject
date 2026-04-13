namespace Progress.Model.Dto.Menu
{
    public class MenuQueryRequest
    {
        public string? menuName { get; set; }
        public string? enable { get; set; }
    }

    public class MenuTreeRowDto
    {
        public int id { get; set; }
        public int parentId { get; set; }
        public string? name { get; set; }
        public string? title { get; set; }
        public string? path { get; set; }
        public string? elIcon { get; set; }
        public string? url { get; set; }
        public string? menuType { get; set; }
        public string? menuSort { get; set; }
        public int keepAlive { get; set; }
        public int enable { get; set; }
        public string? createBy { get; set; }
        public DateTime? createTime { get; set; }
        public string? updateBy { get; set; }
        public DateTime? updateTime { get; set; }
        public int? alwaysShow { get; set; }
        public string? redirect { get; set; }
        public List<MenuTreeRowDto>? children { get; set; }
    }

    public class MenuAddDto
    {
        public int parentId { get; set; }
        public string? name { get; set; }
        public string? title { get; set; }
        public string? path { get; set; }
        public string? elIcon { get; set; }
        public string? url { get; set; }
        public string? menuType { get; set; }
        public string? menuSort { get; set; }
        public int keepAlive { get; set; }
        public string? enable { get; set; }
        public string? createBy { get; set; }
        public int alwaysShow { get; set; }
        public string? Redirect { get; set; }
    }

    public class MenuUpdateDto
    {
        public int id { get; set; }
        public int parentId { get; set; }
        public string? name { get; set; }
        public string? title { get; set; }
        public string? path { get; set; }
        public string? elIcon { get; set; }
        public string? url { get; set; }
        public string? menuType { get; set; }
        public string? menuSort { get; set; }
        public string? keepAlive { get; set; }
        public string? enable { get; set; }
        public string? updateBy { get; set; }
        public int alwaysShow { get; set; }
        public string? Redirect { get; set; }
    }

    public class MenuDeleteDto
    {
        public int id { get; set; }
    }

    public class UpdateMenuStatusDto
    {
        public int id { get; set; }
        public int keepAlive { get; set; }
    }

    public class AllMenuTreeResult
    {
        public List<MenuTreeRowDto> menus { get; set; } = new();
    }
}
