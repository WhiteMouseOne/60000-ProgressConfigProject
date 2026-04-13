namespace Progress.Model.Dto.Menu
{
    public class MenuTreeNode
    {
        public int id { get; set; }
        public string title { get; set; } = "";
        public List<MenuTreeNode> children { get; set; } = new();
    }

    public class MenuTreeSelectResult
    {
        public List<MenuTreeNode> menus { get; set; } = new();
        public List<int> checkKeys { get; set; } = new();
    }
}
