<template>
  <div class="app-container">
    <el-card class="searchTop" shadow="never">
      <div class="searForm">
        <el-form :inline="true" :model="searchFormInline" class="ef_1">
          <el-form-item label="名称 :">
            <el-input
              v-model="searchFormInline.name"
              placeholder="模糊搜索名称"
              clearable
              prefix-icon="Search"
              class="ei_1"
              style="width: 190px"
            />
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
        </div>
      </div>

      <div ref="tableContanierRef" class="table">
        <el-table v-loading="loading" :data="tableData" :fit="true" border :height="tableHeight">
          <el-table-column type="index" label="序号" align="center" width="64" />
          <el-table-column prop="name" label="名称" align="center" min-width="200" show-overflow-tooltip />
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
          v-model:page-size="listConfig.pageSize"
          @size-change="fetchList"
          @current-change="fetchList"
        />
      </div>
    </el-card>

    <el-dialog v-model="dialogVisible" :title="dialogTitle" width="520px" destroy-on-close @closed="resetForm">
      <el-form ref="formRef" :model="form" :rules="formRules" label-width="88px">
        <el-form-item label="编码" prop="code">
          <el-input-number v-model="form.code" :min="1" :step="1" :controls="true" style="width: 100%" />
        </el-form-item>
        <el-form-item label="名称" prop="name">
          <el-input v-model="form.name" placeholder="请输入配方名称" clearable />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="dialogVisible = false">取消</el-button>
        <el-button type="primary" @click="submitForm">确定</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<script setup lang="ts" name="CraftRecipe">
import { ref, reactive, onMounted, nextTick, onBeforeUnmount } from "vue"
import { Search, Refresh, Edit, Delete } from "@element-plus/icons-vue"
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from "element-plus"
import {
  queryCraftRecipeAdminApi,
  createCraftRecipeAdminApi,
  updateCraftRecipeAdminApi,
  deleteCraftRecipeAdminApi
} from "@/api/progress/system"

const loading = ref(false)
const tableContanierRef = ref<HTMLElement>()
const tableHeight = ref<string>()
const tableData = ref<{ id: number; code: number; name: string }[]>([])

const searchFormInline = reactive({
  name: ""
})

const listConfig = reactive({
  page: 1,
  pageSize: 20,
  total: 0
})

const calculateTableHeight = () => {
  nextTick(() => {
    if (tableContanierRef.value) {
      const h = window.innerHeight - tableContanierRef.value.offsetTop - 77
      tableHeight.value = Math.max(280, h) + "px"
    }
  })
}

const fetchList = async () => {
  loading.value = true
  try {
    const res: any = await queryCraftRecipeAdminApi({
      page: listConfig.page,
      pageSize: listConfig.pageSize,
      name: searchFormInline.name || undefined
    })
    const payload = res?.data ?? res
    listConfig.total = payload?.total ?? 0
    tableData.value = payload?.dataList ?? []
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
  searchFormInline.name = ""
  listConfig.page = 1
  fetchList()
}

const dialogVisible = ref(false)
const isEdit = ref(false)
const dialogTitle = ref("新增配方")
const formRef = ref<FormInstance>()
const form = reactive({
  id: 0,
  code: 1,
  name: ""
})

const formRules: FormRules = {
  code: [{ required: true, message: "请输入编码", trigger: "change" }],
  name: [{ required: true, message: "请输入名称", trigger: "blur" }]
}

const resetForm = () => {
  form.id = 0
  form.code = 1
  form.name = ""
  formRef.value?.resetFields()
}

const handleAdd = () => {
  isEdit.value = false
  dialogTitle.value = "新增配方"
  resetForm()
  dialogVisible.value = true
}

const handleEdit = (row: { id: number; code: number; name: string }) => {
  isEdit.value = true
  dialogTitle.value = "编辑配方"
  form.id = row.id
  form.code = row.code
  form.name = row.name
  dialogVisible.value = true
}

const submitForm = async () => {
  await formRef.value?.validate()
  try {
    if (isEdit.value) {
      const res: any = await updateCraftRecipeAdminApi(form.id, {
        code: form.code,
        name: form.name.trim()
      })
      if ((res as any)?.code === 200 || res) {
        ElMessage.success((res as any)?.message ?? "保存成功")
        dialogVisible.value = false
        await fetchList()
      }
    } else {
      const res: any = await createCraftRecipeAdminApi({
        code: form.code,
        name: form.name.trim()
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

const handleDelete = (row: { id: number; name: string }) => {
  ElMessageBox.confirm(`确定删除配方「${row.name}」？`, "提示", { type: "warning" })
    .then(async () => {
      const res: any = await deleteCraftRecipeAdminApi(row.id)
      if ((res as any)?.code === 200 || res) {
        ElMessage.success((res as any)?.message ?? "已删除")
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
