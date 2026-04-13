import { ref, watch } from "vue"
import { getSidebarI18nPath } from "@/locales/langMap"
import i18n from "../locales"

/** 项目标题 */
const VITE_APP_TITLE = import.meta.env.VITE_APP_TITLE ?? "MES"

/** 动态标题 */
const dynamicTitle = ref<string>("")

/** 设置标题 */
const setTitle = (title?: string) => {
  const path = title ? getSidebarI18nPath(title) : undefined
  const title1 = path ? i18n.global.t(path) : title ?? ""
  dynamicTitle.value = title1 ? `${VITE_APP_TITLE} | ${title1}` : VITE_APP_TITLE
}

/** 监听标题变化 */
watch(dynamicTitle, (value, oldValue) => {
  if (document && value !== oldValue) {
    document.title = value
  }
})

export function useTitle() {
  return { setTitle }
}
