<template>
  <div>
    <div class="app-container">
      <el-card shadow="never" class="searchTop">
        <div class="user-header">
          <el-form :inline="true" :model="menuFormInline" class="ef_1">
            <el-form-item :label="$t('systemMenu.menuName')+':'">
              <el-input
                v-model="menuFormInline.keywords"
                :placeholder="$t('systemMenu.menuNameTip')"
                clearable
                prefix-icon="Search"
                class="ei_1"
                style="width: 190px"
              />
            </el-form-item>
            <el-form-item :label="$t('systemMenu.menuStatus') + ':'">
              <el-select
                v-model="menuFormInline.enable"
                prefix-icon="Search"
                clearable
               :placeholder="$t('systemMenu.selectMenuStatus')"
                class="ei_1"
                style="width: 190px"
              >
            <el-option :label="$t('systemMenu.normal')" value="1" />
            <el-option :label="$t('systemMenu.disabled')" value="0" />
              </el-select>
            </el-form-item>
            <el-form-item>
              <el-button type="primary" :icon="Search" @click="handleSearch">{{ $t('common.search') }}</el-button>
              <el-button :icon="Refresh" @click="resetHandleSearch">{{ $t('common.reset') }}</el-button>
            </el-form-item>
          </el-form>
        </div>
      </el-card>
      <el-card shadow="never">
        <div class="tableTop">
          <el-button type="primary" @click="handleAdd">+{{ $t("common.add") }}</el-button>
          <el-button type="danger" @click="handleDelList" style="padding: 5px">{{ $t("common.deleteMany") }}</el-button>
        </div>
        <!-- 表格数据 -->
        <div class="table" ref="tableContanierRef">
          <el-table
            :default-expand-all="defaultExpandAll"
            :data="menuTableData"
            :height="tableHeight"
            ref="menuTableRef"
            row-key="id"
            border
            fit
            @selection-change="handleSelectionChange"
          >
            <el-table-column type="selection" align="center" width="40" />
            <!-- <el-table-column
        type="index"
        label="菜单编号"
        align="center"
        width="100"
      ></el-table-column> -->
            <el-table-column prop="title" :label="$t('systemMenu.menuName')" width="140" :show-overflow-tooltip="true"></el-table-column>
            <el-table-column prop="elIcon" :label="$t('systemMenu.icon')" align="center" width="80">
              <template #default="scope">
                <svg-icon :name="scope.row.elIcon" class="table-icon"></svg-icon>
                <!-- <component class="icons" :is="scope.row.elIcon"></component> -->
              </template>
            </el-table-column>
            <el-table-column prop="menuType" :label="$t('systemMenu.menuType')" align="center" width="80">
              <template #default="scope">
                <el-tag v-if="scope.row.menuType == 'C'">{{ $t('systemMenu.menu') }}</el-tag>
                <el-tag type="success" v-else-if="scope.row.menuType == 'M'">{{ $t('systemMenu.directory') }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column prop="menuSort" :label="$t('systemMenu.displayOrder')" align="center" width="110"></el-table-column>
            <el-table-column
              v-for="item in menuTableLabel"
              align="center"
              :key="item.prop"
              :prop="item.prop"
              :label="item.label"
              :show-overflow-tooltip="true"
            />
            <el-table-column :label="$t('systemMenu.menuStatus')" align="center" prop="enable" width="80">
              <template #default="scope">
                <el-tag v-if="scope.row.enable == 1">{{ $t('systemMenu.normal') }}</el-tag>
                <el-tag type="danger" v-else-if="scope.row.enable == 0">{{ $t('systemMenu.disabled') }}</el-tag>
              </template>
            </el-table-column>
            <el-table-column :label="$t('systemMenu.enableCache')" align="center" prop="keepAlive" width="90">
              <template #default="{ row }">
                <el-switch
                  v-show="row.menuType !== 'M'"
                  v-model="row.keepAlive"
                  inline-prompt
                  inactive-color="#f56c6c"
                  :active-text="$t('systemMenu.yes')"
                  :inactive-text="$t('systemMenu.no')"
                  @change="changeMenuStatu(row)"
                />
              </template>
            </el-table-column>
            <el-table-column fixed="right" align="center" :label="$t('common.operate')" width="150">
              <!--Vue组件中定义一个插槽，row为当前行的数据，从父组件中拿到的，可以打印{{row}}查看 -->
              <!--如果#default="scope" scope为作用域对象，你可以拿到这个vue模版中的所有的对象和方法 -->
              <template #default="{ row }">
                <!-- {{ scope.row }} -->
                <el-button link type="primary" icon="Edit" @click="handleEdit(row)">{{ $t("common.edit") }}</el-button>
                <el-button link type="danger" icon="Delete" @click="handleRemove(row)">{{ $t("common.delete") }}</el-button>
              </template>
            </el-table-column>
          </el-table>
        </div>
      </el-card>
    </div>
    <!--新增和编辑 dialogVisible：隐藏显示对话框视图 -->
    <el-dialog
      v-model="dialogVisible"
      :title="getAction.action === 'add' ? $t('systemMenu.addMenu') : $t('systemMenu.editMenu')"
      width="40%"
      append-to-body
      draggable
      :before-close="handleClose"
    >
      <el-form ref="formMenuRef" :model="formMenu" :rules="rules" label-width="100px">
        <el-row :gutter="10">
          <el-col :lg="24">
            <el-form-item :label="$t('systemMenu.parentMenu')">
              <el-tree-select
                class="treeSelect"
                v-model="formMenu.parentId"
                :data="menuOptions"
                :props="{
                  value: 'id',
                  label: 'title',
                  children: 'children'
                }"
                value-key="id"
                node-key="id"
                :show-count="true"
                :placeholder="$t('systemMenu.selectParentMenu')"
                check-strictly
              />
            </el-form-item>
          </el-col>
          <el-col :lg="24">
            <el-form-item :label="$t('systemMenu.menuType')" prop="menuType">
              <el-radio-group v-model="formMenu.menuType">
                <el-radio-button label="M">{{ $t('systemMenu.directory') }}</el-radio-button>
                <el-radio-button label="C">{{ $t('systemMenu.menu') }}</el-radio-button>
                <!-- <el-radio-button label="F">按钮</el-radio-button> -->
              </el-radio-group>
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item :label="$t('systemMenu.menuNameLabel')" prop="label">
              <el-input v-model="formMenu.title" :placeholder="$t('systemMenu.enterMenuName')" />
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item :label="$t('systemMenu.displayOrder')" prop="menuSort">
              <el-input-number v-model="formMenu.menuSort" controls-position="right" :min="0" :value="999" />
            </el-form-item>
          </el-col>
          <!-- Progress 扩展：显式路由 name，与 LineMes 仅用 path 推导不同；留空则按路径末段转驼峰 -->
          <el-col :lg="24">
            <el-form-item prop="name">
              <template #label>
                <span>
                  <el-tooltip :content="$t('systemMenu.routeNameTooltip')" placement="top">
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ $t("systemMenu.routeName") }}
                </span>
              </template>
              <el-input v-model="formMenu.name" :placeholder="$t('systemMenu.enterRouteName')" clearable />
            </el-form-item>
          </el-col>
          <el-col :lg="24" v-if="formMenu.menuType != 'F'">
            <el-form-item :label="$t('systemMenu.menuIcon')" prop="icon">
              <el-popover placement="bottom-start" :width="540" trigger="click">
                <template #reference>
                  <el-input v-model="formMenu.elIcon" :placeholder="$t('systemMenu.selectIcon')" readonly clearable>
                    <template #prefix>
                      <svg-icon v-if="formMenu.elIcon" :name="formMenu.elIcon" />
                      <el-icon v-else>
                        <search />
                      </el-icon>
                    </template>
                    <template #suffix>
                      <el-icon @click="selectedSvgIcon('')"><Delete /></el-icon>
                    </template>
                  </el-input>
                </template>
                <IconSelect ref="iconSelectRef" @selectedSvgIcon="selectedSvgIcon" />
              </el-popover>
              <!-- <el-input v-model="formMenu.icon"></el-input> -->
              <!-- <el-link type="danger" @click="form.icon = ''">清空</el-link> -->
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item v-if="formMenu.menuType != 'F'" :label="$t('systemMenu.menuStatusLabel')">
              <template #label>
                <span>
                  <el-tooltip :content="$t('systemMenu.statusTooltip')" placement="top">
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ $t('systemMenu.menuStatus') }}
                </span>
              </template>
              <el-select v-model="formMenu.enable">
                <el-option value="1" :label="$t('systemMenu.normal')" />
                <el-option value="0" :label="$t('systemMenu.disabled')" />
              </el-select>
            </el-form-item>
          </el-col>
          <el-col :lg="12">
            <el-form-item v-if="formMenu.menuType != 'F'" prop="path">
              <template #label>
                <span>
                  <el-tooltip :content="$t('systemMenu.routeAddressTooltip')" placement="top">
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ $t('systemMenu.routeAddress') }}
                </span>
              </template>
              <el-input v-model="formMenu.path" :placeholder="$t('systemMenu.enterRouteAddress')" />
            </el-form-item>
          </el-col>

          <el-col :lg="12" v-if="formMenu.menuType == 'C'">
            <el-form-item label="组件路径" prop="url">
              <template #label>
                <span>
                  <el-tooltip :content="$t('systemMenu.componentPathTooltip')" placement="top">
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ $t('systemMenu.componentPath') }}
                </span>
              </template>
              <el-input v-model="formMenu.url" :placeholder="$t('systemMenu.enterComponentPath')" />
            </el-form-item>
          </el-col>

          <!-- <el-col :lg="12">
            <el-form-item v-if="formMenu.menuType != 'M'" label="权限标识">
              <template #label>
                <span slot="label">
                  <el-tooltip content="控制器中定义的权限字符，如：'system:user:delete'" placement="top">
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  权限标识
                </span>
              </template>
              <el-input v-model="formMenu.perms" placeholder="规范([分类]:[页面]:[动作])" maxlength="50" />
            </el-form-item>
          </el-col> -->

          <el-col :lg="12">
            <el-form-item v-if="formMenu.menuType == 'C'" label="是否缓存">
              <template #label>
                <span>
                  <el-tooltip :content="$t('systemMenu.cacheTooltip')" placement="top">
                    <el-icon><question-filled /></el-icon>
                  </el-tooltip>
                  {{ $t('systemMenu.enableCache') }}
                </span>
              </template>
              <el-radio-group v-model="formMenu.keepAlive">
                <el-radio label="1">{{ $t('systemMenu.cached') }}</el-radio>
                <el-radio label="0">{{ $t('systemMenu.notCached') }}</el-radio>
              </el-radio-group>
            </el-form-item>
          </el-col>
        </el-row>
      </el-form>
      <div slot="footer" class="dialog-footer">
        <el-button @click="handleCancel()">{{ $t('common.cancel') }}</el-button>
        <el-button type="primary" @click="onSubmit()">{{ $t('common.confirm') }}</el-button>
      </div>
    </el-dialog>
  </div>
