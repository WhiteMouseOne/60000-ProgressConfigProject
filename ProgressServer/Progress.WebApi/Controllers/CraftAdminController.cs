using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Login;

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
    }
}
