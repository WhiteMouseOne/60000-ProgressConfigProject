<template>
  <div class="app-container">
    <el-card class="searchTop" shadow="never">
      <div class="searForm">
        <el-form :inline="true" :model="searchFormInline" class="ef_1">
          <el-form-item :label="$t('user.account') + ' :'">
            <el-input
              v-model="searchFormInline.employeeNumber"
              :placeholder="$t('user.accountTip')"
              clearable
              prefix-icon="Search"
              class="ei_1"
              style="width: 190px"
            />
          </el-form-item>
          <el-form-item :label="$t('user.username') + ' :'">
            <el-input
              v-model="searchFormInline.userName"
              :placeholder="$t('user.usernameTip')"
              clearable
              prefix-icon="Search"
              class="ei_1"
              style="width: 190px"
            />
          </el-form-item>
          <el-form-item :label="$t('user.status') + ' :'">
            <el-select
              v-model="searchFormInline.enable"
              :placeholder="$t('user.statusTip')"
              clearable
              style="width: 170px"
            >
              <el-option :label="$t('user.enable')" value="true" />
              <el-option :label="$t('user.disable')" value="false" />
            </el-select>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" :icon="Search" @click="handleSearch">{{ $t("common.search") }}</el-button>
            <el-button :icon="Refresh" @click="resetSearch">{{ $t("common.reset") }}</el-button>
          </el-form-item>
        </el-form>
      </div>
    </el-card>
    <el-card shadow="never">
      <div class="tableTop">
        <div class="buttonTop">
          <el-button type="primary" @click="handleAdd">+{{ $t("common.add") }}</el-button>
          <el-button type="danger" @click="handleDelList" style="padding: 5px">{{ $t("common.deleteMany") }}</el-button>
        </div>
      </div>
      <!-- 表格数据 -->
      <div class="table" ref="tableContanierRef">
        <el-table
          :data="userTableData"
          :fit="true"
          @selection-change="handleSelectionChange"
          :height="tableHeight"
          border
        >
          <el-table-column type="selection" width="40" />
          <el-table-column type="index" :label="$t('user.id')" align="center" width="100" />
          <el-table-column prop="employeeNumber" :label="$t('user.account')" align="center" width="120" />
          <el-table-column
            v-for="item in userTableLabel"
            align="center"
            :key="item.prop"
            :prop="item.prop"
            :label="item.label"
            show-overflow-tooltip
          />
          <el-table-column :label="$t('systemUser.supplierAccountCol')" align="center" width="120">
            <template #default="{ row }">
              <span>{{ row.isSupplierAccount === 1 ? $t('systemUser.supplierTagYes') : $t('systemUser.supplierTagNo') }}</span>
            </template>
          </el-table-column>
          <el-table-column :label="$t('user.status')" align="center" width="90" prop="isEnable">
            <template #default="{ row }">
              <el-switch
                v-model="row.enable"
                inline-prompt
                inactive-color="#f56c6c"
                :active-text="$t('user.enable')"
                :inactive-text="$t('user.disable')"
                @change="changeUserStatus(row)"
              />
            </template>
          </el-table-column>
          <el-table-column fixed="right" align="center" :label="$t('user.operate')" width="250">
            <!--Vue组件中定义一个插槽，row为当前行的数据，从父组件中拿到的，可以打印{{row}}查看 -->
            <!--如果#default="scope" scope为作用域对象，你可以拿到这个vue模版中的所有的对象和方法 -->
            <template #default="{ row }">
              <!-- {{ scope.row }} -->
              <el-button link type="primary" icon="Edit" @click="handleEdit(row)">{{ $t("common.edit") }}</el-button>
              <el-button link type="danger" icon="Delete" @click="handleUserRemove(row)">{{
                $t("common.delete")
              }}</el-button>
              <el-button link type="primary" icon="Refresh" @click="resetPassword(row)">{{
                $t("user.resetPwd")
              }}</el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>
      <div class="pager-foot">
        <!-- 分页 -->
        <el-pagination
          background
          :page-sizes="[30, 50, 100, 200]"
          layout="total, sizes, prev, pager, next, jumper"
          :total="userConfig.total"
          class="pager mt-5"
          :default-page-size="defaultPageSize"
          :current-page="userConfig.page"
          @size-change="handleSizeChange"
          @current-change="handleUserCurrentChange"
          v-if="userConfig.total != 0"
        />
      </div>
    </el-card>
  </div>
  <!--新增和编辑用户 dialogVisible：隐藏显示对话框视图 -->
  <el-dialog
    v-model="dialogVisible"
    :title="getAction.action === 'add' ? $t('systemUser.addUser') : $t('systemUser.editUser')"
    :before-close="handleClose"
    class="dialog-form"
    draggable
    style="width: 40%"
  >
    <!-- form的prop和v-model双向绑定的要一致，否则会回显数据-->
    <el-form :model="formUserd" ref="userDialogForm" label-width="70px" label-position="right" :rules="rules">
      <el-row>
        <el-col :lg="12">
          <el-form-item :label="$t('systemUser.accountLabel')" prop="employeeNumber">
            <!-- spellcheck关闭语法检查 -->
            <el-input v-model="formUserd.employeeNumber" spellcheck="false" :placeholder="$t('systemUser.enterAccount')" clearable />
          </el-form-item>
        </el-col>
        <el-col :lg="12">
          <el-form-item v-if="getAction.action === 'add'" :label="$t('systemUser.passwordLabel')" prop="password">
            <el-input v-model="formUserd.password" :placeholder="$t('systemUser.enterPassword')" clearable show-password />
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :lg="12">
          <el-form-item :label="$t('systemUser.nicknameLabel')" prop="userName">
            <!-- spellcheck关闭语法检查 -->
            <el-input v-model="formUserd.userName" spellcheck="false" :placeholder="$t('systemUser.enterNickname')" clearable />
          </el-form-item>
        </el-col>
        <el-col :lg="12">
          <el-form-item :label="$t('systemUser.roleLabel')" prop="roleIds">
            <el-select v-model="formUserd.roleIds" style="width: 100%" :placeholder="$t('systemUser.selectRole')">
              <el-option v-for="item in roleList" :label="item.roleName" :value="item.id" :key="item.id" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :lg="12">
          <el-form-item :label="$t('systemUser.supplierAccountLabel')" prop="isSupplierAccount">
            <el-switch
              v-model="formUserd.isSupplierAccount"
              :active-value="1"
              :inactive-value="0"
              @change="(v: string | number | boolean) => { if (v !== 1) formUserd.supplierId = undefined }"
            />
          </el-form-item>
        </el-col>
        <el-col v-if="formUserd.isSupplierAccount === 1" :lg="12">
          <el-form-item :label="$t('systemUser.bindSupplierLabel')" prop="supplierId">
            <el-select
              v-model="formUserd.supplierId"
              filterable
              clearable
              style="width: 100%"
              :placeholder="$t('systemUser.selectSupplier')"
            >
              <el-option v-for="s in supplierOptions" :key="s.id" :label="s.name" :value="s.id" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row>
        <el-col :lg="12">
          <el-form-item :label="$t('systemUser.phoneLabel')" prop="phoneNumber">
            <!-- spellcheck关闭语法检查 -->
            <el-input v-model="formUserd.phoneNumber" spellcheck="false" :placeholder="$t('systemUser.enterPhone')" clearable />
          </el-form-item>
        </el-col>
        <el-col :lg="12">
          <el-form-item :label="$t('systemUser.emailLabel')" prop="email">
            <el-input v-model="formUserd.email" spellcheck="false" :placeholder="$t('systemUser.enterEmail')" clearable />
          </el-form-item>
        </el-col>
        <el-col :lg="12">
          <el-form-item :label="$t('systemUser.statusLabel')" prop="enable">
            <el-select v-model="formUserd.enable" :placeholder="$t('systemUser.selectStatus')" clearable>
              <el-option :label="$t('systemUser.normal')" value="1" />
              <el-option :label="$t('systemUser.disabled')" value="0" />
            </el-select>
          </el-form-item>
        </el-col>
      </el-row>
      <el-row class="dialog-foot">
        <el-form-item>
          <el-button @click="handleCancel">{{ $t('common.cancel') }}</el-button>
          <el-button type="primary" @click="onSubmit">{{ $t('common.confirm') }}</el-button>
        </el-form-item>
      </el-row>
    </el-form>
  </el-dialog>
