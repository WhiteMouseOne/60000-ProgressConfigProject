import { createI18n } from "vue-i18n"
//引入对应的语言包
import zh_cn from "./lang/zh-cn"
import en_us from "./lang/en-us"
import ru_rus from "./lang/ru-rus"

//定义语言列表,之后新增在这添加即可
export const languageLocal = [
  { label: "简体中文", value: "zh-CN" },
  { label: "English", value: "en-US" },
  { label: "Russian", value: "ru-rus" }
]

const i18n = createI18n({
  locale: localStorage.getItem("locale_language") || navigator.language, //默认语言
  fallbackLocale: "en_US", //备选翻译语言
  legacy: false, // 解决懒加载组件报错
  globalInjection: true, // 全局注入$t
  messages: {
    "zh-CN": zh_cn,
    "en-US": en_us,
    "ru-rus": ru_rus
  }
})

export default i18n
