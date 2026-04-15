using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Login;
using Progress.Model.Dto.SystemManagement;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CraftAdminController : ControllerBase
    {
        private readonly ICraftAdminService _svc;

        public CraftAdminController(ICraftAdminService svc)
        {
            _svc = svc;
        }

        [HttpGet]
        public async Task<ApiResponseData> List()
        {
            var list = await _svc.ListAsync();
            return new ApiResponseData { code = 200, data = list };
        }

        [HttpPost]
        public async Task<ApiResponseData> Query([FromBody] CraftQueryRequest req)
        {
            var (total, dataList) = await _svc.QueryAsync(req);
            return new ApiResponseData
            {
                code = 200,
                data = new { total, dataList }
            };
        }

        [HttpPost]
        public async Task<ApiResponseData> Create([FromBody] CraftCreateDto body)
        {
            var (ok, msg) = await _svc.CreateAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost("{id:int}")]
        public async Task<ApiResponseData> Update(int id, [FromBody] CraftUpdateDto body)
        {
            var (ok, msg) = await _svc.UpdateAsync(id, body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost("{id:int}")]
        public async Task<ApiResponseData> Delete(int id)
        {
            var (ok, msg) = await _svc.DeleteAsync(id);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }
    }
}