</template>

<script setup lang="ts" name="user">
import { Md5 } from "ts-md5"
import { ref, reactive, onMounted, nextTick, onActivated, onBeforeUnmount, computed } from "vue"
import { Search, Refresh } from "@element-plus/icons-vue"
import { ElMessage, ElMessageBox, FormInstance } from "element-plus"
import {
  getUserDataApi,
  addUserApi,
  updateUserDataApi,
  deleteUserApi,
  batchDelUserApi,
  changeStatusOrPasswordApi
} from "@/api/system/sys-user"
import { getSupplierListAdminApi } from "@/api/progress/system"
import { getRoleListApi, getRoleByIdApi } from "@/api/system/sys-role"
import { getPageSize, setPageSize } from "@/utils/cache/local-storage"

import i18n from "@/locales"
// #region 定义属性值
/**表格容器引用 */
const tableContanierRef = ref()
/**表格自适应高度 */
const tableHeight = ref()
/**默认页面的条数 */
const defaultPageSize = ref(10)
interface User {
  total: number
  page: number
  size: number
  userName: string
  employeeNumber: string
  enable?: number
}
/**用户表格数据 */
const userTableData = ref()
/**对话框触发 */
const dialogVisible = ref<boolean>(false)
/**对话框title展示-复用 */
const getAction = reactive({
  action: ""
})
/**用户表格label */
const userTableLabel = computed(() => [
  {
    prop: "userName",
    label: i18n.global.t("user.username")
  },
  {
    prop: "phoneNumber",
    label: i18n.global.t("user.telephone")
  },
  {
    prop: "email",
    label: i18n.global.t("user.email")
  },
  {
    prop: "createTime",
    label: i18n.global.t("user.createTime")
  },
  {
    prop: "createBy",
    label: i18n.global.t("user.creator")
  },
  {
    prop: "updateBy",
    label: i18n.global.t("user.editor")
  }
])
// const userTableLabel: any = reactive([
//   {
//     prop: "userName",
//     label: "用户名"
//   },
//   {
//     prop: "phoneNumber",
//     label: "手机号"
//   },
//   {
//     prop: "createTime",
//     label: "创建时间"
//   },
//   {
//     prop: "createBy",
//     label: "创建人"
//   },
//   {
//     prop: "updateBy",
//     label: "修改人"
//   }
// ])
/**用户分页查询配置 */
const userConfig = reactive<User>({
  total: 0,
  page: 1,
  size: 10,
  userName: "",
  employeeNumber: ""
})
/**绑定搜索关键字 */
const searchFormInline = reactive({
  employeeNumber: "",
  userName: "",
  enable: ""
})
// #endregion

