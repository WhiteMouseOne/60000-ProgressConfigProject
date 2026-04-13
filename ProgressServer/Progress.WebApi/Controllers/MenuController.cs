using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Login;
using Progress.Model.Dto.Menu;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpPost]
        public async Task<ApiResponseData> GetMenuData([FromBody] MenuQueryRequest req)
        {
            var list = await _menuService.GetMenuDataAsync(req);
            return new ApiResponseData
            {
                code = 200,
                message = "Success!",
                data = list
            };
        }

        [HttpGet]
        public async Task<ApiResponseData> GetAllMenuTreeSelect()
        {
            var result = await _menuService.GetAllMenuTreeSelectAsync();
            return new ApiResponseData
            {
                code = 200,
                message = "Success!",
                data = new { menus = result.menus }
            };
        }

        [HttpGet]
        public async Task<ApiResponseData> GetMenuTreeSelect([FromQuery] int roleId)
        {
            var result = await _menuService.GetMenuTreeSelectAsync(roleId);
            return new ApiResponseData
            {
                code = 200,
                message = "Success!",
                data = new { menus = result.menus, checkKeys = result.checkKeys }
            };
        }

        [HttpPost]
        public async Task<ApiResponseData> AddMenu([FromBody] MenuAddDto body)
        {
            var (ok, msg) = await _menuService.AddMenuAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> UpdateMenuData([FromBody] MenuUpdateDto body)
        {
            var (ok, msg) = await _menuService.UpdateMenuAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> DeleteMenu([FromBody] MenuDeleteDto body)
        {
            var (ok, msg) = await _menuService.DeleteMenuAsync(body.id);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> BatchDelMenu([FromBody] List<int> ids)
        {
            var (ok, msg) = await _menuService.BatchDeleteMenusAsync(ids ?? new List<int>());
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> ChangeMenuStatus([FromBody] UpdateMenuStatusDto body)
        {
            var (ok, msg) = await _menuService.ChangeMenuStatusAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }
    }
}