</template>
<script lang="ts" setup name="menu">
import {
  ComponentInternalInstance,
  getCurrentInstance,
  nextTick,
  onBeforeUnmount,
  onMounted,
  reactive,
  ref,
  onActivated
} from "vue"
import { Search, Refresh } from "@element-plus/icons-vue"
import { ElMessageBox, ElMessage } from "element-plus"
import {
  addMenuApi,
  batchDelMenuApi,
  changeMenuStatusApi,
  deleteMenuApi,
  getMenuDataApi,
  getAllMenuTreeSelectApi,
  updateMenuDataApi
} from "@/api/system/sys-menu"
import IconSelect from "@/components/svgIconSelect/index.vue"
import type { FormInstance, FormRules } from "element-plus"
import i18n from "@/locales"
const { proxy } = getCurrentInstance() as ComponentInternalInstance

// #region 属性值
/**表格容器引用 */
const tableContanierRef = ref()
/**表格自适应高度 */
const tableHeight = ref()
export interface Config {
  menuName: string
  enable: string
}
/**查询用户、分页、搜索用户实体 */
const config = reactive<Config>({
  menuName: "",
  enable: ""
})
/**双向绑定，为了关键词搜索 */
const menuFormInline = reactive({
  keywords: "",
  enable: ""
})
/**复用模态框,新增和编辑 */
const getAction = reactive({
  action: "add"
})
/**表格列配置 */
const menuTableLabel = reactive([
  // { label: "权限标识", prop: "perms" },
  { label: i18n.global.t("systemMenu.routeAddress"), prop: "path" },
  { label: i18n.global.t("systemMenu.componentPath"), prop: "url", width: "200" }
])
/**  表格数据*/
const menuTableData = ref()
/**表格是否展开 */
const defaultExpandAll = ref(false)
/**存储批量删除id */
let ids = reactive([] as any)
// #endregion