// #region 其他操作-生命周期、公共函数
/**封装获取pagesize */
const getCurrentPageSize = () => {
  return getPageSize("user-PageSize")
}
/**限制生命周期多次加载同一请求 */
const isMounted = ref(false)
onMounted(() => {
  isMounted.value = true
  const currentPageSize = getCurrentPageSize()
  if (currentPageSize) {
    defaultPageSize.value = currentPageSize
    userConfig.size = defaultPageSize.value
  }
  calculateTableHeight()
  window.addEventListener("resize", calculateTableHeight)
  getUserData(userConfig)
  loadSupplierOptions()
})
onActivated(() => {
  if (!isMounted.value) {
    calculateTableHeight()
    getUserData(userConfig)
  }
  isMounted.value = false
})

/**监听页面高度，计算设置表格高度*/
const calculateTableHeight = () => {
  nextTick(() => {
    if (tableContanierRef.value) {
      const windowHeight = window.innerHeight
      const tableTopHeight = tableContanierRef.value.offsetTop
      const height = windowHeight - tableTopHeight - 77
      tableHeight.value = height + "px"
    }
  })
}
/**组件销毁 */
onBeforeUnmount(() => {
  window.removeEventListener("resize", calculateTableHeight) // 组件销毁前移除事件监听
})
// #endregion

// #region 表格操作
/**可分配的角色数据信息 */
const roleList = ref()
/** 供应商下拉（管理员接口） */
const supplierOptions = ref<{ id: number; supplierNumber: string; name: string }[]>([])
const loadSupplierOptions = async () => {
  try {
    const res: any = await getSupplierListAdminApi()
    supplierOptions.value = res?.data ?? []
  } catch {
    supplierOptions.value = []
  }
}
/**保存批量删除的id */
let ids = reactive([]) as any
/**分页跳转 */
const handleUserCurrentChange = (val: number) => {
  userConfig.page = val
  getUserData(userConfig)
}
/** 改变每页条数数 */
const handleSizeChange = (val: number) => {
  userConfig.size = val
  getUserData(userConfig)
  setPageSize("user-PageSize", val)
}

