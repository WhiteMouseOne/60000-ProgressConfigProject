namespace Progress.Model.Dto.SystemManagement
{
    public class SupplierRowDto
    {
        public int Id { get; set; }
        public string SupplierNumber { get; set; } = "";
        public string Name { get; set; } = "";
    }

    public class CraftRowDto
    {
        public int Id { get; set; }
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
    }

    public class SupplierLiteDto
    {
        public int Id { get; set; }
        public string SupplierNumber { get; set; } = "";
        public string Name { get; set; } = "";
    }
}
