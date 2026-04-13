using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.Business;
using Progress.Model.Dto.Login;
using Progress.Model.Dto.Order;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class RepairController : ControllerBase
    {
        private readonly IRepairService _svc;

        public RepairController(IRepairService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<ApiResponseData> Create([FromBody] RepairCreateRequest body)
        {
            var (ok, message) = await _svc.CreateAsync(body);
            return new ApiResponseData { code = ok ? 200 : 500, message = message };
        }
    }
}
