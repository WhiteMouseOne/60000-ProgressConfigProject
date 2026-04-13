<template>
  <el-dropdown trigger="click" @command="changeLanguage">
    <!-- <i :class="'iconfont icon-zhongyingwen'" class="toolBar-icon"></i> -->
    <svg-icon name="zh" class="toolBar-icon"></svg-icon>
    <!-- <SvgIcon name="fullscreen" /> -->
    <template #dropdown>
      <el-dropdown-menu>
        <el-dropdown-item
          v-for="item in languageList"
          :key="item.value"
          :command="item.value"
          :disabled="currentLanguage === item.value"
        >
          {{ item.label }}
        </el-dropdown-item>
      </el-dropdown-menu>
    </template>
  </el-dropdown>
</template>

<script setup lang="ts">
// //   import { useI18n } from "vue-i18n";
import { computed, ref } from "vue"
import { languageLocal } from "@/locales/index"
import useLocale from "@/hooks/useLocale"
const { currentLocale, changeLocale } = useLocale()

const currentLanguage = computed(() => currentLocale.value)

// 语言列表
const languageList = ref(languageLocal)

// 切换语言
const changeLanguage = (lang: string) => {
  console.log(123,lang)
  changeLocale(lang)
}
</script>
<style lang="scss" scoped>
.toolBar-icon {
  width: 23px;
  height: 23px;
  cursor: pointer;
  margin-right: 10px;
}
.svg-icon {
  font-size: 20px;
  &:focus {
    outline: none;
  }
}
</style>
