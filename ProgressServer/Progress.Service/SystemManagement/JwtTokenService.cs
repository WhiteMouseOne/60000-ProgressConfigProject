using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Progress.Common;
using Progress.IService;
using Progress.Model.Entitys;

namespace Progress.Service.SystemManagement
{
    public class JwtTokenService : IJwtTokenService
    {
        private readonly JWTTokenOptions _opt;

        public JwtTokenService(IOptions<JWTTokenOptions> opt)
        {
            _opt = opt.Value;
        }

        public string CreateToken(Users user, IReadOnlyList<string> roleNames, int? supplierId)
        {
            var claims = new List<Claim>
            {
                new("Id", user.Id.ToString()),
                new("EmployeeNumber", user.EmployeeNumber),
                new("UserName", user.UserName),
                new("SupplierId", (supplierId ?? 0).ToString()),
                new("IsSupplierAccount", user.IsSupplierAccount.ToString())
            };
            foreach (var r in roleNames)
                claims.Add(new Claim(ClaimTypes.Role, r));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.SecurityKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1440),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
