using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Login;
using Progress.Model.Dto.SystemManagement;
using Progress.Repository;
using System.Numerics;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class CraftRecipeAdminController : ControllerBase
    {
        private readonly ICraftRecipeAdminService _svc;
        public CraftRecipeAdminController(ICraftRecipeAdminService svc)
        {
            _svc = svc;
        }
        [HttpGet]
        public async Task<ApiResponseData> List()
        {
            var dataList =  await _svc.ListAsync();
            return new ApiResponseData { code = 200, data = dataList };
        }

        [HttpPost]
        public async Task<ApiResponseData> Query([FromBody] CraftRecipeQueryRequest req)
        {
            var (total, dataList) = await _svc.QueryAsync(req);
            return new ApiResponseData { code = 200, data = new { total, dataList } };
        }

        [HttpPost]
        public async Task<ApiResponseData> Create([FromBody] CraftRecipeCreateDto body)
        {
            var ( ok, mes)= await _svc.CreateAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = mes };
        }
        [HttpPost("{id:int}")]
        public async Task<ApiResponseData> Update(int id,[FromBody] CraftRecipeUpdateDto body)
        {
            var (ok, mes) = await _svc.UpdateAsync(id,body);
            return new ApiResponseData { code = ok ? 200 : 400, message = mes };
        }



        [HttpPost("{id:int}")]
        public async Task<ApiResponseData> Delete(int id)
        {
            var (ok, msg) = await _svc.DeleteAsync(id);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

    }
}
