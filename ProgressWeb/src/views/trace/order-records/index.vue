<script lang="ts" setup>
import { computed, nextTick, onBeforeUnmount, onMounted, reactive, ref } from "vue"
import { Search, Refresh } from "@element-plus/icons-vue"
import {
  queryOrderLinesApi,
  createOrderLineApi,
  updateOrderLineApi,
  deleteOrderLineApi,
  supplierUpdateLineApi,
  createRepairApi,
  type OrderLineDto
} from "@/api/progress/order"
import { getSuppliersMetaApi } from "@/api/progress/meta"
import { useUserStore } from "@/store/modules/user"
import { ElMessage, ElMessageBox, type FormInstance, type FormRules } from "element-plus"

const userStore = useUserStore()
const roles = computed(() => userStore.roles)

const loading = ref(false)
const tableData = ref<OrderLineDto[]>([])
const total = ref(0)
const query = reactive({
  poNumber: "",
  projectCode: "",
  drawingNumber: "",
  partName: "",
  supplierId: undefined as number | undefined,
  page: 1,
  pageSize: 20
})

const suppliers = ref<{ id: number; supplierNumber: string; name: string }[]>([])

const loadSuppliers = async () => {
  try {
    const res = await getSuppliersMetaApi()
    suppliers.value = (res as any).data ?? []
  } catch {
    /* ignore */
  }
}

const fetchList = async () => {
  loading.value = true
  try {
    const res = await queryOrderLinesApi({
      poNumber: query.poNumber || undefined,
      projectCode: query.projectCode || undefined,
      drawingNumber: query.drawingNumber || undefined,
      partName: query.partName || undefined,
      supplierId: query.supplierId,
      page: query.page,
      pageSize: query.pageSize
    })
    const payload = (res as any).data
    tableData.value = payload?.items ?? []
    total.value = payload?.total ?? 0
  } finally {
    loading.value = false
  }
}

const tableContanierRef = ref<HTMLElement>()
const tableHeight = ref("480px")

const calculateTableHeight = () => {
  nextTick(() => {
    if (tableContanierRef.value) {
      const windowHeight = window.innerHeight
      const tableTopHeight = tableContanierRef.value.offsetTop
      const height = windowHeight - tableTopHeight - 77
      tableHeight.value = Math.max(280, height) + "px"
    }
  })
}

onMounted(async () => {
  await loadSuppliers()
  calculateTableHeight()
  window.addEventListener("resize", calculateTableHeight)
  await fetchList()
})

onBeforeUnmount(() => {
  window.removeEventListener("resize", calculateTableHeight)
})

const handleSearch = () => {
  query.page = 1
  fetchList()
}

const resetSearch = () => {
  query.poNumber = ""
  query.projectCode = ""
  query.drawingNumber = ""
  query.partName = ""
  query.supplierId = undefined
  query.page = 1
  fetchList()
}

const editVisible = ref(false)
const editFormRef = ref<FormInstance>()
const editForm = reactive({
  id: undefined as number | undefined,
  poNumber: "",
  projectCode: "",
  drawingNumber: "",
  partName: "",
  material: "",
  supplierId: undefined as number | undefined,
  quantity: 1,
  unit: "件",
  receivedQuantity: undefined as number | undefined,
  requiredDeliveryDate: "" as string | undefined,
  latestCraftCode: "",
  shippingStatus: 0,
  supplierNotes: "",
  actualDeliveryDate: "" as string | undefined
})

const editRules: FormRules = {
  poNumber: [{ required: true, message: "必填", trigger: "blur" }],
  projectCode: [{ required: true, message: "必填", trigger: "blur" }],
  drawingNumber: [{ required: true, message: "必填", trigger: "blur" }],
  partName: [{ required: true, message: "必填", trigger: "blur" }],
  supplierId: [{ required: true, message: "必选", trigger: "change" }]
}

