using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Login;
using Progress.Model.Dto.SystemManagement;
using Progress.WebApi.Models;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IWebHostEnvironment _env;

        public UsersController(IUserService userService, IWebHostEnvironment env)
        {
            _userService = userService;
            _env = env;
        }

        [HttpPost]
        public async Task<ApiResponseData> GetUserData([FromBody] UserGetRequest req)
        {
            var (total, dataList) = await _userService.GetUserDataAsync(req);
            return new ApiResponseData
            {
                code = 200,
                message = "Success!",
                data = new { total, dataList }
            };
        }

        [HttpGet]
        public async Task<ApiResponseData> GetPersonInfo([FromQuery] string employeeNumber)
        {
            var (ok, message, person) = await _userService.GetPersonInfoAsync(employeeNumber);
            if (!ok || person == null)
                return new ApiResponseData { code = 500, message = message };
            return new ApiResponseData { code = 200, message = "Success!", data = person };
        }

        [HttpPost]
        public async Task<ApiResponseData> AddUser([FromBody] UserAddDto body)
        {
            var (ok, msg) = await _userService.AddUserAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> UpdateUserData([FromBody] UserUpdateDto body)
        {
            var (ok, msg) = await _userService.UpdateUserAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> DeleteUser([FromBody] UserDeleteDto body)
        {
            var (ok, msg) = await _userService.DeleteUserAsync(body.id);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> BatchDelUser([FromBody] List<int> ids)
        {
            var (ok, msg) = await _userService.BatchDeleteUsersAsync(ids ?? new List<int>());
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> ChangeStatusOrPassword([FromBody] ResetPwdOrStatusDto body)
        {
            var (ok, msg) = await _userService.ResetPwdOrStatusAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> UpdatePersonanlInfo([FromBody] UpdatePersonalInfoDto body)
        {
            var (ok, msg) = await _userService.UpdatePersonalInfoAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ApiResponseData> FileupLoad([FromForm] AvatarUploadForm form)
        {
            var file = form?.file;
            var employeeNumber = form?.employeeNumber;
            if (file == null || file.Length == 0)
                return new ApiResponseData { code = 400, message = "file required" };
            if (string.IsNullOrWhiteSpace(employeeNumber))
                return new ApiResponseData { code = 400, message = "employeeNumber required" };

            await _userService.DeleteHeadPortraitAsync(employeeNumber);

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrEmpty(ext)) ext = ".png";
            var name = $"{Guid.NewGuid():N}{ext}";
            var webRoot = _env.WebRootPath ?? Path.Combine(_env.ContentRootPath, "wwwroot");
            var dir = Path.Combine(webRoot, "uploads", "avatars");
            Directory.CreateDirectory(dir);
            var full = Path.Combine(dir, name);
            await using (var fs = new FileStream(full, FileMode.Create))
                await file.CopyToAsync(fs);

            var relative = $"uploads/avatars/{name}".Replace("\\", "/");
            var (ok, msg) = await _userService.UpdateHeadPortraitPathAsync(employeeNumber, relative);
            if (!ok)
                return new ApiResponseData { code = 400, message = msg };
            return new ApiResponseData { code = 200, message = msg, data = new { path = relative } };
        }
    }
}
