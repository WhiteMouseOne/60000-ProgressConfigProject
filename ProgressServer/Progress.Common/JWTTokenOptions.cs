namespace Progress.Common
{
    /// <summary>与 LineMesServer MES.Common.JWTTokenOptions 一致，供 JWT 配置节绑定。</summary>
    public class JWTTokenOptions
    {
        public string Audience { get; set; } = "";
        public string SecurityKey { get; set; } = "";
        public string Issuer { get; set; } = "";
    }
}