const openCreate = () => {
  Object.assign(editForm, {
    id: undefined,
    poNumber: "",
    projectCode: "",
    drawingNumber: "",
    partName: "",
    material: "",
    supplierId: undefined,
    quantity: 1,
    unit: "件",
    receivedQuantity: undefined,
    requiredDeliveryDate: undefined,
    latestCraftCode: "",
    shippingStatus: 0,
    supplierNotes: "",
    actualDeliveryDate: undefined
  })
  editVisible.value = true
}

const openEdit = (row: OrderLineDto) => {
  Object.assign(editForm, {
    id: row.id,
    poNumber: row.poNumber,
    projectCode: row.projectCode,
    drawingNumber: row.drawingNumber,
    partName: row.partName,
    material: row.material ?? "",
    supplierId: row.supplierId,
    quantity: row.quantity,
    unit: row.unit,
    receivedQuantity: row.receivedQuantity,
    requiredDeliveryDate: row.requiredDeliveryDate?.slice(0, 10),
    latestCraftCode: row.latestCraftCode,
    shippingStatus: row.shippingStatus,
    supplierNotes: row.supplierNotes,
    actualDeliveryDate: row.actualDeliveryDate?.slice(0, 10)
  })
  editVisible.value = true
}

const saveEdit = async () => {
  await editFormRef.value?.validate()
  const body: Record<string, unknown> = {
    poNumber: editForm.poNumber,
    projectCode: editForm.projectCode,
    drawingNumber: editForm.drawingNumber,
    partName: editForm.partName,
    material: editForm.material || null,
    supplierId: editForm.supplierId,
    quantity: editForm.quantity,
    unit: editForm.unit,
    receivedQuantity: editForm.receivedQuantity ?? null,
    requiredDeliveryDate: editForm.requiredDeliveryDate || null,
    latestCraftCode: editForm.latestCraftCode || null,
    shippingStatus: editForm.shippingStatus,
    supplierNotes: editForm.supplierNotes || null,
    actualDeliveryDate: editForm.actualDeliveryDate || null
  }
  if (editForm.id) body.id = editForm.id
  if (editForm.id) await updateOrderLineApi(body)
  else await createOrderLineApi(body)
  ElMessage.success("已保存")
  editVisible.value = false
  await fetchList()
}

const repairStatusLabel = (s: number) => {
  const m: Record<number, string> = { 0: "-", 1: "待供应商", 2: "返修中", 3: "已返修发货" }
  return m[s] ?? String(s)
}

const shippingStatusLabel = (s: number) => {
  const m: Record<number, string> = { 0: "未填写", 1: "已发货", 2: "未发货" }
  return m[s] ?? String(s)
}

const fmtDateCol = (s?: string) => (s ? s.slice(0, 10) : "-")

const supplierDlg = ref(false)
const supplierForm = reactive({
  id: 0,
  repairStatus: 0,
  repairCreatedAt: "" as string | undefined,
  supplierNotes: "",
  latestCraftCode: "",
  shippingStatus: 0,
  actualDeliveryDate: "" as string | undefined,
  vendorEstimatedDeliveryDate: "" as string | undefined,
  shippedQuantity: undefined as number | undefined,
  repairStartedAt: "" as string | undefined,
  repairShippedAt: "" as string | undefined
})

const openSupplier = (row: OrderLineDto) => {
  supplierForm.id = row.id
  supplierForm.repairStatus = row.repairStatus ?? 0
  supplierForm.repairCreatedAt = row.repairCreatedAt
  supplierForm.supplierNotes = row.supplierNotes ?? ""
  supplierForm.latestCraftCode = row.latestCraftCode ?? ""
  supplierForm.shippingStatus = row.shippingStatus
  supplierForm.actualDeliveryDate = row.actualDeliveryDate?.slice(0, 10)
  supplierForm.repairStartedAt = row.repairStartedAt?.slice(0, 10)
  supplierForm.repairShippedAt = row.repairShippedAt?.slice(0, 10)
  supplierForm.vendorEstimatedDeliveryDate = row.vendorEstimatedDeliveryDate?.slice(0, 10)
  supplierForm.shippedQuantity = row.shippedQuantity
  supplierDlg.value = true
}