// #region 搜素-表格操作
/** 关键词搜索*/
const handleSearch = () => {
  config.menuName = menuFormInline.keywords
  config.enable = menuFormInline.enable
  getMenuData(config)
  defaultExpandAll.value = true
}

/**重置查询 */
const resetConfig = () => {
  Object.assign(config, {
    menuName: "",
    enable: ""
  })
}
/**重置搜索表单内容 */
const resetmenuFormInline = () => {
  Object.assign(menuFormInline, {
    keywords: "",
    enable: ""
  })
}
/**重置搜索 */
const resetHandleSearch = () => {
  defaultExpandAll.value = false
  resetmenuFormInline()
  resetConfig()
  getMenuData(config)
}

/** 获取菜单数据*/
const getMenuData = async (config: Config) => {
  // 调用接口
  let res: any = await getMenuDataApi(config)
  if (res) {
    console.log("%c [ res ]-336", "font-size:13px; background:pink; color:#bf2c9f;", res)
    const list = Array.isArray(res.data) ? res.data : []
    menuTableData.value = list.map((item: any) => {
      return changeAliveValue(item)
    })
  }
}

/**
 * 改变页面keepAlive的值，方便页面展示
 *  递归改变所有数据的keepAlive属性
 * @param item
 */
const changeAliveValue = (item: any) => {
  item.keepAlive = item.keepAlive === 1 ? true : false

  if (item.children) {
    item.children = item.children.map((child: any) => {
      return changeAliveValue(child)
    })
  }
  return item
}
/**改变用户状态（是否禁用）*/
const changeMenuStatu = (row: any) => {
  let status = row.keepAlive === true ? i18n.global.t('systemMenu.cached') : i18n.global.t('systemMenu.notCached')
  ElMessageBox.confirm(i18n.global.t('systemMenu.changeCacheConfirm', { status: status, title: row.title }), i18n.global.t('systemMenu.systemPrompt'), {
    confirmButtonText: i18n.global.t('common.confirm'),
    cancelButtonText: i18n.global.t('common.cancel'),
    type: "warning"
  })
    .then(async () => {
      const id = row.id
      const keepAlive = row.keepAlive == true ? 1 : 0
      let res: any = await changeMenuStatusApi({
        id: id,
        keepAlive: keepAlive
      })
      if (res) {
        ElMessage({
          type: "success",
          message: res.message
        })
        getMenuData(config)
      }
    })
    .catch(() => {
      row.keepAlive = row.keepAlive === true ? false : true
    })
}

