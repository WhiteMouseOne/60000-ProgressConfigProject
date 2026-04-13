using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.Business;
using Progress.Model.Dto.Login;
using Progress.Model.Dto.Order;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin,Supervisor,Supplier")]
    public class OrderLineController : ControllerBase
    {
        private readonly IOrderLineService _svc;

        public OrderLineController(IOrderLineService svc)
        {
            _svc = svc;
        }

        [HttpPost]
        public async Task<ApiResponseData> Query([FromBody] OrderLineQuery q)
        {
            try
            {
                var page = await _svc.QueryAsync(q);
                return new ApiResponseData { code = 200, message = "OK", data = page };
            }
            catch (Exception ex)
            {
                return new ApiResponseData { code = 500, message = ex.Message };
            }
        }

        [HttpGet]
        public async Task<ApiResponseData> Get(int id)
        {
            var row = await _svc.GetByIdAsync(id);
            if (row == null) return new ApiResponseData { code = 404, message = "未找到" };
            return new ApiResponseData { code = 200, data = row };
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<ApiResponseData> Create([FromBody] OrderLineCreateOrEdit body)
        {
            try
            {
                var newId = await _svc.CreateAsync(body);
                return new ApiResponseData { code = 200, message = "OK", data = new { id = newId } };
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseData { code = 403, message = "无权操作" };
            }
            catch (Exception ex)
            {
                return new ApiResponseData { code = 500, message = ex.Message };
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<ApiResponseData> Update([FromBody] OrderLineCreateOrEdit body)
        {
            try
            {
                var ok = await _svc.UpdateAsync(body);
                if (!ok) return new ApiResponseData { code = 404, message = "未找到" };
                return new ApiResponseData { code = 200, message = "OK" };
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseData { code = 403, message = "无权操作" };
            }
            catch (Exception ex)
            {
                return new ApiResponseData { code = 500, message = ex.Message };
            }
        }

        [HttpPost("{id:int}")]
        [Authorize(Roles = "Admin,Supervisor")]
        public async Task<ApiResponseData> Delete(int id)
        {
            try
            {
                var ok = await _svc.DeleteAsync(id);
                if (!ok) return new ApiResponseData { code = 404, message = "未找到" };
                return new ApiResponseData { code = 200, message = "OK" };
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseData { code = 403, message = "无权操作" };
            }
            catch (Exception ex)
            {
                return new ApiResponseData { code = 500, message = ex.Message };
            }
        }

        [HttpPost("{id:int}")]
        [Authorize(Roles = "Supplier")]
        public async Task<ApiResponseData> SupplierUpdate(int id, [FromBody] SupplierLineUpdate body)
        {
            try
            {
                var ok = await _svc.SupplierUpdateAsync(id, body);
                if (!ok) return new ApiResponseData { code = 404, message = "未找到" };
                return new ApiResponseData { code = 200, message = "OK" };
            }
            catch (UnauthorizedAccessException)
            {
                return new ApiResponseData { code = 403, message = "无权操作" };
            }
        }
    }
}
