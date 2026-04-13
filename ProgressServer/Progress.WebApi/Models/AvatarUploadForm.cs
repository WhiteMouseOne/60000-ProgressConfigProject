namespace Progress.WebApi.Models
{
    /// <summary>头像上传表单。使用单一 [FromForm] 模型，避免 Swashbuckle 对 IFormFile + [FromForm] 多参数生成 swagger.json 时抛错。</summary>
    public class AvatarUploadForm
    {
        public IFormFile? file { get; set; }
        public string? employeeNumber { get; set; }
    }
}
