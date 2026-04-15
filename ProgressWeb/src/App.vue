<script lang="ts" setup>
import { computed, onMounted } from "vue"
import { useTheme } from "@/hooks/useTheme"
import useLocale from "@/hooks/useLocale"
const { currentLocale, changeLocale } = useLocale()
// 将 Element Plus 的语言设置为中文
import zhCn from "element-plus/es/locale/lang/zh-cn"
import en from "element-plus/es/locale/lang/en"
import i18n from "./locales"

const locale = computed(() => {
  //console.log(333, currentLocale.value)
  //这里默认语言不起作用
  return currentLocale.value === "zh-CN" ? zhCn : en
})

//设置默认语言
onMounted(() => {
  if (localStorage.getItem("locale_language") == null) {
    changeLocale("zh-CN") //zh-CN  en-US
  }
})
const { initTheme } = useTheme()

//网页标题
document.title = "加工件过程管理系统"

/** 初始化主题 */
initTheme()
</script>

<template>
  <el-config-provider :locale="en">
    <router-view />
  </el-config-provider>
</template>
