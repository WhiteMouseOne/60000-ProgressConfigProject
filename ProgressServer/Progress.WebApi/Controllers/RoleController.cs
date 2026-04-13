using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progress.Model.Dto.Login;
using Progress.Repository;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        private readonly ProgressDbContext _db;

        public RoleController(ProgressDbContext db)
        {
            _db = db;
        }

        /// <summary>下拉：全部启用角色（新增/编辑用户）。</summary>
        [HttpGet]
        public async Task<ApiResponseData> GetRoleList()
        {
            var roles = await _db.Roles!.AsNoTracking()
                .Where(r => r.Enable == 1)
                .OrderBy(r => r.RoleSort)
                .Select(r => new { id = r.Id, roleName = r.RoleName, description = r.Description, enable = r.Enable })
                .ToListAsync();
            return new ApiResponseData { code = 200, message = "Success!", data = roles };
        }

        /// <summary>编辑用户：全部可选角色 + 当前用户角色 Id。</summary>
        [HttpGet]
        public async Task<ApiResponseData> GetRoleListAndsingle([FromQuery] int id)
        {
            var roles = await _db.Roles!.AsNoTracking()
                .Where(r => r.Enable == 1)
                .OrderBy(r => r.RoleSort)
                .Select(r => new { id = r.Id, roleName = r.RoleName, description = r.Description, enable = r.Enable })
                .ToListAsync();

            var ur = await _db.UserRoles!.AsNoTracking()
                .Where(x => x.UserId == id)
                .Select(x => (int?)x.RoleId)
                .FirstOrDefaultAsync();

            var payload = new
            {
                roles,
                roleIds = ur ?? -1
            };
            return new ApiResponseData { code = 200, message = "Success!", data = payload };
        }
    }
}
