using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.Model.Dto.SystemManagement
{
    public class SupplierQueryRequest
    {
        public int page { get; set; } = 1;
        public int size { get; set; } = 10;
        public string? supplierNumber { get; set; }
        public string? name { get; set; }
        public int? enable { get; set; }
    }

    public class SupplierListRow
    {
        public int id { get; set; }
        public string supplierNumber { get; set; } = "";
        public string name { get; set; } = "";
        public int enable { get; set; }
        public string? createBy { get; set; }
        public DateTime? createTime { get; set; }
        public string? updateBy { get; set; }
        public DateTime? updateTime { get; set; }
    }

    public class SupplierCreateDto
    {
        public string SupplierNumber { get; set; } = "";
        public string Name { get; set; } = "";

        public int Enable { get; set; } = 1;
    }

    public class SupplierBatchDeleteDto
    {
        public List<int> Ids { get; set; } = new();
    }
}
