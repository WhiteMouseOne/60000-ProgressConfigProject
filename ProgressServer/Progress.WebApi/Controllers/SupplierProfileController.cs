using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Login;
using Progress.Model.Dto.SystemManagement;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Supplier")]
    public class SupplierProfileController : ControllerBase
    {
        private readonly ISupplierProfileService _svc;

        public SupplierProfileController(ISupplierProfileService svc)
        {
            _svc = svc;
        }

        [HttpGet]
        public async Task<ApiResponseData> Get()
        {
            var data = await _svc.GetAsync();
            if (data == null) return new ApiResponseData { code = 403, message = "无权或不是供应商账号" };
            return new ApiResponseData { code = 200, data = data };
        }

        [HttpPost]
        public async Task<ApiResponseData> Update([FromBody] SupplierProfileUpdateDto body)
        {
            var (ok, message) = await _svc.UpdateAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = message };
        }
    }
}
