namespace Progress.Model.Dto.Menu
{
    /// <summary>与 LineMesServer MES.Model.Dto.Menu.DynamicRoutes 结构对齐，供动态路由序列化。</summary>
    public class DynamicRoutes
    {
        public string path { get; set; } = "";
        public string component { get; set; } = "";
        public string redirect { get; set; } = "";
        public string name { get; set; } = "";
        public Meta meta { get; set; } = new();
        public List<DynamicRoutes>? children { get; set; }
    }

    public class Meta
    {
        public string title { get; set; } = "";
        public string elIcon { get; set; } = "";
        public string[] roles { get; set; } = Array.Empty<string>();
        public int? alwaysShow { get; set; }
        public int keepAlive { get; set; }
    }
}
