using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Progress.IService;

namespace Progress.WebApi.Infrastructure
{
    public class HttpCurrentUser : ICurrentUser
    {
        private readonly IHttpContextAccessor _accessor;

        public HttpCurrentUser(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        private ClaimsPrincipal? P => _accessor.HttpContext?.User;

        public int UserId => int.TryParse(P?.FindFirst("Id")?.Value, out var v) ? v : 0;
        public string EmployeeNumber => P?.FindFirst("EmployeeNumber")?.Value ?? "";
        public string UserName => P?.FindFirst("UserName")?.Value ?? "";
        public IReadOnlyList<string> Roles => P?.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? new List<string>();
        public int? SupplierId => int.TryParse(P?.FindFirst("SupplierId")?.Value, out var s) && s > 0 ? s : null;
        public int IsSupplierAccount => int.TryParse(P?.FindFirst("IsSupplierAccount")?.Value, out var f) ? f : 0;
        public bool IsAdmin => Roles.Contains("Admin");
    }
}
