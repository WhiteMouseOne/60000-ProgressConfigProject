<script lang="ts" setup>
import { onMounted, ref } from "vue"
import { getCraftListAdminApi } from "@/api/progress/system"

const loading = ref(false)
const rows = ref<any[]>([])

const load = async () => {
  loading.value = true
  try {
    const res = await getCraftListAdminApi()
    rows.value = (res as any).data ?? []
  } finally {
    loading.value = false
  }
}

onMounted(load)
</script>

<template>
  <div class="app-container">
    <el-card shadow="never" header="工艺库">
      <el-table v-loading="loading" :data="rows" border stripe size="small">
        <el-table-column prop="id" label="ID" width="64" />
        <el-table-column prop="code" label="编码" width="120" />
        <el-table-column prop="name" label="名称" min-width="200" />
      </el-table>
    </el-card>
  </div>
</template>

<style scoped>
.app-container {
  padding: 16px;
}
</style>
