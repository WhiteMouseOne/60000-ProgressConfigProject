<template>
  <div class="app-container">
    <el-card class="searchTop" shadow="never">
      <div class="searForm">
        <el-form :inline="true" :model="searchFormInline" class="ef_1">
          <el-form-item label="供应商编号 :">
            <el-input
              v-model="searchFormInline.supplierNumber"
              placeholder="请输入供应商编号"
              clearable
              prefix-icon="Search"
              class="ei_1"
              style="width: 190px"
            />
          </el-form-item>
          <el-form-item label="供应商名称 :">
            <el-input
              v-model="searchFormInline.name"
              placeholder="请输入供应商名称"
              clearable
              prefix-icon="Search"
              class="ei_1"
              style="width: 190px"
            />
          </el-form-item>
          <el-form-item label="状态 :">
            <el-select v-model="searchFormInline.enable" placeholder="请选择状态" clearable style="width: 170px">
              <el-option label="启用" value="1" />
              <el-option label="停用" value="0" />
            </el-select>
          </el-form-item>
          <el-form-item>
            <el-button type="primary" :icon="Search" @click="handleSearch">搜索</el-button>
            <el-button :icon="Refresh" @click="resetSearch">重置</el-button>
          </el-form-item>
        </el-form>
      </div>
    </el-card>

    <el-card shadow="never">
      <div class="tableTop">
        <div class="buttonTop">
          <el-button type="primary" @click="handleAdd">+ 新增</el-button>
          <el-button type="danger" style="padding: 5px" @click="handleBatchDelete">批量删除</el-button>
        </div>
      </div>

      <div ref="tableContanierRef" class="table">
        <el-table
          v-loading="loading"
          :data="tableData"
          :fit="true"
          border
          :height="tableHeight"
          @selection-change="handleSelectionChange"
        >
          <el-table-column type="selection" width="40" />
          <el-table-column type="index" label="序号" align="center" width="64" />
          <el-table-column prop="supplierNumber" label="供应商编号" align="center" width="140" show-overflow-tooltip />
          <el-table-column prop="name" label="供应商名称" align="center" min-width="160" show-overflow-tooltip />
          <el-table-column label="创建时间" align="center" width="170">
            <template #default="{ row }">{{ fmtDateTime(row.createTime) }}</template>
          </el-table-column>
          <el-table-column prop="createBy" label="创建人" align="center" width="120" show-overflow-tooltip />
          <el-table-column prop="updateBy" label="修改人" align="center" width="120" show-overflow-tooltip />
          <el-table-column label="状态" align="center" width="100">
            <template #default="{ row }">
              <el-switch
                v-model="row.enable"
                inline-prompt
                inactive-color="#f56c6c"
                active-text="启用"
                inactive-text="停用"
                :active-value="true"
                :inactive-value="false"
                @change="() => onEnableChange(row)"
              />
            </template>
          </el-table-column>
          <el-table-column fixed="right" label="操作" align="center" width="180">
            <template #default="{ row }">
              <el-button link type="primary" :icon="Edit" @click="handleEdit(row)">编辑</el-button>
              <el-button link type="danger" :icon="Delete" @click="handleDelete(row)">删除</el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>

      <div class="pager-foot">
        <el-pagination
          v-if="listConfig.total > 0"
          background
          :page-sizes="[10, 20, 50, 100]"
          layout="total, sizes, prev, pager, next, jumper"
          :total="listConfig.total"
          class="pager mt-5"
          v-model:current-page="listConfig.page"
          v-model:page-size="listConfig.size"
          @size-change="fetchList"
          @current-change="fetchList"
        />
      </div>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="520px" destroy-on-close @closed="resetForm">
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="100px">
        <el-form-item label="供应商编号" prop="supplierNumber">
          <el-input v-model="form.supplierNumber" :disabled="isEdit" placeholder="请输入供应商编号" clearable />
        </el-form-item>
        <el-form-item label="供应商名称" prop="name">
          <el-input v-model="form.name" placeholder="请输入供应商名称" clearable />
        </el-form-item>
        <el-form-item label="状态" prop="enable">
          <el-radio-group v-model="form.enable">
            <el-radio :label="1">启用</el-radio>
            <el-radio :label="0">停用</el-radio>
          </el-radio-group>
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitForm">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts" name="SupplierManage">
import { ref, reactive, onMounted, nextTick, onBeforeUnmount } from "vue"
import { Search, Refresh, Edit, Delete } from "@element-plus/icons-vue"
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from "element-plus"
import {
  querySupplierAdminApi,
  createSupplierAdminApi,
  updateSupplierAdminApi,
  deleteSupplierAdminApi,
  batchDeleteSupplierAdminApi
} from "@/api/progress/system"

const loading = ref(false)
const tableContanierRef = ref<HTMLElement>()
const tableHeight = ref<string>()
const tableData = ref<any[]>([])

const searchFormInline = reactive({
  supplierNumber: "",
  name: "",
  enable: "" as "" | "1" | "0"
})

const listConfig = reactive({
  page: 1,
  size: 10,
  total: 0
})

const selectedIds = ref<number[]>([])

const fmtDateTime = (v: unknown) => {
  if (v == null || v === "") return "-"
  const d = new Date(v as string)
  if (Number.isNaN(d.getTime())) return String(v)
  return d.toLocaleString("zh-CN", { hour12: false })
}

