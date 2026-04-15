using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Login;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin,Supervisor,Supplier")]
    public class MetaController : ControllerBase
    {
        private readonly IMetaService _meta;

        public MetaController(IMetaService meta)
        {
            _meta = meta;
        }

        [HttpGet]
        public async Task<ApiResponseData> Suppliers()
        {
            var list = await _meta.GetSuppliersForCurrentUserAsync();
            return new ApiResponseData { code = 200, data = list };
        }

        [HttpGet]
        public async Task<ApiResponseData> CraftRecipes()
        {
            var list = await _meta.GetCraftRecipesAsync();
            return new ApiResponseData { code = 200, data = list };
        }

        [HttpGet]
        public async Task<ApiResponseData> CraftRecipeCrafts([FromQuery] int craftRecipeId)
        {
            var list = await _meta.GetCraftRecipeCraftsAsync(craftRecipeId);
            return new ApiResponseData { code = 200, data = list };
        }
    }
}