const saveSupplier = async () => {
  await supplierUpdateLineApi(supplierForm.id, {
    supplierNotes: supplierForm.supplierNotes,
    latestCraftCode: supplierForm.latestCraftCode,
    shippingStatus: supplierForm.shippingStatus,
    actualDeliveryDate: supplierForm.actualDeliveryDate || null,
    vendorEstimatedDeliveryDate: supplierForm.vendorEstimatedDeliveryDate || null,
    shippedQuantity: supplierForm.shippedQuantity ?? null,
    repairStartedAt: supplierForm.repairStartedAt || null,
    repairShippedAt: supplierForm.repairShippedAt || null
  })
  ElMessage.success("已更新")
  supplierDlg.value = false
  await fetchList()
}

const repairDlg = ref(false)
const repairLineId = ref(0)
const repairText = ref("")

const openRepair = (row: OrderLineDto) => {
  repairLineId.value = row.id
  repairText.value = ""
  repairDlg.value = true
}

const submitRepair = async () => {
  if (!repairText.value.trim()) {
    ElMessage.warning("请填写返修说明")
    return
  }
  await createRepairApi({ orderLineId: repairLineId.value, description: repairText.value })
  ElMessage.success("已提交返修")
  repairDlg.value = false
}

const removeLine = (row: OrderLineDto) => {
  ElMessageBox.confirm(`确定删除订单行 #${row.id}（PO ${row.poNumber} 行${row.lineNo}）？`, "删除确认", {
    type: "warning",
    confirmButtonText: "删除",
    cancelButtonText: "取消"
  })
    .then(async () => {
      await deleteOrderLineApi(row.id)
      ElMessage.success("已删除")
      await fetchList()
    })
    .catch(() => {})
}

const canAdminEdit = computed(() => roles.value.includes("Admin") || roles.value.includes("Supervisor"))
const isSupplier = computed(() => roles.value.includes("Supplier"))

/** 跨页连续序号（中间列，随横向区域滚动） */
const tableIndex = (index: number) => (query.page - 1) * query.pageSize + index + 1
</script>