/** 新增按钮*/
const handleAdd = () => {
  // 强制清除表单数据
  resetFormMenu()
  getTreeSelect()
  getAction.action = "add"
  dialogVisible.value = true
}
/** 编辑按钮*/
const handleEdit = (row: any) => {
  getTreeSelect()
  getAction.action = "edit"
  dialogVisible.value = true
  nextTick(() => {
    Object.assign(formMenu, row)
    formMenu.name = row.name != null ? String(row.name) : ""
    formMenu.parentId = row.parentId != null && row.parentId !== "" ? Number(row.parentId) : 0
    formMenu.menuSort = row.menuSort != null && row.menuSort !== "" ? Number(row.menuSort) : 0
    formMenu.enable = formMenu.enable.toString()
    formMenu.keepAlive = formMenu.keepAlive ? "1" : "0"
  })
}

/** 删除菜单 */
const handleRemove = (row: any) => {
  ElMessageBox.confirm(i18n.global.t('systemMenu.deleteConfirm'), i18n.global.t('systemMenu.prompt'), {
    confirmButtonText: i18n.global.t('common.confirm'),
    cancelButtonText: i18n.global.t('common.cancel'),
    type: "warning"
  })
    .then(async () => {
      let res: any = await deleteMenuApi({ id: row.id })
      if (res) {
        ElMessage({
          type: "success",
          message: res.message
        })
        getMenuData(config)
      }
    })
    .catch(() => {
      // catch error
    })
}
/**批量选中 */
const handleSelectionChange = (val: any) => {
  ids = <any>[]
  val.forEach((item: any) => {
    ids.push(item.id)
  })
}
/**批量删除 */
const handleDelList = () => {
  ElMessageBox.confirm(i18n.global.t('systemMenu.batchDeleteConfirm'), i18n.global.t('systemMenu.prompt'), {
    confirmButtonText: i18n.global.t('common.confirm'),
    cancelButtonText: i18n.global.t('common.cancel'),
    type: "warning"
  })
    .then(async () => {
      if (ids.length !== 0) {
        console.log("%c [ ids ]-557", "font-size:13px; background:pink; color:#bf2c9f;", ids)
        let res: any = await batchDelMenuApi(ids)
        if (res) {
          ElMessage({
            showClose: true,
            message: res.message,
            type: "success"
          })
          getMenuData(config)
          ids = []
        }
      } else {
        ElMessage({
          showClose: true,
          message: i18n.global.t('systemMenu.pleaseSelectMenu'),
          type: "warning"
        })
      }
    })
    .catch((err) => {})
}
// #endregion

