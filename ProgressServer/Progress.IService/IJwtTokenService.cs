using Progress.Model.Entitys;

namespace Progress.IService
{
    public interface IJwtTokenService
    {
        string CreateToken(Users user, IReadOnlyList<string> roleNames, int? supplierId);
    }
}
