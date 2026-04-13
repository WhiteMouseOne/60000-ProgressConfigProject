<script lang="ts" setup>
import { onMounted, ref } from "vue"
import { getDashboardApi, type HomeDashboard } from "@/api/progress/home"
import { ElMessage } from "element-plus"

const loading = ref(false)
const dashboard = ref<HomeDashboard | null>(null)

const load = async () => {
  loading.value = true
  try {
    const res = (await getDashboardApi()) as { data: HomeDashboard }
    dashboard.value = res.data
  } catch {
    ElMessage.error("加载首页数据失败")
  } finally {
    loading.value = false
  }
}

onMounted(() => load())
</script>

<template>
  <div class="app-container" v-loading="loading">
    <el-row :gutter="16">
      <el-col :span="24">
        <el-card shadow="never" header="预警列表（未发货且进入预警窗口）">
          <el-table :data="dashboard?.alerts ?? []" stripe border size="small" empty-text="暂无预警">
            <el-table-column prop="poNumber" label="PO" width="140" />
            <el-table-column prop="lineNo" label="行号" width="72" />
            <el-table-column prop="partName" label="加工件" min-width="160" />
            <el-table-column prop="supplierName" label="供应商" width="140" />
            <el-table-column prop="requiredDeliveryDate" label="要求交期" width="120" />
            <el-table-column prop="leadDays" label="提前天数" width="88" />
          </el-table>
        </el-card>
      </el-col>
    </el-row>
    <el-row :gutter="16" style="margin-top: 16px">
      <el-col :span="24">
        <el-card shadow="never" header="供应商完成率与逾期（按数据范围）">
          <el-table :data="dashboard?.supplierStats ?? []" stripe border size="small" empty-text="暂无数据">
            <el-table-column prop="supplierName" label="供应商" min-width="160" />
            <el-table-column prop="totalLines" label="总行数" width="88" />
            <el-table-column prop="shippedLines" label="已发货" width="88" />
            <el-table-column prop="overdueNotShipped" label="逾期未发货" width="110" />
            <el-table-column prop="completionRate" label="完成率%" width="100" />
          </el-table>
        </el-card>
      </el-col>
    </el-row>
  </div>
</template>

<style scoped>
.app-container {
  padding: 16px;
}
</style>
