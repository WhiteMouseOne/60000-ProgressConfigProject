<script lang="ts" setup>
import { onBeforeMount } from "vue"
import { useRouter } from "vue-router"

const router = useRouter()
onBeforeMount(() => {
  router.replace({ path: "/trace/order-records" })
})
</script>

<template>
  <div class="app-container" />
</template>