/** 关键字搜索*/
const handleSearch = () => {
  userConfig.userName = searchFormInline.userName
  userConfig.employeeNumber = searchFormInline.employeeNumber
  userConfig.enable = searchFormInline.enable === "true" ? 1 : searchFormInline.enable === "false" ? 0 : undefined
  // userConfig.page = 1
  getUserData(userConfig)
}

/**重置查询 */
const resetUserConfig = () => {
  defaultPageSize.value = getCurrentPageSize() || 10
  Object.assign(userConfig, {
    page: 1,
    userName: "",
    employeeNumber: ""
  } as User)
}
/**重置搜索表单内容 */
const resetSearchFormInline = () => {
  Object.assign(searchFormInline, {
    userName: "",
    employeeNumber: "",
    enable: ""
  })
}
/** 重置搜索*/
const resetSearch = () => {
  resetSearchFormInline()
  resetUserConfig()
  getUserData(userConfig)
}

/**获取用户列表数据 */
const getUserData = async (userConfig: User) => {
  const req = {
    page: userConfig.page,
    size: userConfig.size,
    userName: userConfig.userName || undefined,
    employeeNumber: userConfig.employeeNumber || undefined,
    enable: userConfig.enable
  }
  const res: any = await getUserDataApi(req)
  console.log("%c [ userres ]-267", "font-size:13px; background:pink; color:#bf2c9f;", res)
  if (res) {
    // const rulesMap = {
    //   1: "管理员",
    //   2: "普通用户"
    // }
    userConfig.total = res.data.total
    if (res.data.dataList !== null && res.data.dataList.length !== 0) {
      userTableData.value = res.data.dataList.map((item: any) => {
        item.enable = item.enable === 1 ? true : false
        if (item.email == null && item.Email != null) item.email = item.Email
        const isa = item.isSupplierAccount ?? item.IsSupplierAccount
        item.isSupplierAccount = isa === 1 || isa === true ? 1 : 0
        return item
      })
    } else {
      userTableData.value = []
    }
  }
}
/** 重置密码*/
const resetPassword = (row: any) => {
    ElMessageBox.confirm(i18n.global.t('systemUser.resetPasswordConfirm'), i18n.global.t('systemUser.systemPrompt'), {
      type: "warning",
      cancelButtonText: i18n.global.t('common.cancel'),
      confirmButtonText: i18n.global.t('common.confirm')
    })
    .then(async () => {
      const password = await Md5.hashStr("123456").toString()
      const id = row.id
      //enable在这里不等于1或者0，后端进行校验
      const enable = 101
      const res = await changeStatusOrPasswordApi({
        id: id,
        enable: enable,
        password: password
      })
      if (res) {
        ElMessage({
          type: "success",
          message: `用户 "${row.userName}" 密码已重置"`
        })
        getUserData(userConfig)
      }
    })
    .catch(() => {})
}
/** 是否禁用或启用用户 */
const changeUserStatus = (row: any) => {
const status = row.enable === true ? i18n.global.t('systemUser.enabled') : i18n.global.t('systemUser.disabled')
    ElMessageBox.confirm(i18n.global.t('systemUser.changeStatusConfirm', { status: status, username: row.userName }), i18n.global.t('systemUser.systemPrompt'), {
      confirmButtonText: i18n.global.t('common.confirm'),
      cancelButtonText: i18n.global.t('common.cancel'),
      type: "warning"
    })
    .then(async () => {
      const id = row.id
      const enable = row.enable === true ? 1 : 0
      const res = await changeStatusOrPasswordApi({
        id: id,
        enable: enable,
        password: null
      })
      if (res) {
        ElMessage({
          type: "success",
          message: `用户 "${row.userName}" 状态已修改为 "${status}"`
        })
        getUserData(userConfig)
      }
    })
    .catch(() => {
      row.enable = row.enable === true ? false : true
    })
}

/** 新增用户*/
const handleAdd = async () => {
  resetFormUserd()
  getAction.action = "add"
  const res: any = await getRoleListApi()
  if (res) {
    roleList.value = res.data
  }
  dialogVisible.value = true
}