// #region 其他操作-生命周期、公共函数
/**限制生命周期多次加载同一请求 */
const isMounted = ref(false)
/** 页面初始化*/
onMounted(() => {
  isMounted.value = true
  calculateTableHeight()
  window.addEventListener("resize", calculateTableHeight)
  getMenuData(config)
})
/**页面缓存被激活 */
onActivated(() => {
  if (!isMounted.value) {
    calculateTableHeight()
    window.addEventListener("resize", calculateTableHeight)
    getMenuData(config)
  }
  isMounted.value = false
})
/**监听页面高度，计算设置表格高度*/
const calculateTableHeight = () => {
  nextTick(() => {
    if (tableContanierRef.value) {
      const windowHeight = window.innerHeight
      const tableTopHeight = tableContanierRef.value.offsetTop
      const height = windowHeight - tableTopHeight - 25
      tableHeight.value = height + "px"
    }
  })
}
/**组件销毁 */
onBeforeUnmount(() => {
  window.removeEventListener("resize", calculateTableHeight) // 组件销毁前移除事件监听
})
// #endregion

// interface Menu {
//   id: number
//   label: string
//   children: Array[]
// }

// #region 对话框表单
/**新增菜单获取父节点数据下拉树结构数据 */
const menuOptions = ref<any>([])
/**表单ref */
const formMenuRef = ref()
/**对话框是否展示 */
const dialogVisible = ref()
/**添加用户的form数据接收绑定 */
const formMenu = reactive({
  id: "" as string | number,
  /** 与树「根菜单」id=0 一致，提交时规范为 int */
  parentId: 0 as number | string,
  menuType: "",
  perms: "",
  title: "",
  /** 路由 name，须与 sidebar.xxx 的键一致；留空则按路由地址末段转驼峰 */
  name: "",
  path: "",
  elIcon: "",
  url: "",
  menuSort: 999,
  enable: "",
  keepAlive: "0"
})
/**表单权限 */
const rules = reactive({
  title: [{ required: true, message: i18n.global.t('systemMenu.menuNameRequired'), trigger: "blur" }],
  elIcon: [{ required: true, message: i18n.global.t('systemMenu.iconRequired'), trigger: "blur" }],
  path: [{ required: true, message: i18n.global.t('systemMenu.routeAddressRequired'), trigger: "blur" }],
  url: [{ required: true, message: i18n.global.t('systemMenu.componentPathRequired'), trigger: "blur" }],
  menuSort: [{ required: true, message: i18n.global.t('systemMenu.sortRequired'), trigger: "blur" }]
})
/**重置彻底清除表单数据，防止数据莫名回显*/
const resetFormMenu = () => {
  Object.assign(formMenu, {
    id: "",
    parentId: 0,
    menuType: "M",
    title: "",
    name: "",
    path: "",
    elIcon: "",
    url: "",
    menuSort: 999,
    enable: "",
    keepAlive: ""
  })
}
/**表单取消时也需要重置表单内容 */
const handleCancel = () => {
  dialogVisible.value = false
  formMenuRef.value.resetFields()
}

