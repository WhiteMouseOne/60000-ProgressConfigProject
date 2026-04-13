using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Login;
using Progress.Model.Dto.SystemManagement;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>角色管理：分页列表（关键词：角色名）。</summary>
        [HttpPost]
        public async Task<ApiResponseData> GetRoleData([FromBody] RoleGetRequest req)
        {
            var (total, dataList) = await _roleService.GetRoleDataAsync(req);
            return new ApiResponseData
            {
                code = 200,
                message = "Success!",
                data = new { total, dataList }
            };
        }

        /// <summary>下拉：全部启用角色（新增/编辑用户）。</summary>
        [HttpGet]
        public async Task<ApiResponseData> GetRoleList()
        {
            var roles = await _roleService.GetAliveRolesForSelectAsync();
            return new ApiResponseData { code = 200, message = "Success!", data = roles };
        }

        /// <summary>编辑用户：全部可选角色 + 当前用户角色 Id。</summary>
        [HttpGet]
        public async Task<ApiResponseData> GetRoleListAndsingle([FromQuery] int id)
        {
            var (roles, roleIds) = await _roleService.GetRolesAndUserRoleIdAsync(id);
            var payload = new { roles, roleIds };
            return new ApiResponseData { code = 200, message = "Success!", data = payload };
        }

        [HttpPost]
        public async Task<ApiResponseData> AddRole([FromBody] RoleAddDto body)
        {
            var (ok, msg) = await _roleService.AddRoleAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> UpdateRoleData([FromBody] RoleUpdateDto body)
        {
            var (ok, msg) = await _roleService.UpdateRoleAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> DeleteRole([FromBody] RoleDeleteDto body)
        {
            var (ok, msg) = await _roleService.DeleteRoleAsync(body.id);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> BatchDelRole([FromBody] int[]? ids)
        {
            var (ok, msg) = await _roleService.BatchDeleteRolesAsync(ids);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> ChangeRoleEnable([FromBody] RoleChangeEnableDto body)
        {
            var (ok, msg) = await _roleService.ChangeRoleEnableAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> UpdataRoleMenuScope([FromBody] RoleMenuScopeDto body)
        {
            var (ok, msg) = await _roleService.UpdateRoleMenuScopeAsync(body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }
    }
}