/** 编辑用户*/
const handleEdit = async (row: any) => {
  const userId = row.id
  // 获取角色列表
  const res: any = await getRoleByIdApi(userId)
  if (res) {
    getAction.action = "edit"
    roleList.value = res.data.roles
    dialogVisible.value = true
    //事件循环机制，异步执行，避免在新增按钮触发是还可以看到编辑的值显示
    nextTick(() => {
      // 浅拷贝
      Object.assign(formUserd, row)
      formUserd.email = (row as any).email ?? (row as any).Email ?? ""
      formUserd.enable = formUserd.enable ? "1" : "0"
      if (res.data.roleIds > 0) {
        formUserd.roleIds = res.data.roleIds
      } else {
        ElMessage.error("用戶角色被删除请重新选择")
      }
      const isa = (row as any).isSupplierAccount ?? (row as any).IsSupplierAccount
      formUserd.isSupplierAccount = isa === 1 || isa === true ? 1 : 0
      formUserd.supplierId = (row as any).supplierId ?? (row as any).SupplierId ?? undefined
      //
      // res.data.roleIds.map((item: any) => {
      //   formUserd.roleIds = item
      // })
      console.log("%c [ formUserd ]-421", "font-size:13px; background:pink; color:#bf2c9f;", formUserd)
    })
  }
}
/** 删除用户*/
const handleUserRemove = (row: any) => {
    ElMessageBox.confirm(i18n.global.t('systemUser.deleteConfirm'), i18n.global.t('systemUser.prompt'), {
      confirmButtonText: i18n.global.t('common.confirm'),
      cancelButtonText: i18n.global.t('common.cancel'),
      type: "warning"
    })
    .then(async () => {
      const res: any = await deleteUserApi({ id: row.id })
      console.log(res)
      if (res) {
        ElMessage({
          showClose: true,
          message: res.message,
          type: "success"
        })
        getUserData(userConfig)
      }
    })
    .catch(() => {
      // catch error
    })
}

/** 批量选中*/
const handleSelectionChange = (val: any) => {
  console.log("%c [ val ]-447", "font-size:13px; background:pink; color:#bf2c9f;", val)
  ids = []
  val.forEach((item: any) => {
    ids.push(item.id)
  })
}
/** 批量删除*/
const handleDelList = () => {
    ElMessageBox.confirm(i18n.global.t('systemUser.batchDeleteConfirm'), i18n.global.t('systemUser.prompt'), {
      confirmButtonText: i18n.global.t('common.confirm'),
      cancelButtonText: i18n.global.t('common.cancel'),
      type: "warning"
    })
    .then(async () => {
      if (ids.length !== 0) {
        console.log("%c [ ids ]-466", "font-size:13px; background:pink; color:#bf2c9f;", ids)
        const res: any = await batchDelUserApi(ids)
        if (res) {
          ElMessage({
            showClose: true,
            message: res.message,
            type: "success"
          })
          getUserData(userConfig)
          ids = []
        }
      } else {
        ElMessage({
          showClose: true,
          message: i18n.global.t('systemUser.pleaseSelectUser'),
          type: "warning"
        })
      }
    })
    .catch((err) => {
      console.log(err)
    })
}
// #endregion

