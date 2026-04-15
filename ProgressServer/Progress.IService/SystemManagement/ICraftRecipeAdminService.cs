using Progress.Model.Dto.SystemManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Progress.IService.SystemManagement
{
    public interface ICraftRecipeAdminService
    {
        //展示工艺配方列表
        Task<List<CraftRecipeRowDto>> ListAsync(CancellationToken ct = default);
        //查询工艺配方
        Task<(int total, List<CraftRecipeRowDto> list)> QueryAsync(CraftRecipeQueryRequest req, CancellationToken ct = default);
        //增加工艺配方
        Task<(bool ok, string message)> CreateAsync(CraftRecipeCreateDto dto, CancellationToken ct = default);
        //修改工艺配方
        Task<(bool ok, string messagq)> UpdateAsync(int id, CraftRecipeUpdateDto dto, CancellationToken ct = default);
        //删除工艺配方
        Task<(bool ok,string message)> DeleteAsync(int id, CancellationToken ct = default);
    }
}
