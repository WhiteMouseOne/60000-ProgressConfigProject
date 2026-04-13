using Microsoft.EntityFrameworkCore;
using Progress.IService;
using Progress.IService.SystemManagement;
using Progress.Model.Dto.SystemManagement;
using Progress.Model.Entitys;
using Progress.Repository;

namespace Progress.Service.SystemManagement
{
    public class SupplierAdminService : ISupplierAdminService
    {
        private readonly ProgressDbContext _db;
        private readonly ICurrentUser _user;//获取当前用户信息。

        public SupplierAdminService(ProgressDbContext db,ICurrentUser user)
        {
            _db = db;
            _user = user;
        }

        public async Task<List<SupplierRowDto>> ListAsync()
        {
            //返回所有供应商列表，按照编号排序，将每个供应商信息映射为SupplierRowDto对象，并以列表形式返回。
            return await _db.Suppliers!.AsNoTracking()
                .OrderBy(x => x.SupplierNumber)
                .Select(x => new SupplierRowDto
                {
                    Id = x.Id,
                    SupplierNumber = x.SupplierNumber,
                    Name = x.Name
                })
                .ToListAsync();
        }

        /// <summary>
        /// 查询供应商列表，支持根据供应商编号、名称和启用状态进行过滤，并返回满足条件的供应商总数和分页后的供应商列表。
        /// </summary>
        /// <param name="req">查询条件</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<(int total, List<SupplierListRow> list)> QueryAsync(SupplierQueryRequest req,CancellationToken ct = default)
        {
            //获取供应商数据的查询对象，使用AsNoTracking()方法提高查询性能。
            var q = _db.Suppliers!.AsNoTracking();
            //如果请求中包含供应商编号，则在查询对象上添加过滤条件，筛选出编号包含指定字符串的供应商。
            if (!string.IsNullOrWhiteSpace(req.supplierNumber))
                q = q.Where(x => x.SupplierNumber.Contains(req.supplierNumber));
            //如果请求中包含供应商名称，则在查询对象上添加过滤条件，筛选出名称包含指定字符串的供应商。
            if (!string.IsNullOrWhiteSpace(req.name))
                q = q.Where(x => x.Name.Contains(req.name));
            //如果请求中包含启用状态，则在查询对象上添加过滤条件，筛选出启用状态等于指定值的供应商。
            if (req.enable is int en)
                q = q.Where(x => x.Enable == en);

            //计算满足查询条件的供应商总数，并将结果存储在total变量中。
            var total = await q.CountAsync(ct);
            var page = Math.Max(1, req.page);
            var size = Math.Max(1, req.size);
            //获取满足条件的供应商列表，按照ID升序，跳过前面(page-1)*size条记录，获取size条记录，
            //将每个供应商信息映射为SupplierListRow对象，并以列表形式返回。
            var list = await q.OrderBy(x => x.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .Select(x => new SupplierListRow
                {
                    id = x.Id,
                    supplierNumber = x.SupplierNumber,
                    name = x.Name,
                    enable = x.Enable,
                    createTime = x.CreateTime,
                    createBy = x.CreateBy,
                    updateTime = x.UpdateTime,
                    updateBy = x.UpdateBy
                })
                .ToListAsync(ct);
            return (total, list);
        }
        
        /// <summary>
        /// 创建供应商
        /// </summary>
        /// <param name="dto">供应商信息</param>
        /// <param name="ct">取消令牌[控制“是否中途取消这次异步任务”]</param>
        /// <returns></returns>
        public async Task<(bool ok, string message)> CreateAsync(SupplierCreateDto dto, CancellationToken ct = default)
        { 
            var sn = dto.SupplierNumber.Trim();//去除供应商编号两端的空格。
            var name = dto.Name.Trim();//去除供应商名称两端的空格。
            if(string.IsNullOrEmpty(sn))
                return (false, "供应商编号不能为空");
            if (string.IsNullOrEmpty(name))
                return (false, "供应商名称不能为空");
            if (await _db.Suppliers!.AnyAsync(x => x.SupplierNumber == sn, ct))
                return (false, "供应商编号已存在");

            var now = DateTime.Now;
            var by = _user.EmployeeNumber;
            if (string.IsNullOrEmpty(by))
                by = "system";

            _db.Suppliers!.Add(new Supplier
            {
                SupplierNumber = sn,
                Name = name,
                Enable = dto.Enable == 0 ? 0 :1,
                CreateTime = now,
                CreateBy = by,
                UpdateTime = now,
                UpdateBy = by
            });
            await _db.SaveChangesAsync(ct);
            return (true, "供应商创建成功");
        }

        /// <summary>
        /// 更新供应商信息
        /// </summary>
        /// <param name="id">供应商ID</param>
        /// <param name="dto">需要更新的供应商信息</param>
        /// <param name="ct">取消令牌</param>
        /// <returns></returns>
        public async Task<(bool ok, string message)> UpdateAsync(int id, SupplierAdminUpdateDto dto, CancellationToken ct = default)
        {
            //从Supliers表中异步查询，如果找到则返回供应商对象，否则返回null，支持取消操作。
            var s = await _db.Suppliers!.FirstOrDefaultAsync(x => x.Id == id, ct);
            if (s == null) return (false, "供应商不存在");
            if (!string.IsNullOrWhiteSpace(dto.Name))
                s.Name = dto.Name.Trim();
            if(!string.IsNullOrWhiteSpace(dto.SupplierNumber))
            {
                var sn = dto.SupplierNumber.Trim();
                if (await _db.Suppliers!.AnyAsync(x => x.SupplierNumber == sn && x.Id != id, ct))
                    return (false, "供应商编号已占用");
                s.SupplierNumber = sn;
            }

            if (dto.Enable is int en)
                s.Enable = en == 0 ? 0 : 1;

            s.UpdateTime = DateTime.Now;
            s.UpdateBy = string.IsNullOrEmpty(_user.EmployeeNumber) ? "system" : _user.EmployeeNumber;

            await _db.SaveChangesAsync(ct);
            return (true, "供应商信息更新成功");
        }

        /// <summary>
        /// 删除供应商【检查Users表和WorkpieceOrderLines表是否有绑定该供应商的记录】
        /// </summary>
        /// <param name="id">供应商ID</param>
        /// <param name="ct">取消令牌</param>
        /// <returns></returns>
        public async Task<(bool ok, string message)> DeleteAsync(int id, CancellationToken ct = default)
        {
            //异步查询Users表，如果有和要删除的供应商绑定，则返回false和提示信息。
            if (await _db.Users!.AnyAsync(x => x.SupplierId == id, ct))
                return (false, "存在绑定该供应商的用户，无法删除");
            //异步查询WorkpieceOrderLines表，如果有和要删除的供应商绑定，则返回false和提示信息。
            if(await _db.WorkpieceOrderLines!.AnyAsync(x=> x.SupplierId==id,ct))
                return (false, "存在光联订单，无法删除");

            var s = await _db.Suppliers!.FirstOrDefaultAsync(x => x.Id == id, ct);
            if(s==null)
                return (false, "供应商不存在");
            _db.Suppliers.Remove(s);
            await _db.SaveChangesAsync(ct);
            return (true, "供应商已成功");
        }

        /// <summary>
        /// 批量删除供应商【循环调用DeleteAsync方法，遇到无法删除的供应商时返回错误信息和已成功删除的数量】。
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<(bool ok,string message,int deleted)> BatchDeleteAsync(IReadOnlyList<int> ids, CancellationToken ct = default)
        {
            if (ids == null || ids.Count == 0)
                return (false, "未选择任何供应商", 0);
            var deleted = 0;
            foreach(var id in ids)
            {
                var (ok, msg) = await DeleteAsync(id, ct);
                if (!ok) return (false, $"ID{id}:{msg}", deleted);
                deleted++;
            }
            return (true, $"成功删除{deleted}个供应商", deleted);
        }
    }
}
