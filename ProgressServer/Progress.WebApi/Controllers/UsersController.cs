using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Progress.Model.Dto.Login;
using Progress.Model.Dto.SystemManagement;
using Progress.Repository;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly ProgressDbContext _db;

        public UsersController(ProgressDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<ApiResponseData> GetUserData([FromBody] UserGetRequest req)
        {
            var q = _db.Users!.AsNoTracking().Where(u => u.IsDeleted == 0);
            if (!string.IsNullOrWhiteSpace(req.employeeNumber))
                q = q.Where(u => u.EmployeeNumber.Contains(req.employeeNumber));
            if (!string.IsNullOrWhiteSpace(req.userName))
                q = q.Where(u => u.UserName.Contains(req.userName));
            if (req.enable is int en)
                q = q.Where(u => u.Enable == en);

            var total = await q.CountAsync();
            var page = Math.Max(1, req.page);
            var size = Math.Max(1, req.size);
            var list = await q
                .OrderBy(u => u.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(u => new UserListRow
                {
                    id = u.Id,
                    employeeNumber = u.EmployeeNumber,
                    userName = u.UserName,
                    phoneNumber = u.PhoneNumber,
                    email = u.Email,
                    headPortrait = u.HeadPortrait,
                    enable = u.Enable,
                    createBy = u.CreateBy,
                    createTime = u.CreateTime,
                    updateBy = u.UpdateBy,
                    updateTime = u.UpdateTime,
                    isDeleted = u.IsDeleted
                })
                .ToListAsync();

            return new ApiResponseData
            {
                code = 200,
                message = "Success!",
                data = new { total, dataList = list }
            };
        }

        /// <summary>个人中心：按工号取用户信息（与前端约定一致）。</summary>
        [HttpGet]
        public async Task<ApiResponseData> GetPersonInfo([FromQuery] string employeeNumber)
        {
            if (string.IsNullOrWhiteSpace(employeeNumber))
                return new ApiResponseData { code = 500, message = "employeeNumber required" };

            var u = await _db.Users!.AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmployeeNumber == employeeNumber && x.IsDeleted == 0);
            if (u == null)
                return new ApiResponseData { code = 500, message = "用户不存在" };

            var roleName = await (
                from ur in _db.UserRoles!
                join r in _db.Roles! on ur.RoleId equals r.Id
                where ur.UserId == u.Id
                orderby r.RoleSort
                select r.RoleName
            ).FirstOrDefaultAsync() ?? "";

            return new ApiResponseData
            {
                code = 200,
                message = "Success!",
                data = new
                {
                    id = u.Id,
                    employeeNumber = u.EmployeeNumber,
                    userName = u.UserName,
                    phoneNumber = u.PhoneNumber,
                    email = u.Email,
                    headPortrait = u.HeadPortrait,
                    roleName,
                    password = ""
                }
            };
        }
    }
}