/**对话框点击x关闭 */
const handleClose = (done: () => void) => {
  ElMessageBox.confirm(i18n.global.t('systemMenu.closeConfirm'), {
    confirmButtonText: i18n.global.t('common.confirm'),
    cancelButtonText: i18n.global.t('common.cancel')
  })
    .then(() => {
      formMenuRef.value.resetFields()
      done()
    })
    .catch(() => {
      // catch error
    })
}
/** 新增菜单获取父节点数据*/
const getTreeSelect = () => {
  menuOptions.value = []
  getAllMenuTreeSelectApi().then((res: any) => {
    const menu = { id: 0, title: "根菜单", children: [] }
    menu.children = res.data.menus
    menuOptions.value.push(menu)
    console.log("%c [ menuOptions.value ]-428", "font-size:13px; background:pink; color:#bf2c9f;", menuOptions.value)
  })
}
/** 路由地址末段 kebab/snake 转驼峰，供动态路由 name 与 langMap 对齐 */
const pathLastSegmentToCamel = (path: string) => {
  const raw = (path.match(/\/([^/]+)$/) || [])[1] || ""
  if (!raw) return ""
  const parts = raw.split(/[-_]/)
  return parts.map((p, i) => (i === 0 ? p : p.charAt(0).toUpperCase() + p.slice(1))).join("")
}

/** 在菜单树中查找节点 */
const findMenuNodeById = (nodes: any[], id: number | string): any | null => {
  for (const n of nodes || []) {
    if (n.id === id || String(n.id) === String(id)) return n
    const c = findMenuNodeById(n.children || [], id)
    if (c) return c
  }
  return null
}

/** 目录 M：按启用子菜单数计算 alwaysShow（1 个子且启用则扁平，0 或多个规则见产品） */
const alwaysShowForDirectory = (menuId: number | string | undefined): number => {
  const tree = menuTableData.value
  if (!tree || !Array.isArray(tree) || menuId === undefined || menuId === "") return 0
  const node = findMenuNodeById(tree, menuId)
  if (!node || !node.children?.length) return 0
  const n = node.children.filter((c: any) => c.enable === 1 || c.enable === "1").length
  return n > 1 ? 1 : 0
}

/** 后端 DTO 要求 int；树未选时 el-tree-select 可能为 "" */
const normalizeParentId = (): number => {
  const p = formMenu.parentId
  if (p === "" || p === null || p === undefined) return 0
  const n = Number(p)
  return Number.isFinite(n) ? n : 0
}

/** 解析提交用的路由 name：优先表单；误填路径时取末段转驼峰；否则由 path 末段转驼峰 */
const resolveRouteName = (): string => {
  let t = String(formMenu.name ?? "").trim()
  if (t.includes("/")) {
    const last = (t.match(/\/([^/]+)\/?$/) || [])[1] || ""
    t = last ? pathLastSegmentToCamel("/" + last) : ""
  }
  if (t) return t
  return pathLastSegmentToCamel(formMenu.path)
}

const normalizeMenuSort = (): string => String(formMenu.menuSort ?? "")

