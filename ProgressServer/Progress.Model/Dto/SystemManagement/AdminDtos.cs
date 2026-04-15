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
        public int Code { get; set; }
        public string Name { get; set; } = "";
    }

    public class CraftQueryRequest
    {
        public string? name { get; set; }
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 20;
    }

    public class CraftCreateDto
    {
        public int Code { get; set; }
        public string Name { get; set; } = "";
        public string? RecipeBody { get; set; }
    }

    public class CraftUpdateDto
    {
        public int Code { get; set; }
        public string Name { get; set; } = "";
        public string? RecipeBody { get; set; }
    }

    public class SupplierLiteDto
    {
        public int Id { get; set; }
        public string SupplierNumber { get; set; } = "";
        public string Name { get; set; } = "";
    }

    /// <summary>Meta：工艺配方下拉。</summary>
    public class CraftRecipeLiteDto
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; } = "";
    }

    /// <summary>Meta：某配方步序关联的工艺（用于「最新工艺」下拉）。</summary>
    public class CraftInRecipeStepDto
    {
        public int CraftId { get; set; }
        public int CraftCode { get; set; }
        public string CraftName { get; set; } = "";
        public int StepOrder { get; set; }
    }

    #region CraftRecipes方法
    // 工序配方管理页面的表格行数据（包含工艺配方编号和名称）。
    // 与工艺配方列表页的行数据 <see cref="CraftRowDto"/> 区别在于：前者仅包含工艺配方信息，后者还包含工艺信息。
    public class CraftRecipeRowDto
    {
        public int Id { get; set; }
        public int Code { get; set; }
        public string Name { get; set; } = "";
    }

    // 工序配方列表页的查询参数（包含工艺配方名称）。
    // 与工艺配方管理页面的查询参数 <see cref="CraftQueryRequest"/> 区别在于：前者仅包含工艺配方信息，后者还包含工艺信息。
    public class CraftRecipeQueryRequest
    {
        public string? Name { get; set; }
        public int page { get; set; } = 1;
        public int pageSize { get; set; } = 20;
    }

    // 工序配方创建
    public class CraftRecipeCreateDto
    {
        public int Code { get; set; }
        public string Name { get; set; }="";
    }

    //工序配方更新
    public class CraftRecipeUpdateDto
    {
        public int Code { get; set; }
        public string Name { get; set; } = "";
    }
    #endregion
}
