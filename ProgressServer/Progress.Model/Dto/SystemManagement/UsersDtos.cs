namespace Progress.Model.Dto.SystemManagement
{
    public class UserGetRequest
    {
        public int page { get; set; } = 1;
        public int size { get; set; } = 10;
        public string? employeeNumber { get; set; }
        public string? userName { get; set; }
        public int? enable { get; set; }
    }

    public class UserListRow
    {
        public int id { get; set; }
        public string employeeNumber { get; set; } = "";
        public string userName { get; set; } = "";
        public string? phoneNumber { get; set; }
        public string? email { get; set; }
        public string? headPortrait { get; set; }//Í·Ïñ
        public int enable { get; set; }//ÊÇ·ñÆôÓÃ
        public string? createBy { get; set; }
        public DateTime? createTime { get; set; }
        public string? updateBy { get; set; }
        public DateTime? updateTime { get; set; }
        public int isDeleted { get; set; }
        public int isSupplierAccount { get; set; }
        public int? supplierId { get; set; }
    }
}
