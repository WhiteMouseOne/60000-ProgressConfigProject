/**路径映射文件,不需要拼接字符串可直接langMap.属性调用 */
const langMap = {
  //所有属性路径映射，后续方便修改
  login_username: "login.username",
  login_password: "login.password",
  login_login: "login.login",
  //头部
  header_PersonalCenter: "header.PersonalCenter",
  header_Logout: "header.Logout",
  //侧边栏
  sidebar_Dashboard: "sidebar.Dashboard",
  sidebar_dashboard: "sidebar.dashboard",
  sidebar_system: "sidebar.system",
  sidebar_user: "sidebar.user",
  sidebar_menu: "sidebar.menu",
  sidebar_role: "sidebar.role",
  sidebar_machine: "sidebar.machine",
  sidebar_workshop: "sidebar.workshop",
  sidebar_productionLine: "sidebar.productionLine",
  sidebar_workstation: "sidebar.workstation",
  sidebar_process: "sidebar.process",
  sidebar_product: "sidebar.product",
  sidebar_workOrder: "sidebar.workOrder",
  sidebar_part: "sidebar.part",
  sidebar_processroutelines: "sidebar.processroutelines",
  sidebar_lines: "sidebar.lines",
  sidebar_recipeType: "sidebar.recipeType",
  sidebar_recipe: "sidebar.recipe",
  sidebar_recipeConfig: "sidebar.recipeConfig",
  sidebar_userRecipe: "sidebar.userRecipe",
  sidebar_material: "sidebar.material",
  sidebar_trace: "sidebar.trace",
  sidebar_processRecord: "sidebar.processRecord",
  sidebar_file: "sidebar.file",
  sidebar_standardDocument: "sidebar.standardDocument",
  sidebar_pointInspection: "sidebar.pointInspection",
  sidebar_alarm: "sidebar.alarm",
  sidebar_alarmStorage: "sidebar.alarmStorage",
  sidebar_daConfig: "sidebar.daConfig",
  sidebar_flowConfig: "sidebar.flowConfig",
  sidebar_pcSetting: "sidebar.pcSetting",
  sidebar_plcSetting: "sidebar.plcSetting",
  sidebar_mesConfig: "sidebar.mesConfig",
  sidebar_processVar: "sidebar.processVar",
  sidebar_headerAds: "sidebar.headerAds",
  sidebar_variableAds: "sidebar.variableAds",
  sidebar_alarmConfig: "sidebar.alarmConfig",
  sidebar_recipeManage: "sidebar.recipeManage",
  sidebar_progress: "sidebar.progress",
  sidebar_orderTrace: "sidebar.orderTrace",
  sidebar_supplierAdmin: "sidebar.supplierAdmin",
  sidebar_craftAdmin: "sidebar.craftAdmin",
  sidebar_alertCfg: "sidebar.alertCfg",
  sidebar_userManage: "sidebar.userManage",
  sidebar_menuManage: "sidebar.menuManage",
  sidebar_roleManage: "sidebar.roleManage",
  sidebar_supplierManage: "sidebar.supplierManage",
  sidebar_orderRecords: "sidebar.orderRecords",
  sidebar_alertRecords: "sidebar.alertRecords",
  sidebar_processManage: "sidebar.processManage",
  sidebar_craftContent: "sidebar.craftContent",
  sidebar_craftRecipe: "sidebar.craftRecipe",
  sidebar_craftStep: "sidebar.craftStep",
  sidebar_warning: "sidebar.warning",
  sidebar_warningRules: "sidebar.warningRules",
  sidebar_alertRecordsRoot: "sidebar.alertRecordsRoot",
  /** 须与路由 name 一致：getLangValue('sidebar', 'testRecords') → 键名 sidebar_testRecords */
  sidebar_testRecords: "sidebar.testRecords"
}
const langMapKeys = Object.keys(langMap)

/** 侧边栏路由 name 对应的 i18n 路径；无映射时返回 undefined（调用方应使用 meta.title） */
export function getSidebarI18nPath(routeName: string | symbol | undefined): string | undefined {
  if (routeName == null || typeof routeName !== "string" || routeName === "") return undefined
  const key = `sidebar_${routeName}` as keyof typeof langMap
  return key in langMap ? (langMap[key] as string) : undefined
}

/**根据前缀和后缀获取对应的value（对应语言包的路径） */
const getLangValue = (prefixKey: string, suffixKey: any) => {
  const key = `${prefixKey}_${suffixKey}` as keyof typeof langMap
  //console.log("%c [ key ]-48", "font-size:13px; background:pink; color:#bf2c9f;", key)
  if (key in langMap) {
    return langMap[key]
  }
  return "属性不存在"
}
export { langMap, langMapKeys, getLangValue }