/**新增菜单，提交表单内容 */
const onSubmit = () => {
  // 表单内容验证，表单的ref属性拿到表单，使用el-api校验
  formMenuRef.value.validate(async (valid: FormInstance | undefined) => {
    if (valid) {
      const routeName = resolveRouteName()
      const parentId = normalizeParentId()
      const menuSortStr = normalizeMenuSort()
      const currentUser = localStorage.getItem("employeeNumber")
      const alwaysShowM =
        formMenu.menuType === "M"
          ? getAction.action === "edit"
            ? alwaysShowForDirectory(formMenu.id)
            : 0
          : 0
      if (getAction.action === "add") {
        let formMenudAdd = reactive({
          parentId,
          menuType: formMenu.menuType,
          name: routeName,
          title: formMenu.title,
          path: formMenu.path,
          elIcon: formMenu.elIcon,
          url: formMenu.menuType === "M" ? "Layouts" : formMenu.url,
          menuSort: menuSortStr,
          enable: formMenu.enable,
          keepAlive: formMenu.keepAlive === "" || formMenu.keepAlive === "0" ? 0 : 1,
          createBy: currentUser,
          alwaysShow: alwaysShowM,
          Redirect: ""
        })
        // await只会返回给他最近的async回调函数！！！否则报错
        const res: any = await addMenuApi(formMenudAdd)
        // 如果成功，则清空form表单，并关闭弹框
        if (res) {
          //需要注意的是：使用validate、resetFields，需要在表单内容上加prop绑定表单相应属性才会生效
          formMenuRef.value.resetFields()
          // 关闭弹框
          dialogVisible.value = false
          ElMessage({
            showClose: true,
            message: res.message,
            type: "success"
          })
          // 重新获取数据
          getMenuData(config)
        }
        // 编辑用户信息
      } else if (getAction.action === "edit") {
        let formMenudEdit = reactive({
          id: Number(formMenu.id),
          parentId,
          menuType: formMenu.menuType,
          name: routeName,
          title: formMenu.title,
          path: formMenu.path,
          elIcon: formMenu.elIcon,
          url: formMenu.menuType === "M" ? "Layouts" : formMenu.url,
          menuSort: menuSortStr,
          enable: formMenu.enable,
          keepAlive: formMenu.keepAlive,
          updateBy: currentUser,
          alwaysShow: alwaysShowM,
          Redirect: ""
        })
        const res: any = await updateMenuDataApi(formMenudEdit)
        if (res) {
          formMenuRef.value.resetFields()
          dialogVisible.value = false
          ElMessage({
            showClose: true,
            message: res.message,
            type: "success"
          })
          // 重新获取数据
          getMenuData(config)
        }
      }
    } else {
      ElMessage.error(i18n.global.t('systemMenu.pleaseFillComplete'))
    }
  })
}
/** 图标选择器*/
const iconSelectRef = ref()
const selectedSvgIcon = (name: string) => {
  console.log("%c [ name ]-617", "font-size:13px; background:pink; color:#bf2c9f;", name)
  formMenu.elIcon = name
}
// #endregion
</script>

<style lang="scss" scoped>
.searchTop {
  margin-bottom: 10px;
  :deep(.el-card__body) {
    padding-bottom: 2px;
  }
}
.tableTop {
  display: flex;
  justify-content: start;
  margin-bottom: 20px;
}
.table {
  display: flex;
  margin-bottom: 0px;
  height: 100%;
}

.el-input-number {
  width: 180px;
}
// 输入框宽度限制
.input_add {
  width: 196px;
  margin-right: 20px;
  &:hover {
    width: 196px;
  }
  &:focus {
    width: 196px;
  }
}
.icons {
  height: 18px;
  width: 18px;
}
.dialog-footer {
  display: flex;
  justify-content: end;
}
.treeSelect {
  width: 100% !important;
}
// 表格图标,i标签包裹。需要先设置i标签的大小，再deep到svg
// .table-icon {
//   width: 18px;
//   height: 18px;
//   :deep(svg) {
//     width: 100%;
//     height: 100%;
//   }
// }
</style>
