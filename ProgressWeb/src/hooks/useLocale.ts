import { useI18n } from "vue-i18n"
import { computed } from "vue"

export default function useLocale() {
  const i18n = useI18n()
  /**获取当前语言 */
  const currentLocale = computed(() => {
    //console.log(1212,i18n.locale.value)
    return i18n.locale.value
  })
  //console.log(currentLocale.value)
  /**切换语言 */
  const changeLocale = (value: string) => {
    if (i18n.locale.value === value) {
      return
    }
    i18n.locale.value = value
    localStorage.setItem("locale_language", value)
  }

  return {
    currentLocale,
    changeLocale
  }
}