<template>
  <div class="app-container">
    <el-card class="searchTop" shadow="never">
      <div class="searchForm">
        <el-form :inline="true" :model="query">
          <el-form-item label="PO">
            <el-input v-model="query.poNumber" clearable placeholder="PO" style="width: 140px" />
          </el-form-item>
          <el-form-item label="项目">
            <el-input v-model="query.projectCode" clearable placeholder="项目编号" style="width: 120px" />
          </el-form-item>
          <el-form-item label="图号">
            <el-input v-model="query.drawingNumber" clearable placeholder="图号" style="width: 120px" />
          </el-form-item>
          <el-form-item label="加工件">
            <el-input v-model="query.partName" clearable placeholder="名称" style="width: 120px" />
          </el-form-item>
          <el-form-item v-if="!isSupplier" label="供应商">
            <el-select v-model="query.supplierId" clearable placeholder="全部" style="width: 180px" filterable>
              <el-option v-for="s in suppliers" :key="s.id" :label="`${s.supplierNumber} ${s.name}`" :value="s.id" />
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
          <el-button v-if="canAdminEdit" type="primary" @click="openCreate">+ 新增</el-button>
        </div>
      </div>
      <div ref="tableContanierRef" class="table">
        <el-table
          v-loading="loading"
          :data="tableData"
          :fit="true"
          border
          stripe
          size="small"
          :height="tableHeight"
        >
          <el-table-column type="index" label="序号" width="64" fixed="left" :index="tableIndex" align="center" />
          <el-table-column prop="poNumber" label="订单编号" fixed="left" width="130" show-overflow-tooltip />
          <el-table-column prop="projectCode" label="项目编号" fixed="left" width="110" show-overflow-tooltip />
          <el-table-column prop="partName" label="加工件名称" fixed="left" width="150" show-overflow-tooltip />
          <el-table-column prop="supplierName" label="供应商" fixed="left" width="130" show-overflow-tooltip />
          <el-table-column prop="drawingNumber" label="图号" width="120" show-overflow-tooltip />
          <el-table-column prop="quantity" label="数量" width="72" />
          <el-table-column prop="requiredDeliveryDate" label="交期" width="110" show-overflow-tooltip>
            <template #default="{ row }">{{ fmtDateCol(row.requiredDeliveryDate) }}</template>
          </el-table-column>
          <el-table-column prop="latestCraftCode" label="最新工艺" width="88" />
          <el-table-column prop="shippingStatus" label="发货状态" width="88">
            <template #default="{ row }">
              {{ shippingStatusLabel(row.shippingStatus ?? 0) }}
            </template>
          </el-table-column>
          <el-table-column prop="vendorUpdatedAt" label="加工商更新" width="160" show-overflow-tooltip />
          <el-table-column label="返修状态" width="96">
            <template #default="{ row }">
              {{ repairStatusLabel(row.repairStatus ?? 0) }}
            </template>
          </el-table-column>
          <el-table-column label="返修创建" width="110">
            <template #default="{ row }">{{ fmtDateCol(row.repairCreatedAt) }}</template>
          </el-table-column>
          <el-table-column label="返修开始" width="110">
            <template #default="{ row }">{{ fmtDateCol(row.repairStartedAt) }}</template>
          </el-table-column>
          <el-table-column label="返修发货" width="110">
            <template #default="{ row }">{{ fmtDateCol(row.repairShippedAt) }}</template>
          </el-table-column>
          <el-table-column label="操作" width="280" fixed="right" align="center">
            <template #default="{ row }">
              <el-button v-if="canAdminEdit" link type="primary" @click="openEdit(row)">编辑</el-button>
              <el-button v-if="canAdminEdit" link type="danger" @click="removeLine(row)">删除</el-button>
              <el-button v-if="isSupplier" link type="primary" @click="openSupplier(row)">填报</el-button>
              <el-button v-if="canAdminEdit" link type="warning" @click="openRepair(row)">返修</el-button>
            </template>
          </el-table-column>
        </el-table>
      </div>
      <div class="pager-foot">
        <el-pagination
          v-model:current-page="query.page"
          v-model:page-size="query.pageSize"
          background
          :total="total"
          layout="total, sizes, prev, pager, next, jumper"
          :page-sizes="[10, 20, 50]"
          class="pager mt-5"
          @current-change="fetchList"
          @size-change="fetchList"
        />
      </div>
    </el-card>

    <el-dialog v-model="editVisible" :title="editForm.id ? '编辑订单行' : '新建订单行'" width="560px" destroy-on-close>
      <el-form ref="editFormRef" :model="editForm" :rules="editRules" label-width="100px">
        <el-form-item label="PO" prop="poNumber">
          <el-input v-model="editForm.poNumber" />
        </el-form-item>
        <el-form-item label="项目编号" prop="projectCode">
          <el-input v-model="editForm.projectCode" />
        </el-form-item>
        <el-form-item label="图号" prop="drawingNumber">
          <el-input v-model="editForm.drawingNumber" />
        </el-form-item>
        <el-form-item label="加工件名称" prop="partName">
          <el-input v-model="editForm.partName" />
        </el-form-item>
        <el-form-item label="材质">
          <el-input v-model="editForm.material" />
        </el-form-item>
        <el-form-item label="供应商" prop="supplierId">
          <el-select v-model="editForm.supplierId" filterable style="width: 100%">
            <el-option v-for="s in suppliers" :key="s.id" :label="`${s.supplierNumber} ${s.name}`" :value="s.id" />
          </el-select>
        </el-form-item>
        <el-form-item label="数量">
          <el-input-number v-model="editForm.quantity" :min="0" :step="1" />
        </el-form-item>
        <el-form-item label="收货数量">
          <el-input-number v-model="editForm.receivedQuantity" :min="0" :step="1" />
        </el-form-item>
        <el-form-item label="单位">
          <el-input v-model="editForm.unit" />
        </el-form-item>
        <el-form-item label="要求交期">
          <el-date-picker v-model="editForm.requiredDeliveryDate" type="date" value-format="YYYY-MM-DD" />
        </el-form-item>
        <el-form-item label="最新工艺码">
          <el-input v-model="editForm.latestCraftCode" />
        </el-form-item>
        <el-form-item label="发货状态">
          <el-select v-model="editForm.shippingStatus" style="width: 100%">
            <el-option :value="0" label="未填写" />
            <el-option :value="1" label="已发货" />
            <el-option :value="2" label="未发货" />
          </el-select>
        </el-form-item>
        <el-form-item label="备注">
          <el-input v-model="editForm.supplierNotes" type="textarea" />
        </el-form-item>
        <el-form-item label="实际发货日">
          <el-date-picker v-model="editForm.actualDeliveryDate" type="date" value-format="YYYY-MM-DD" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="editVisible = false">取消</el-button>
        <el-button type="primary" @click="saveEdit">保存</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="supplierDlg" title="供应商填报" width="520px" destroy-on-close>
      <el-form label-width="112px">
        <template v-if="supplierForm.repairStatus > 0">
          <el-form-item label="返修状态">
            <span>{{ repairStatusLabel(supplierForm.repairStatus) }}</span>
          </el-form-item>
          <el-form-item label="返修创建日">
            <span>{{ fmtDateCol(supplierForm.repairCreatedAt) }}</span>
          </el-form-item>
          <el-form-item label="返修开始日">
            <el-date-picker
              v-model="supplierForm.repairStartedAt"
              type="date"
              value-format="YYYY-MM-DD"
              style="width: 100%"
            />
          </el-form-item>
          <el-form-item label="返修发货日">
            <el-date-picker
              v-model="supplierForm.repairShippedAt"
              type="date"
              value-format="YYYY-MM-DD"
              style="width: 100%"
            />
          </el-form-item>
        </template>
        <el-form-item label="备注">
          <el-input v-model="supplierForm.supplierNotes" type="textarea" />
        </el-form-item>
        <el-form-item label="最新工艺码">
          <el-input v-model="supplierForm.latestCraftCode" />
        </el-form-item>
        <el-form-item label="加工商预估交期">
          <el-date-picker
            v-model="supplierForm.vendorEstimatedDeliveryDate"
            type="date"
            value-format="YYYY-MM-DD"
            style="width: 100%"
          />
        </el-form-item>
        <el-form-item label="发货数量">
          <el-input-number v-model="supplierForm.shippedQuantity" :min="0" :step="1" style="width: 100%" />
        </el-form-item>
        <el-form-item label="发货状态">
          <el-select v-model="supplierForm.shippingStatus" style="width: 100%">
            <el-option :value="0" label="未填写" />
            <el-option :value="1" label="已发货" />
            <el-option :value="2" label="未发货" />
          </el-select>
        </el-form-item>
        <el-form-item label="实际发货日">
          <el-date-picker v-model="supplierForm.actualDeliveryDate" type="date" value-format="YYYY-MM-DD" />
        </el-form-item>
      </el-form>
      <template #footer>
        <el-button @click="supplierDlg = false">取消</el-button>
        <el-button type="primary" @click="saveSupplier">提交</el-button>
      </template>
    </el-dialog>

    <el-dialog v-model="repairDlg" title="返修" width="420px" destroy-on-close>
      <el-input v-model="repairText" type="textarea" rows="4" placeholder="返修说明" />
      <template #footer>
        <el-button @click="repairDlg = false">取消</el-button>
        <el-button type="primary" @click="submitRepair">提交</el-button>
      </template>
    </el-dialog>
  </div>
</template>

<style lang="scss" scoped>
.tableTop {
  margin-bottom: 12px;
  display: flex;
  .buttonTop {
    display: flex;
    gap: 8px;
  }
}
/* 与 LineMesWeb 工序记录页表格容器一致，表内横向滚动由 EP 根据列宽与容器宽度计算 */
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