const calculateTableHeight = () => {
  nextTick(() => {
    if (tableContanierRef.value) {
      const h = window.innerHeight - tableContanierRef.value.offsetTop - 77
      tableHeight.value = Math.max(280, h) + "px"
    }
  })
}

const mapRows = (list: any[]) =>
  list.map((item) => ({
    ...item,
    enable: item.enable === 1 || item.enable === true
  }))

const fetchList = async () => {
  loading.value = true
  try {
    const enableFilter =
      searchFormInline.enable === "" ? undefined : Number.parseInt(searchFormInline.enable as string, 10)
    const res: any = await querySupplierAdminApi({
      page: listConfig.page,
      size: listConfig.size,
      supplierNumber: searchFormInline.supplierNumber || undefined,
      name: searchFormInline.name || undefined,
      enable: enableFilter
    })
    const payload = res?.data ?? res
    listConfig.total = payload?.total ?? 0
    const list = payload?.dataList ?? []
    tableData.value = mapRows(list)
  } catch {
    tableData.value = []
    listConfig.total = 0
  } finally {
    loading.value = false
  }
}

const handleSearch = () => {
  listConfig.page = 1
  fetchList()
}

const resetSearch = () => {
  searchFormInline.supplierNumber = ""
  searchFormInline.name = ""
  searchFormInline.enable = ""
  listConfig.page = 1
  fetchList()
}

const handleSelectionChange = (rows: { id: number }[]) => {
  selectedIds.value = rows.map((r) => r.id)
}

const dialogVisible = ref(false)
const isEdit = ref(false)
const dialogTitle = ref("新增供应商")
const formRef = ref<FormInstance>()
const form = reactive({
  id: 0,
  supplierNumber: "",
  name: "",
  enable: 1
})

const formRules: FormRules = {
  supplierNumber: [{ required: true, message: "请输入供应商编号", trigger: "blur" }],
  name: [{ required: true, message: "请输入供应商名称", trigger: "blur" }]
}

const resetForm = () => {
  form.id = 0
  form.supplierNumber = ""
  form.name = ""
  form.enable = 1
  formRef.value?.resetFields()
}

const handleAdd = () => {
  isEdit.value = false
  dialogTitle.value = "新增供应商"
  resetForm()
  dialogVisible.value = true
}

const handleEdit = (row: any) => {
  isEdit.value = true
  dialogTitle.value = "编辑供应商"
  form.id = row.id
  form.supplierNumber = row.supplierNumber
  form.name = row.name
  form.enable = row.enable === true ? 1 : 0
  dialogVisible.value = true
}

const submitForm = async () => {
  await formRef.value?.validate()
  try {
    if (isEdit.value) {
      const res: any = await updateSupplierAdminApi(form.id, {
        name: form.name,
        supplierNumber: form.supplierNumber,
        enable: form.enable
      })
      if ((res as any)?.code === 200 || (res as any)?.data?.code === 200 || res) {
        ElMessage.success((res as any)?.message ?? "保存成功")
        dialogVisible.value = false
        await fetchList()
      }
    } else {
      const res: any = await createSupplierAdminApi({
        supplierNumber: form.supplierNumber,
        name: form.name,
        enable: form.enable
      })
      if ((res as any)?.code === 200 || res) {
        ElMessage.success((res as any)?.message ?? "创建成功")
        dialogVisible.value = false
        await fetchList()
      }
    }
  } catch (e: any) {
    ElMessage.error(e?.message ?? "请求失败")
  }
}

const onEnableChange = async (row: any) => {
  const enable = row.enable === true ? 1 : 0
  try {
    const res: any = await updateSupplierAdminApi(row.id, { enable })
    if ((res as any)?.code === 200 || res) {
      ElMessage.success("状态已更新")
      await fetchList()
    }
  } catch {
    row.enable = !row.enable
  }
}

const handleDelete = (row: any) => {
  ElMessageBox.confirm(`确定删除供应商「${row.name}」？`, "提示", { type: "warning" })
    .then(async () => {
      const res: any = await deleteSupplierAdminApi(row.id)
      if ((res as any)?.code === 200 || res) {
        ElMessage.success((res as any)?.message ?? "已删除")
        await fetchList()
      }
    })
    .catch(() => {})
}

const handleBatchDelete = () => {
  if (selectedIds.value.length === 0) {
    ElMessage.warning("请先勾选要删除的行")
    return
  }
  ElMessageBox.confirm(`确定删除选中的 ${selectedIds.value.length} 条供应商？`, "提示", { type: "warning" })
    .then(async () => {
      const res: any = await batchDeleteSupplierAdminApi({ ids: selectedIds.value })
      if ((res as any)?.code === 200 || res) {
        ElMessage.success((res as any)?.message ?? "已删除")
        selectedIds.value = []
        await fetchList()
      }
    })
    .catch(() => {})
}

onMounted(() => {
  calculateTableHeight()
  window.addEventListener("resize", calculateTableHeight)
  fetchList()
})

onBeforeUnmount(() => {
  window.removeEventListener("resize", calculateTableHeight)
})
</script>

<style lang="scss" scoped>
.searchTop {
  margin-bottom: 10px;
  :deep(.el-card__body) {
    padding-bottom: 2px;
  }
}
.tableTop {
  margin-bottom: 12px;
}
.buttonTop {
  display: flex;
  gap: 8px;
}
.table {
  display: flex;
  margin-bottom: 0;
}
.pager-foot {
  display: flex;
  justify-content: flex-end;
  margin-top: 8px;
}
.mt-5 {
  margin-top: 12px;
}
</style>
