using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.Business;
using Progress.Model.Dto.Login;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin,Supervisor,Supplier")]
    public class HomeController : ControllerBase
    {
        private readonly IHomeStatsService _home;

        public HomeController(IHomeStatsService home)
        {
            _home = home;
        }

        [HttpGet]
        public async Task<ApiResponseData> Dashboard()
        {
            var dto = await _home.GetDashboardAsync();
            return new ApiResponseData { code = 200, message = "OK", data = dto };
        }
    }
}
