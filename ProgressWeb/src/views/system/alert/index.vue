<script lang="ts" setup>
import { onMounted, reactive, ref } from "vue"
import { getAlertConfigApi, saveAlertConfigApi } from "@/api/progress/system"
import { ElMessage } from "element-plus"

const loading = ref(false)
const form = reactive({ id: 1, leadDays: 3, enabled: true })

const load = async () => {
  loading.value = true
  try {
    const res = await getAlertConfigApi()
    const d = (res as any).data
    if (d) {
      form.id = d.id
      form.leadDays = d.leadDays
      form.enabled = d.enabled
    }
  } catch {
    ElMessage.error("加载失败")
  } finally {
    loading.value = false
  }
}

onMounted(load)

const save = async () => {
  await saveAlertConfigApi({ id: form.id, leadDays: form.leadDays, enabled: form.enabled })
  ElMessage.success("已保存")
}
</script>

<template>
  <div class="app-container">
    <el-card v-loading="loading" shadow="never" header="交期预警">
      <el-form label-width="120px" style="max-width: 400px">
        <el-form-item label="提前天数">
          <el-input-number v-model="form.leadDays" :min="0" :max="365" />
        </el-form-item>
        <el-form-item label="启用扫描">
          <el-switch v-model="form.enabled" />
        </el-form-item>
        <el-form-item>
          <el-button type="primary" @click="save">保存</el-button>
        </el-form-item>
      </el-form>
      <el-alert type="info" show-icon :closable="false" title="后台每 15 分钟扫描一次；邮件仅成功发送后写入去重日志。" />
    </el-card>
  </div>
</template>

<style scoped>
.app-container {
  padding: 16px;
}
</style>
