namespace Progress.Model.Dto.SystemManagement
{
    public class UserAddDto
    {
        public string employeeNumber { get; set; } = "";
        public string userName { get; set; } = "";
        public string password { get; set; } = "";
        public int roleIds { get; set; }
        public string? phoneNumber { get; set; }
        public string? email { get; set; }
        public string? enable { get; set; }
        public string? createBy { get; set; }
        /// <summary>0 普通账号，1 供应商账号。</summary>
        public int isSupplierAccount { get; set; }
        public int? supplierId { get; set; }
    }

    public class UserUpdateDto
    {
        public int id { get; set; }
        public string employeeNumber { get; set; } = "";
        public string userName { get; set; } = "";
        public string password { get; set; } = "";
        public int roleIds { get; set; }
        public string? phoneNumber { get; set; }
        public string? email { get; set; }
        public string? enable { get; set; }
        public string? updateBy { get; set; }
        public int isSupplierAccount { get; set; }
        public int? supplierId { get; set; }
    }

    public class UserDeleteDto
    {
        public int id { get; set; }
    }

    public class ResetPwdOrStatusDto
    {
        public int id { get; set; }
        public int enable { get; set; }
        public string? password { get; set; }
    }

    public class UpdatePersonalInfoDto
    {
        public int id { get; set; }
        public string? oldPassword { get; set; }
        public string? newPassword { get; set; }
        public string? phoneNumber { get; set; }
        public string? email { get; set; }
        public string? roleName { get; set; }
        public string? userName { get; set; }
    }

    public class PersonInfoDto
    {
        public int id { get; set; }
        public string employeeNumber { get; set; } = "";
        public string userName { get; set; } = "";
        public string? phoneNumber { get; set; }
        public string? email { get; set; }
        public string? headPortrait { get; set; }
        public string roleName { get; set; } = "";
        public string password { get; set; } = "";
    }
}