// #region 对话框-表单操作
/**对话框表单ref */
const userDialogForm = ref()
/**添加用户对话框表单数据接收绑定*/
const formUserd = reactive({
  id: "",
  employeeNumber: "",
  userName: "",
  phoneNumber: "",
  email: "",
  enable: "",
  // 默认密码
  password: "123456",
  roleIds: [] as any,
  isSupplierAccount: 0 as 0 | 1,
  supplierId: undefined as number | undefined
})
/**重置表单数据，防止数据回显 */
const resetFormUserd = () => {
  Object.assign(formUserd, {
    id: "",
    employeeNumber: "",
    userName: "",
    phoneNumber: "",
    email: "",
    enable: "",
    // 默认密码
    password: "123456",
    roleIds: "",
    isSupplierAccount: 0,
    supplierId: undefined
  })
}
/**表单校验 */
const rules = reactive({
  employeeNumber: [{ required: true, message: i18n.global.t('systemUser.accountRequired'), trigger: "blur" }],
  userName: [{ required: true, message: i18n.global.t('systemUser.usernameRequired'), trigger: "blur" }],
  password: [{ required: true, message: i18n.global.t('systemUser.passwordRequired'), trigger: "blur" }],
  roleIds: [{ required: true, message: i18n.global.t('systemUser.roleRequired'), trigger: "blur" }],
  enable: [{ required: true, message: i18n.global.t('systemUser.statusRequired'), trigger: "blur" }],
  phoneNumber: [{ pattern: /^(?:(?:\+|00)86)?1\d{10}$/, message: i18n.global.t('systemUser.phoneInvalid'), trigger: "blur" }],
  email: [
    {
      validator: (_rule: unknown, value: string, callback: (e?: Error) => void) => {
        if (value === undefined || value === null || String(value).trim() === "") {
          callback()
          return
        }
        const ok = /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(String(value).trim())
        callback(ok ? undefined : new Error(i18n.global.t("systemUser.emailInvalid")))
      },
      trigger: "blur"
    }
  ],
  supplierId: [
    {
      validator: (_rule: unknown, value: unknown, callback: (e?: Error) => void) => {
        if (formUserd.isSupplierAccount !== 1) {
          callback()
          return
        }
        if (value === null || value === undefined || value === "") {
          callback(new Error(i18n.global.t("systemUser.supplierRequired")))
          return
        }
        callback()
      },
      trigger: "change"
    }
  ]
})
/** 取消对话框*/
const handleCancel = () => {
  dialogVisible.value = false
  formUserd.password = "123456"
  userDialogForm.value.resetFields()
}
/** 关闭对话框*/
const handleClose = (done: () => void) => {
    ElMessageBox.confirm(i18n.global.t('systemUser.closeConfirm'), {
      confirmButtonText: i18n.global.t('common.confirm'),
      cancelButtonText: i18n.global.t('common.cancel')
    })
    .then(() => {
      formUserd.password = "123456"
      userDialogForm.value.resetFields()
      done()
    })
    .catch(() => {
      // catch error
    })
}
/** 提交保存用户信息*/
const onSubmit = () => {
  // 表单内容验证，表单的ref属性拿到表单，使用el-api校验
  userDialogForm.value.validate(async (valid: FormInstance | undefined) => {
    if (valid) {
      const currentUser = localStorage.getItem("employeeNumber")
      const userPassword = Md5.hashStr(formUserd.password).toString()
      if (getAction.action === "add") {
        console.log("%c [ formUserd ]-513", "font-size:13px; background:pink; color:#bf2c9f;", formUserd)
        // 新增用户密码加密
        const formUserdAdd = reactive({
          employeeNumber: formUserd.employeeNumber,
          userName: formUserd.userName,
          password: userPassword,
          roleIds: formUserd.roleIds,
          phoneNumber: formUserd.phoneNumber,
          email: formUserd.email?.trim() || null,
          enable: formUserd.enable,
          createBy: currentUser,
          isSupplierAccount: formUserd.isSupplierAccount,
          supplierId: formUserd.isSupplierAccount === 1 ? (formUserd.supplierId ?? null) : null
        })
        // await只会返回给他最近的async回调函数！！！否则报错
        const res: any = await addUserApi(formUserdAdd)
        // 如果成功，则清空form表单，并关闭弹框
        if (res) {
          // 拿到form表单的ref，使用form自带的清空表单api，ep提供
          //需要注意的是：使用validate、resetFields，需要在表单内容上加prop绑定表单相应属性才会生效
          userDialogForm.value.resetFields()
          // 关闭弹框
          dialogVisible.value = false
          ElMessage({
            showClose: true,
            message: res.message,
            type: "success"
          })
          // 重新获取数据
          getUserData(userConfig)
        }
        // 编辑用户信息
      } else if (getAction.action === "edit") {
        const formUserdEdit = reactive({
          id: formUserd.id,
          employeeNumber: formUserd.employeeNumber,
          userName: formUserd.userName,
          password: userPassword,
          roleIds: formUserd.roleIds,
          phoneNumber: formUserd.phoneNumber,
          email: formUserd.email?.trim() || null,
          enable: formUserd.enable,
          updateBy: currentUser,
          isSupplierAccount: formUserd.isSupplierAccount,
          supplierId: formUserd.isSupplierAccount === 1 ? (formUserd.supplierId ?? null) : null
        })
        const res: any = await updateUserDataApi(formUserdEdit)
        if (res) {
          userDialogForm.value.resetFields()
          dialogVisible.value = false
          ElMessage({
            showClose: true,
            message: res.message,
            type: "success"
          })
          // 重新获取数据
          getUserData(userConfig)
        }
      }
    } else {
      ElMessage.error(i18n.global.t('systemUser.pleaseFillComplete'))
    }
  })
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
  margin-bottom: 20px;
}
.table {
  display: flex;
}
.pager-foot {
  display: flex;
  justify-content: flex-end;
}
.dialog-foot {
  display: flex;
  justify-content: end;
}
</style>
