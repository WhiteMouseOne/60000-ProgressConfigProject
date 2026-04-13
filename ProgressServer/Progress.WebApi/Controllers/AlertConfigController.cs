using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Login;
using Progress.Model.Dto.Order;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AlertConfigController : ControllerBase
    {
        private readonly IAlertSettingService _svc;

        public AlertConfigController(IAlertSettingService svc)
        {
            _svc = svc;
        }

        [HttpGet]
        public async Task<ApiResponseData> Get()
        {
            var s = await _svc.GetAsync();
            if (s == null) return new ApiResponseData { code = 404, message = "未配置" };
            return new ApiResponseData { code = 200, data = s };
        }

        [HttpPost]
        public async Task<ApiResponseData> Save([FromBody] AlertSettingDto dto)
        {
            await _svc.SaveAsync(dto);
            return new ApiResponseData { code = 200, message = "OK" };
        }
    }
}
