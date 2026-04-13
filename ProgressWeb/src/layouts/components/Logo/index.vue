<script lang="ts" setup>
import { useLayoutMode } from "@/hooks/useLayoutMode"
import packUpLogo from "@/assets/layouts/hcLogoLight.png?url"
// import packUpLogo from "@/assets/layouts/LOGO白黄.png?url"
import expandlLogo from "@/assets/layouts/hcLogoNoText.png?url"
import xitengLogo from "@/assets/layouts/xiteng2.png?url"
interface Props {
  collapse?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  collapse: true
})
</script>

<template>
  <div class="layout-logo-container" :class="{ collapse: props.collapse }">
    <transition name="layout-logo-fade">
      <!-- 折叠logo -->
      <router-link v-if="props.collapse" key="collapse" to="/">
        
      </router-link>
      <!-- 展开 -->
      <router-link v-else key="expand" to="/">
        <img :src="xitengLogo" class="layout-logo-text" />
      </router-link>
    </transition>
  </div>
</template>

<style lang="scss" scoped>
.layout-logo-container {
  position: relative;
  width: 100%;
  height: var(--v3-header-height);
  line-height: var(--v3-header-height);
  background-color: transparent;
  text-align: center;
  overflow: hidden;
  .layout-logo {
    display: none;
  }
  .layout-logo-text {
    height: 90%;
    width: 200px;
    vertical-align: middle;
  }
}

.layout-mode-top {
  height: var(--v3-navigationbar-height);
  line-height: var(--v3-navigationbar-height);
}

.collapse {
  .layout-logo {
    width: 48px;
    height: 43px;
    vertical-align: middle;
    display: inline-block;
  }
  .layout-logo-text {
    display: none;
  }
}
</style>
