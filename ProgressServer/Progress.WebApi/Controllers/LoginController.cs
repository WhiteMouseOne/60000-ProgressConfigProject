using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Login;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IAuthLoginService _auth;

        public LoginController(IAuthLoginService auth)
        {
            _auth = auth;
        }

        [HttpPost]
        public async Task<ApiResponseData> GetToken([FromBody] LoginRequestData data)
        {
            var res = new ApiResponseData();
            var (ok, message, payload) = await _auth.LoginAsync(data.EmployeeNumber, data.Password);
            if (!ok)
            {
                res.code = 500;
                res.message = message;
                return res;
            }
            res.code = 200;
            res.message = message;
            res.data = payload;
            return res;
        }

        [HttpPost]
        [Authorize]
        public async Task<ApiResponseData> Info([FromBody] LoginRequestData data)
        {
            var (username, roles, isSupplierAccount, supplierId) = await _auth.GetInfoAsync(data.EmployeeNumber);
            return new ApiResponseData
            {
                code = 200,
                message = "Success!",
                data = new { username, roles, isSupplierAccount, supplierId }
            };
        }

        [HttpGet]
        [Authorize]
        public async Task<ApiResponseData> GetDynamicRoutes()
        {
            return new ApiResponseData
            {
                code = 200,
                message = "Success!",
                data = await _auth.GetDynamicRoutesAsync()
            };
        }
    }
}
