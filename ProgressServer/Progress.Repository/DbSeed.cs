using Microsoft.EntityFrameworkCore;
using Progress.Model;
using Progress.Model.Entitys;

namespace Progress.Repository
{
    public static class DbSeed
    {
        private const string Md5_123456 = "e10adc3949ba59abbe56e057f20f883e";

        public static async Task EnsureSeedAsync(ProgressDbContext db)
        {
            await db.Database.MigrateAsync();

            if (await db.Roles!.AnyAsync()) return;

            var adminRole = new Role
            {
                Id = 1,
                RoleName = "Admin",
                Description = "管理员",
                RoleSort = 0,
                Enable = 1,
                CreateTime = DateTime.UtcNow
            };
            var supRole = new Role
            {
                Id = 2,
                RoleName = "Supervisor",
                Description = "监督员",
                RoleSort = 1,
                Enable = 1,
                CreateTime = DateTime.UtcNow
            };
            var venRole = new Role
            {
                Id = 3,
                RoleName = "Supplier",
                Description = "供应商",
                RoleSort = 2,
                Enable = 1,
                CreateTime = DateTime.UtcNow
            };
            db.Roles!.AddRange(adminRole, supRole, venRole);

            var supplier = new Supplier
            {
                Id = 1,
                SupplierNumber = "1001",
                Name = "供应商A"
            };
            db.Suppliers!.Add(supplier);

            var admin = new Users
            {
                Id = 1,
                EmployeeNumber = "admin",
                UserName = "管理员",
                Password = Md5_123456,
                Enable = 1,
                IsDeleted = 0,
                IsSupplierAccount = 0,
                SupplierId = null,
                CreateTime = DateTime.UtcNow
            };
            var supervisor = new Users
            {
                Id = 2,
                EmployeeNumber = "supervisor",
                UserName = "监督员",
                Password = Md5_123456,
                Enable = 1,
                IsDeleted = 0,
                IsSupplierAccount = 0,
                SupplierId = null,
                CreateTime = DateTime.UtcNow
            };
            var vendor = new Users
            {
                Id = 3,
                EmployeeNumber = "supplier",
                UserName = "供应商账号",
                Password = Md5_123456,
                Enable = 1,
                IsDeleted = 0,
                IsSupplierAccount = 1,
                SupplierId = 1,
                CreateTime = DateTime.UtcNow
            };
            db.Users!.AddRange(admin, supervisor, vendor);

            db.UserRoles!.AddRange(
                new UserRole { Id = 1, UserId = 1, RoleId = 1 },
                new UserRole { Id = 2, UserId = 2, RoleId = 2 },
                new UserRole { Id = 3, UserId = 3, RoleId = 3 });

            // 菜单种子仅在空库首次创建角色时写入。若数据库已有角色，EnsureSeedAsync 会提前 return，此处不会执行。
            // 存量库请执行 Scripts/MigrateMenus_TopLevelTraceWarning_MySql.sql（基于旧种子 Id 6–8、13–14）。
            int mi = 1;
            void menu(Menu m) => db.Menus!.Add(m);

            menu(new Menu
            {
                Id = mi++,
                ParentId = 0,
                Name = "system",
                Title = "系统管理",
                Path = "/system",
                ElIcon = "Setting",
                Url = "Layouts",
                MenuType = "M",
                MenuSort = "9",
                KeepAlive = 0,
                Enable = 1,
                AlwaysShow = 1,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });
            menu(new Menu
            {
                Id = mi++,
                ParentId = 1,
                Name = "userManage",
                Title = "用户管理",
                Path = "user",
                ElIcon = "User",
                Url = "/system/user/index",
                MenuType = "C",
                MenuSort = "0",
                KeepAlive = 1,
                Enable = 1,
                AlwaysShow = 0,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });
            menu(new Menu
            {
                Id = mi++,
                ParentId = 1,
                Name = "menuManage",
                Title = "菜单管理",
                Path = "menu",
                ElIcon = "Menu",
                Url = "/system/menu/index",
                MenuType = "C",
                MenuSort = "1",
                KeepAlive = 1,
                Enable = 1,
                AlwaysShow = 0,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });
            menu(new Menu
            {
                Id = mi++,
                ParentId = 1,
                Name = "roleManage",
                Title = "角色管理",
                Path = "role",
                ElIcon = "Key",
                Url = "/system/role/index",
                MenuType = "C",
                MenuSort = "2",
                KeepAlive = 1,
                Enable = 1,
                AlwaysShow = 0,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });
            menu(new Menu
            {
                Id = mi++,
                ParentId = 1,
                Name = "supplierManage",
                Title = "供应商管理",
                Path = "supplier",
                ElIcon = "Shop",
                Url = "/system/supplier/index",
                MenuType = "C",
                MenuSort = "3",
                KeepAlive = 1,
                Enable = 1,
                AlwaysShow = 0,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });

            // 业务侧栏顺序：订单追溯 → 预警记录 → 工艺管理（三子项）→ 预警管理 → 系统管理（四子项）。
            // 「首页」由前端常量路由提供，不在菜单表。订单记录 / 预警记录 / 预警规则：一级目录（各为 M + Layouts + 单页 C）
            menu(new Menu
            {
                Id = mi++,
                ParentId = 0,
                Name = "orderTrace",
                Title = "订单追溯",
                Path = "/order-records",
                ElIcon = "document",
                Url = "Layouts",
                MenuType = "M",
                MenuSort = "0",
                KeepAlive = 0,
                Enable = 1,
                AlwaysShow = 0,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });
            menu(new Menu
            {
                Id = mi++,
                ParentId = 6,
                Name = "orderRecords",
                Title = "订单记录",
                Path = "index",
                ElIcon = "document",
                Url = "/trace/order-records/index",
                MenuType = "C",
                MenuSort = "0",
                KeepAlive = 1,
                Enable = 1,
                AlwaysShow = 0,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });
            menu(new Menu
            {
                Id = mi++,
                ParentId = 0,
                Name = "alertRecordsRoot",
                Title = "预警记录",
                Path = "/alert-records",
                ElIcon = "Warning",
                Url = "Layouts",
                MenuType = "M",
                MenuSort = "1",
                KeepAlive = 0,
                Enable = 1,
                AlwaysShow = 0,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });
            menu(new Menu
            {
                Id = mi++,
                ParentId = 8,
                Name = "alertRecords",
                Title = "预警记录",
                Path = "index",
                ElIcon = "Warning",
                Url = "/trace/alert-records/index",
                MenuType = "C",
                MenuSort = "0",
                KeepAlive = 0,
                Enable = 1,
                AlwaysShow = 0,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });

            menu(new Menu
            {
                Id = mi++,
                ParentId = 0,
                Name = "processManage",
                Title = "工艺管理",
                Path = "/process",
                ElIcon = "Cpu",
                Url = "Layouts",
                MenuType = "M",
                MenuSort = "2",
                KeepAlive = 0,
                Enable = 1,
                AlwaysShow = 1,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });
            menu(new Menu
            {
                Id = mi++,
                ParentId = 10,
                Name = "craftContent",
                Title = "工艺内容设置",
                Path = "craft-content",
                ElIcon = "Notebook",
                Url = "/process/craft-content/index",
                MenuType = "C",
                MenuSort = "0",
                KeepAlive = 1,
                Enable = 1,
                AlwaysShow = 0,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });
            menu(new Menu
            {
                Id = mi++,
                ParentId = 10,
                Name = "craftRecipe",
                Title = "工艺配方设置",
                Path = "craft-recipe",
                ElIcon = "Document",
                Url = "/process/craft-recipe/index",
                MenuType = "C",
                MenuSort = "1",
                KeepAlive = 0,
                Enable = 1,
                AlwaysShow = 0,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });
            menu(new Menu
            {
                Id = mi++,
                ParentId = 10,
                Name = "craftStep",
                Title = "工艺步序设置",
                Path = "craft-step",
                ElIcon = "Sort",
                Url = "/process/craft-step/index",
                MenuType = "C",
                MenuSort = "2",
                KeepAlive = 0,
                Enable = 1,
                AlwaysShow = 0,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });

            menu(new Menu
            {
                Id = mi++,
                ParentId = 0,
                Name = "warning",
                Title = "预警管理",
                Path = "/warning-rules",
                ElIcon = "Bell",
                Url = "Layouts",
                MenuType = "M",
                MenuSort = "3",
                KeepAlive = 0,
                Enable = 1,
                AlwaysShow = 0,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });
            menu(new Menu
            {
                Id = mi++,
                ParentId = 14,
                Name = "warningRules",
                Title = "预警规则",
                Path = "index",
                ElIcon = "AlarmClock",
                Url = "/warning/rules/index",
                MenuType = "C",
                MenuSort = "0",
                KeepAlive = 1,
                Enable = 1,
                AlwaysShow = 0,
                Redirect = "",
                CreateTime = DateTime.UtcNow
            });

            void mr(int id, int menuId, int roleId) =>
                db.MenuRoles!.Add(new MenuRole { Id = id, MenuId = menuId, RoleId = roleId });

            // Admin：全部菜单 1–15
            mr(1, 1, 1); mr(2, 2, 1); mr(3, 3, 1); mr(4, 4, 1); mr(5, 5, 1);
            mr(6, 6, 1); mr(7, 7, 1); mr(8, 8, 1); mr(9, 9, 1); mr(10, 10, 1);
            mr(11, 11, 1); mr(12, 12, 1); mr(13, 13, 1); mr(14, 14, 1); mr(15, 15, 1);
            // Supervisor：业务菜单 6–15（不含系统管理）
            mr(16, 6, 2); mr(17, 7, 2); mr(18, 8, 2); mr(19, 9, 2); mr(20, 10, 2);
            mr(21, 11, 2); mr(22, 12, 2); mr(23, 13, 2); mr(24, 14, 2); mr(25, 15, 2);
            // Supplier：订单记录（根 + 页）
            mr(26, 6, 3); mr(27, 7, 3);

            db.Crafts!.Add(new Craft { Id = 1, Code = 1, Name = "下料", RecipeBody = "[]" });
            db.Crafts!.Add(new Craft { Id = 2, Code = 2, Name = "粗加工", RecipeBody = "[]" });
            db.Crafts!.Add(new Craft { Id = 3, Code = 3, Name = "精加工", RecipeBody = "[]" });
            db.Crafts!.Add(new Craft { Id = 4, Code = 4, Name = "热处理", RecipeBody = "[]" });
            db.Crafts!.Add(new Craft { Id = 5, Code = 5, Name = "表面处理", RecipeBody = "[]" });
            db.Crafts!.Add(new Craft { Id = 6, Code = 6, Name = "焊接", RecipeBody = "[]" });
            db.Crafts!.Add(new Craft { Id = 7, Code = 7, Name = "折弯", RecipeBody = "[]" });
            db.Crafts!.Add(new Craft { Id = 8, Code = 8, Name = "检验", RecipeBody = "[]" });
            var recipe = new CraftRecipe { Id = 1, Code = 1, Name = "默认配方" };
            db.CraftRecipes!.Add(recipe);
            db.CraftRecipeSteps!.Add(new CraftRecipeStep
            {
                Id = 1,
                CraftRecipeId = 1,
                CraftId = 1,
                StepOrder = 1
            });
            db.CraftRecipeSteps!.Add(new CraftRecipeStep
            {
                Id = 2,
                CraftRecipeId = 1,
                CraftId = 2,
                StepOrder = 2
            });
            db.CraftRecipeSteps!.Add(new CraftRecipeStep
            {
                Id = 3,
                CraftRecipeId = 1,
                CraftId = 3,
                StepOrder = 3
            });
            db.CraftRecipeSteps!.Add(new CraftRecipeStep
            {
                Id = 4,
                CraftRecipeId = 1,
                CraftId = 4,
                StepOrder = 4
            });
            db.CraftRecipeSteps!.Add(new CraftRecipeStep
            {
                Id = 5,
                CraftRecipeId = 1,
                CraftId = 5,
                StepOrder = 5
            });
            db.CraftRecipeSteps!.Add(new CraftRecipeStep
            {
                Id = 6,
                CraftRecipeId = 1,
                CraftId = 6,
                StepOrder = 6
            });
            db.CraftRecipeSteps!.Add(new CraftRecipeStep
            {
                Id = 7,
                CraftRecipeId = 1,
                CraftId = 7,
                StepOrder = 7
            });
            db.CraftRecipeSteps!.Add(new CraftRecipeStep
            {
                Id = 8,
                CraftRecipeId = 1,
                CraftId = 8,
                StepOrder = 8
            });


            db.AlertSettings!.Add(new AlertSetting { Id = 1, LeadDays = 3, Enabled = true });

            db.WorkpieceOrderLines!.Add(new WorkpieceOrderLine
            {
                Id = 1,
                LineNo = 1,
                PoNumber = "PO20260001",
                ProjectCode = "PRJ-01",
                DrawingNumber = "DWG-001",
                PartName = "示例零件",
                SupplierId = 1,
                Quantity = 10,
                ShippedQuantity = 10,
                Unit = "件",
                RequiredDeliveryDate = DateTime.UtcNow.AddDays(5),
                CraftRecipeId = 1,
                LatestCraftCode = 1,
                ShippingStatus = OrderShippingStatus.NotShipped,
                CreateTime = DateTime.UtcNow
            });

            await db.SaveChangesAsync();
        }
    }
}
