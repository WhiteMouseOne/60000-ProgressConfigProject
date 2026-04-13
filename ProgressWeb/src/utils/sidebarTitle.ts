import type { RouteRecordName } from "vue-router"
import i18n from "@/locales"
import { getSidebarI18nPath } from "@/locales/langMap"

/** 侧栏/面包屑/标签：优先 langMap+sidebar.xxx，无映射时用后端菜单 meta.title */
export function resolveSidebarTitle(item: {
  name?: RouteRecordName | null
  meta?: { title?: string }
}): string {
  const path = getSidebarI18nPath(item.name ?? undefined)
  if (path) return i18n.global.t(path)
  const t = item.meta?.title
  if (typeof t === "string" && t.length > 0) return t
  if (typeof item.name === "string") return item.name
  return ""
}
