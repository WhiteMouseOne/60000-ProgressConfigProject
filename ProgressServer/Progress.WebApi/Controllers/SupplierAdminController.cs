using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.Login;
using Progress.Model.Dto.SystemManagement;

namespace Progress.WebApi.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class SupplierAdminController : ControllerBase
    {
        private readonly ISupplierAdminService _svc;
        public SupplierAdminController(ISupplierAdminService svc)
        {
            _svc = svc;
        }

        [HttpGet]
        public async Task<ApiResponseData> List()
        {
            var list = await _svc.ListAsync();
            return new ApiResponseData { code = 200, data = list };
        }

        /// <summary>
        /// 查询供应商列表，支持根据供应商编号、名称和启用状态进行过滤，并返回满足条件的供应商总数和分页后的供应商列表。
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponseData> Query([FromBody] SupplierQueryRequest req)
        {
            var (total, dataList) = await _svc.QueryAsync(req);
            return new ApiResponseData
            {
                code = 200,
                data = new { total, dataList }
            };
        }

        /// <summary>
        /// 创建供应商，接收一个SupplierCreateDto对象作为请求体，
        /// 调用ISupplierAdminService的CreateAsync方法创建供应商，并返回操作结果的状态码和消息。
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResponseData> Create([FromBody] SupplierCreateDto body)
        {
            var (ok, msg) = await _svc.CreateAsync(body);
            return new ApiResponseData { code = ok? 200:400,message=msg };
        }

        /// <summary>
        /// 更新供应商，接收一个整数id作为路径参数和一个SupplierAdminUpdateDto对象作为请求体，
        /// id:int说明ID是从 URL 路径传
        /// </summary>
        /// <param name="id"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("{id:int}")]
        public async Task<ApiResponseData> Update(int id, [FromBody]SupplierAdminUpdateDto body)
        {
            var (ok, msg) = await _svc.UpdateAsync(id, body);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        /// <summary>
        /// 删除供应商
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost("{id:int}")]
        public async Task<ApiResponseData> Delete(int id)
        {
            var (ok, msg) = await _svc.DeleteAsync(id);
            return new ApiResponseData { code = ok ? 200 : 400, message = msg };
        }

        [HttpPost]
        public async Task<ApiResponseData> BatchDelete([FromBody] SupplierBatchDeleteDto body)
        {
            var (ok, msg, _) = await _svc.BatchDeleteAsync(body.Ids);
            return new ApiResponseData { code = ok?200:400,message = msg };
        }

    }
}
