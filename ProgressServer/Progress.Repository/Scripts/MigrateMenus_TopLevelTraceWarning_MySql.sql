-- 将「追溯管理 / 预警管理」下的业务页提升为一级目录（与 DbSeed 新种子一致）。
-- 适用：仍使用旧版种子菜单 Id（6=trace,7=order,8=alert,9=process,10-12=craft,13=warning,14=rules）的 MySQL 库。
-- 执行前请备份数据库。若 Id=16 或 MenuRoles.Id=26/27 已占用，请先调整本脚本。

START TRANSACTION;

-- 1) 订单追溯：原 trace(6) -> orderTrace；子 order(7) -> path index
UPDATE `Menus`
SET `ParentId` = 0,
    `Name` = 'orderTrace',
    `Title` = '订单追溯',
    `Path` = '/order-records',
    `ElIcon` = 'document',
    `Url` = 'Layouts',
    `MenuType` = 'M',
    `MenuSort` = '0',
    `AlwaysShow` = 0
WHERE `Id` = 6;

UPDATE `Menus`
SET `ParentId` = 6,
    `Name` = 'orderRecords',
    `Path` = 'index',
    `Url` = '/trace/order-records/index',
    `MenuType` = 'C'
WHERE `Id` = 7;

-- 2) 预警记录：原 alert 子(8) 改为根 alertRecordsRoot，并新增子菜单(16) 承载页面
UPDATE `Menus`
SET `ParentId` = 0,
    `Name` = 'alertRecordsRoot',
    `Title` = '预警记录',
    `Path` = '/alert-records',
    `ElIcon` = 'Warning',
    `Url` = 'Layouts',
    `MenuType` = 'M',
    `MenuSort` = '1',
    `AlwaysShow` = 0
WHERE `Id` = 8;

INSERT INTO `Menus` (`Id`, `ParentId`, `Name`, `Title`, `Path`, `ElIcon`, `Url`, `MenuType`, `MenuSort`, `KeepAlive`, `Enable`, `AlwaysShow`, `Redirect`, `CreateTime`)
VALUES (16, 8, 'alertRecords', '预警记录', 'index', 'Warning', '/trace/alert-records/index', 'C', '0', 0, 1, 0, '', UTC_TIMESTAMP())
ON DUPLICATE KEY UPDATE
  `ParentId` = 8,
  `Name` = 'alertRecords',
  `Title` = '预警记录',
  `Path` = 'index',
  `Url` = '/trace/alert-records/index',
  `MenuType` = 'C';

-- 3) 预警管理：原 warning(13) -> warning；子(14) path index（子项仍为 warningRules）
UPDATE `Menus`
SET `ParentId` = 0,
    `Name` = 'warning',
    `Title` = '预警管理',
    `Path` = '/warning-rules',
    `ElIcon` = 'Bell',
    `Url` = 'Layouts',
    `MenuType` = 'M',
    `MenuSort` = '3',
    `AlwaysShow` = 0
WHERE `Id` = 13;

UPDATE `Menus`
SET `ParentId` = 13,
    `Name` = 'warningRules',
    `Path` = 'index',
    `Url` = '/warning/rules/index',
    `MenuType` = 'C'
WHERE `Id` = 14;

-- 4) 工艺管理排序（一级顺序：订单0 / 预警1 / 工艺2 / 规则3）
UPDATE `Menus` SET `MenuSort` = '2' WHERE `Id` = 9;

-- 5) 新子菜单(16) 的角色：Admin(1)、Supervisor(2)（旧种子 MenuRoles 最大 Id 一般为 25）
INSERT INTO `MenuRoles` (`Id`, `MenuId`, `RoleId`) VALUES (26, 16, 1), (27, 16, 2)
ON DUPLICATE KEY UPDATE `MenuId` = VALUES(`MenuId`), `RoleId` = VALUES(`RoleId`);

-- 6) 与侧边栏、langMap 对齐（按 Name；含曾用中间名 orderRecordsRoot / warningRulesRoot）
UPDATE `Menus` SET `Name` = 'orderTrace', `Title` = '订单追溯', `AlwaysShow` = 0 WHERE `Name` = 'orderRecordsRoot';
UPDATE `Menus` SET `Name` = 'warning', `Title` = '预警管理', `AlwaysShow` = 0 WHERE `Name` = 'warningRulesRoot';
UPDATE `Menus` SET `AlwaysShow` = 0 WHERE `Name` IN ('orderTrace', 'alertRecordsRoot', 'warning');
UPDATE `Menus` SET `MenuSort` = '2' WHERE `Name` = 'processManage';
UPDATE `Menus` SET `MenuSort` = '3' WHERE `Name` = 'warning';
UPDATE `Menus` SET `Title` = '工艺内容设置' WHERE `Name` = 'craftContent';
UPDATE `Menus` SET `Title` = '工艺配方设置' WHERE `Name` = 'craftRecipe';
UPDATE `Menus` SET `Title` = '工艺步序设置' WHERE `Name` = 'craftStep';
UPDATE `Menus` SET `Title` = '供应商管理' WHERE `Name` = 'supplierManage';

COMMIT;
