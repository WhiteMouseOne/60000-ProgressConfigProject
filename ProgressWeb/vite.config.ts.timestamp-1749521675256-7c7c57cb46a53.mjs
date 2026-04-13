// vite.config.ts
import { loadEnv } from "file:///D:/01%20Software/MES_BASE/LineMesWeb/node_modules/vite/dist/node/index.js";
import path, { resolve } from "path";
import vue from "file:///D:/01%20Software/MES_BASE/LineMesWeb/node_modules/@vitejs/plugin-vue/dist/index.mjs";
import vueJsx from "file:///D:/01%20Software/MES_BASE/LineMesWeb/node_modules/@vitejs/plugin-vue-jsx/dist/index.mjs";
import { createSvgIconsPlugin } from "file:///D:/01%20Software/MES_BASE/LineMesWeb/node_modules/vite-plugin-svg-icons/dist/index.mjs";
import vueSetupExtend from "file:///D:/01%20Software/MES_BASE/LineMesWeb/node_modules/unplugin-vue-setup-extend-plus/dist/vite.js";
import svgLoader from "file:///D:/01%20Software/MES_BASE/LineMesWeb/node_modules/vite-svg-loader/index.js";
import UnoCSS from "file:///D:/01%20Software/MES_BASE/LineMesWeb/node_modules/unocss/dist/vite.mjs";
var __vite_injected_original_dirname = "D:\\01 Software\\MES_BASE\\LineMesWeb";
var vite_config_default = (configEnv) => {
  const viteEnv = loadEnv(configEnv.mode, process.cwd());
  const { VITE_PUBLIC_PATH } = viteEnv;
  return {
    /** 打包时根据实际情况修改 base */
    base: VITE_PUBLIC_PATH,
    resolve: {
      alias: {
        /** @ 符号指向 src 目录 */
        "@": resolve(__vite_injected_original_dirname, "./src")
      }
    },
    server: {
      /** 设置 host: true 才可以使用 Network 的形式，以 IP 访问项目 */
      host: true,
      // host: "0.0.0.0"
      /** 端口号 */
      port: 3333,
      /** 是否自动打开浏览器 */
      open: false,
      /** 跨域设置允许 */
      cors: true,
      /** 端口被占用时，是否直接退出 */
      strictPort: false,
      /** 接口代理 */
      proxy: {
        "/api": {
          target: "http://localhost:7235",
          ws: true,
          /** 是否允许跨域 */
          changeOrigin: true
        },
        "/hubs": {
          target: "http://localhost:7235",
          ws: true,
          /** 是否允许跨域 */
          changeOrigin: true
        }
      },
      /** 预热常用文件，提高初始页面加载速度 */
      warmup: {
        clientFiles: ["./src/layouts/**/*.vue"]
      }
    },
    build: {
      /** 单个 chunk 文件的大小超过 2048KB 时发出警告 */
      chunkSizeWarningLimit: 2048,
      /** 禁用 gzip 压缩大小报告 */
      reportCompressedSize: false,
      /** 打包后静态资源目录 */
      assetsDir: "static",
      rollupOptions: {
        output: {
          /**
           * 分块策略
           * 1. 注意这些包名必须存在，否则打包会报错
           * 2. 如果你不想自定义 chunk 分割策略，可以直接移除这段配置
           */
          manualChunks: {
            vue: ["vue", "vue-router", "pinia"],
            element: ["element-plus", "@element-plus/icons-vue"],
            vxe: ["vxe-table", "vxe-table-plugin-element", "xe-utils"]
          }
        }
      }
    },
    /** 混淆器 */
    esbuild: {
      /** 打包时移除 console.log */
      pure: ["console.log"],
      /** 打包时移除 debugger */
      drop: ["debugger"],
      /** 打包时移除所有注释 */
      legalComments: "none"
    },
    /** Vite 插件 */
    plugins: [
      vue(),
      vueJsx(),
      /** 将 SVG 静态图转化为 Vue 组件 */
      svgLoader({ defaultImport: "url" }),
      /** SVG */
      createSvgIconsPlugin({
        iconDirs: [path.resolve(process.cwd(), "src/icons/svg")],
        symbolId: "icon-[dir]-[name]"
      }),
      /** UnoCSS */
      UnoCSS(),
      /**组件name插件 */
      vueSetupExtend({
        /* options */
      })
    ],
    /** Vitest 单元测试配置：https://cn.vitest.dev/config */
    test: {
      include: ["tests/**/*.test.ts"],
      environment: "jsdom"
    }
  };
};
export {
  vite_config_default as default
};
//# sourceMappingURL=data:application/json;base64,ewogICJ2ZXJzaW9uIjogMywKICAic291cmNlcyI6IFsidml0ZS5jb25maWcudHMiXSwKICAic291cmNlc0NvbnRlbnQiOiBbImNvbnN0IF9fdml0ZV9pbmplY3RlZF9vcmlnaW5hbF9kaXJuYW1lID0gXCJEOlxcXFwwMSBTb2Z0d2FyZVxcXFxNRVNfQkFTRVxcXFxMaW5lTWVzV2ViXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ZpbGVuYW1lID0gXCJEOlxcXFwwMSBTb2Z0d2FyZVxcXFxNRVNfQkFTRVxcXFxMaW5lTWVzV2ViXFxcXHZpdGUuY29uZmlnLnRzXCI7Y29uc3QgX192aXRlX2luamVjdGVkX29yaWdpbmFsX2ltcG9ydF9tZXRhX3VybCA9IFwiZmlsZTovLy9EOi8wMSUyMFNvZnR3YXJlL01FU19CQVNFL0xpbmVNZXNXZWIvdml0ZS5jb25maWcudHNcIjsvLy8gPHJlZmVyZW5jZSB0eXBlcz1cInZpdGVzdFwiIC8+XHJcblxyXG5pbXBvcnQgeyB0eXBlIENvbmZpZ0VudiwgdHlwZSBVc2VyQ29uZmlnRXhwb3J0LCBsb2FkRW52IH0gZnJvbSBcInZpdGVcIlxyXG5pbXBvcnQgcGF0aCwgeyByZXNvbHZlIH0gZnJvbSBcInBhdGhcIlxyXG5pbXBvcnQgdnVlIGZyb20gXCJAdml0ZWpzL3BsdWdpbi12dWVcIlxyXG5pbXBvcnQgdnVlSnN4IGZyb20gXCJAdml0ZWpzL3BsdWdpbi12dWUtanN4XCJcclxuaW1wb3J0IHsgY3JlYXRlU3ZnSWNvbnNQbHVnaW4gfSBmcm9tIFwidml0ZS1wbHVnaW4tc3ZnLWljb25zXCJcclxuaW1wb3J0IHZ1ZVNldHVwRXh0ZW5kIGZyb20gXCJ1bnBsdWdpbi12dWUtc2V0dXAtZXh0ZW5kLXBsdXMvdml0ZVwiXHJcbmltcG9ydCBzdmdMb2FkZXIgZnJvbSBcInZpdGUtc3ZnLWxvYWRlclwiXHJcbmltcG9ydCBVbm9DU1MgZnJvbSBcInVub2Nzcy92aXRlXCJcclxuXHJcbi8qKiBcdTkxNERcdTdGNkVcdTk4NzlcdTY1ODdcdTY4NjNcdUZGMUFodHRwczovL2NuLnZpdGVqcy5kZXYvY29uZmlnICovXHJcbmV4cG9ydCBkZWZhdWx0IChjb25maWdFbnY6IENvbmZpZ0Vudik6IFVzZXJDb25maWdFeHBvcnQgPT4ge1xyXG4gIGNvbnN0IHZpdGVFbnYgPSBsb2FkRW52KGNvbmZpZ0Vudi5tb2RlLCBwcm9jZXNzLmN3ZCgpKSBhcyBJbXBvcnRNZXRhRW52XHJcbiAgY29uc3QgeyBWSVRFX1BVQkxJQ19QQVRIIH0gPSB2aXRlRW52XHJcbiAgcmV0dXJuIHtcclxuICAgIC8qKiBcdTYyNTNcdTUzMDVcdTY1RjZcdTY4MzlcdTYzNkVcdTVCOUVcdTk2NDVcdTYwQzVcdTUxQjVcdTRGRUVcdTY1MzkgYmFzZSAqL1xyXG4gICAgYmFzZTogVklURV9QVUJMSUNfUEFUSCxcclxuICAgIHJlc29sdmU6IHtcclxuICAgICAgYWxpYXM6IHtcclxuICAgICAgICAvKiogQCBcdTdCMjZcdTUzRjdcdTYzMDdcdTU0MTEgc3JjIFx1NzZFRVx1NUY1NSAqL1xyXG4gICAgICAgIFwiQFwiOiByZXNvbHZlKF9fZGlybmFtZSwgXCIuL3NyY1wiKVxyXG4gICAgICB9XHJcbiAgICB9LFxyXG4gICAgc2VydmVyOiB7XHJcbiAgICAgIC8qKiBcdThCQkVcdTdGNkUgaG9zdDogdHJ1ZSBcdTYyNERcdTUzRUZcdTRFRTVcdTRGN0ZcdTc1MjggTmV0d29yayBcdTc2ODRcdTVGNjJcdTVGMEZcdUZGMENcdTRFRTUgSVAgXHU4QkJGXHU5NUVFXHU5ODc5XHU3NkVFICovXHJcbiAgICAgIGhvc3Q6IHRydWUsIC8vIGhvc3Q6IFwiMC4wLjAuMFwiXHJcbiAgICAgIC8qKiBcdTdBRUZcdTUzRTNcdTUzRjcgKi9cclxuICAgICAgcG9ydDogMzMzMyxcclxuICAgICAgLyoqIFx1NjYyRlx1NTQyNlx1ODFFQVx1NTJBOFx1NjI1M1x1NUYwMFx1NkQ0Rlx1ODlDOFx1NTY2OCAqL1xyXG4gICAgICBvcGVuOiBmYWxzZSxcclxuICAgICAgLyoqIFx1OERFOFx1NTdERlx1OEJCRVx1N0Y2RVx1NTE0MVx1OEJCOCAqL1xyXG4gICAgICBjb3JzOiB0cnVlLFxyXG4gICAgICAvKiogXHU3QUVGXHU1M0UzXHU4OEFCXHU1MzYwXHU3NTI4XHU2NUY2XHVGRjBDXHU2NjJGXHU1NDI2XHU3NkY0XHU2M0E1XHU5MDAwXHU1MUZBICovXHJcbiAgICAgIHN0cmljdFBvcnQ6IGZhbHNlLFxyXG4gICAgICAvKiogXHU2M0E1XHU1M0UzXHU0RUUzXHU3NDA2ICovXHJcbiAgICAgIHByb3h5OiB7XHJcbiAgICAgICAgXCIvYXBpXCI6IHtcclxuICAgICAgICAgIHRhcmdldDogXCJodHRwOi8vbG9jYWxob3N0OjcyMzVcIixcclxuICAgICAgICAgIHdzOiB0cnVlLFxyXG4gICAgICAgICAgLyoqIFx1NjYyRlx1NTQyNlx1NTE0MVx1OEJCOFx1OERFOFx1NTdERiAqL1xyXG4gICAgICAgICAgY2hhbmdlT3JpZ2luOiB0cnVlXHJcbiAgICAgICAgfSxcclxuICAgICAgICBcIi9odWJzXCI6IHtcclxuICAgICAgICAgIHRhcmdldDogXCJodHRwOi8vbG9jYWxob3N0OjcyMzVcIixcclxuICAgICAgICAgIHdzOiB0cnVlLFxyXG4gICAgICAgICAgLyoqIFx1NjYyRlx1NTQyNlx1NTE0MVx1OEJCOFx1OERFOFx1NTdERiAqL1xyXG4gICAgICAgICAgY2hhbmdlT3JpZ2luOiB0cnVlXHJcbiAgICAgICAgfVxyXG4gICAgICB9LFxyXG4gICAgICAvKiogXHU5ODg0XHU3MEVEXHU1RTM4XHU3NTI4XHU2NTg3XHU0RUY2XHVGRjBDXHU2M0QwXHU5QUQ4XHU1MjFEXHU1OUNCXHU5ODc1XHU5NzYyXHU1MkEwXHU4RjdEXHU5MDFGXHU1RUE2ICovXHJcbiAgICAgIHdhcm11cDoge1xyXG4gICAgICAgIGNsaWVudEZpbGVzOiBbXCIuL3NyYy9sYXlvdXRzLyoqLyoudnVlXCJdXHJcbiAgICAgIH1cclxuICAgIH0sXHJcbiAgICBidWlsZDoge1xyXG4gICAgICAvKiogXHU1MzU1XHU0RTJBIGNodW5rIFx1NjU4N1x1NEVGNlx1NzY4NFx1NTkyN1x1NUMwRlx1OEQ4NVx1OEZDNyAyMDQ4S0IgXHU2NUY2XHU1M0QxXHU1MUZBXHU4QjY2XHU1NDRBICovXHJcbiAgICAgIGNodW5rU2l6ZVdhcm5pbmdMaW1pdDogMjA0OCxcclxuICAgICAgLyoqIFx1Nzk4MVx1NzUyOCBnemlwIFx1NTM4Qlx1N0YyOVx1NTkyN1x1NUMwRlx1NjJBNVx1NTQ0QSAqL1xyXG4gICAgICByZXBvcnRDb21wcmVzc2VkU2l6ZTogZmFsc2UsXHJcbiAgICAgIC8qKiBcdTYyNTNcdTUzMDVcdTU0MEVcdTk3NTlcdTYwMDFcdThENDRcdTZFOTBcdTc2RUVcdTVGNTUgKi9cclxuICAgICAgYXNzZXRzRGlyOiBcInN0YXRpY1wiLFxyXG4gICAgICByb2xsdXBPcHRpb25zOiB7XHJcbiAgICAgICAgb3V0cHV0OiB7XHJcbiAgICAgICAgICAvKipcclxuICAgICAgICAgICAqIFx1NTIwNlx1NTc1N1x1N0I1Nlx1NzU2NVxyXG4gICAgICAgICAgICogMS4gXHU2Q0U4XHU2MTBGXHU4RkQ5XHU0RTlCXHU1MzA1XHU1NDBEXHU1RkM1XHU5ODdCXHU1QjU4XHU1NzI4XHVGRjBDXHU1NDI2XHU1MjE5XHU2MjUzXHU1MzA1XHU0RjFBXHU2MkE1XHU5NTE5XHJcbiAgICAgICAgICAgKiAyLiBcdTU5ODJcdTY3OUNcdTRGNjBcdTRFMERcdTYwRjNcdTgxRUFcdTVCOUFcdTRFNDkgY2h1bmsgXHU1MjA2XHU1MjcyXHU3QjU2XHU3NTY1XHVGRjBDXHU1M0VGXHU0RUU1XHU3NkY0XHU2M0E1XHU3OUZCXHU5NjY0XHU4RkQ5XHU2QkI1XHU5MTREXHU3RjZFXHJcbiAgICAgICAgICAgKi9cclxuICAgICAgICAgIG1hbnVhbENodW5rczoge1xyXG4gICAgICAgICAgICB2dWU6IFtcInZ1ZVwiLCBcInZ1ZS1yb3V0ZXJcIiwgXCJwaW5pYVwiXSxcclxuICAgICAgICAgICAgZWxlbWVudDogW1wiZWxlbWVudC1wbHVzXCIsIFwiQGVsZW1lbnQtcGx1cy9pY29ucy12dWVcIl0sXHJcbiAgICAgICAgICAgIHZ4ZTogW1widnhlLXRhYmxlXCIsIFwidnhlLXRhYmxlLXBsdWdpbi1lbGVtZW50XCIsIFwieGUtdXRpbHNcIl1cclxuICAgICAgICAgIH1cclxuICAgICAgICB9XHJcbiAgICAgIH1cclxuICAgIH0sXHJcbiAgICAvKiogXHU2REY3XHU2REM2XHU1NjY4ICovXHJcbiAgICBlc2J1aWxkOiB7XHJcbiAgICAgIC8qKiBcdTYyNTNcdTUzMDVcdTY1RjZcdTc5RkJcdTk2NjQgY29uc29sZS5sb2cgKi9cclxuICAgICAgcHVyZTogW1wiY29uc29sZS5sb2dcIl0sXHJcbiAgICAgIC8qKiBcdTYyNTNcdTUzMDVcdTY1RjZcdTc5RkJcdTk2NjQgZGVidWdnZXIgKi9cclxuICAgICAgZHJvcDogW1wiZGVidWdnZXJcIl0sXHJcbiAgICAgIC8qKiBcdTYyNTNcdTUzMDVcdTY1RjZcdTc5RkJcdTk2NjRcdTYyNDBcdTY3MDlcdTZDRThcdTkxQ0EgKi9cclxuICAgICAgbGVnYWxDb21tZW50czogXCJub25lXCJcclxuICAgIH0sXHJcbiAgICAvKiogVml0ZSBcdTYzRDJcdTRFRjYgKi9cclxuICAgIHBsdWdpbnM6IFtcclxuICAgICAgdnVlKCksXHJcbiAgICAgIHZ1ZUpzeCgpLFxyXG4gICAgICAvKiogXHU1QzA2IFNWRyBcdTk3NTlcdTYwMDFcdTU2RkVcdThGNkNcdTUzMTZcdTRFM0EgVnVlIFx1N0VDNFx1NEVGNiAqL1xyXG4gICAgICBzdmdMb2FkZXIoeyBkZWZhdWx0SW1wb3J0OiBcInVybFwiIH0pLFxyXG4gICAgICAvKiogU1ZHICovXHJcbiAgICAgIGNyZWF0ZVN2Z0ljb25zUGx1Z2luKHtcclxuICAgICAgICBpY29uRGlyczogW3BhdGgucmVzb2x2ZShwcm9jZXNzLmN3ZCgpLCBcInNyYy9pY29ucy9zdmdcIildLFxyXG4gICAgICAgIHN5bWJvbElkOiBcImljb24tW2Rpcl0tW25hbWVdXCJcclxuICAgICAgfSksXHJcbiAgICAgIC8qKiBVbm9DU1MgKi9cclxuICAgICAgVW5vQ1NTKCksXHJcbiAgICAgIC8qKlx1N0VDNFx1NEVGNm5hbWVcdTYzRDJcdTRFRjYgKi9cclxuICAgICAgdnVlU2V0dXBFeHRlbmQoe1xyXG4gICAgICAgIC8qIG9wdGlvbnMgKi9cclxuICAgICAgfSlcclxuICAgIF0sXHJcbiAgICAvKiogVml0ZXN0IFx1NTM1NVx1NTE0M1x1NkQ0Qlx1OEJENVx1OTE0RFx1N0Y2RVx1RkYxQWh0dHBzOi8vY24udml0ZXN0LmRldi9jb25maWcgKi9cclxuICAgIHRlc3Q6IHtcclxuICAgICAgaW5jbHVkZTogW1widGVzdHMvKiovKi50ZXN0LnRzXCJdLFxyXG4gICAgICBlbnZpcm9ubWVudDogXCJqc2RvbVwiXHJcbiAgICB9XHJcbiAgfVxyXG59XHJcbiJdLAogICJtYXBwaW5ncyI6ICI7QUFFQSxTQUFnRCxlQUFlO0FBQy9ELE9BQU8sUUFBUSxlQUFlO0FBQzlCLE9BQU8sU0FBUztBQUNoQixPQUFPLFlBQVk7QUFDbkIsU0FBUyw0QkFBNEI7QUFDckMsT0FBTyxvQkFBb0I7QUFDM0IsT0FBTyxlQUFlO0FBQ3RCLE9BQU8sWUFBWTtBQVRuQixJQUFNLG1DQUFtQztBQVl6QyxJQUFPLHNCQUFRLENBQUMsY0FBMkM7QUFDekQsUUFBTSxVQUFVLFFBQVEsVUFBVSxNQUFNLFFBQVEsSUFBSSxDQUFDO0FBQ3JELFFBQU0sRUFBRSxpQkFBaUIsSUFBSTtBQUM3QixTQUFPO0FBQUE7QUFBQSxJQUVMLE1BQU07QUFBQSxJQUNOLFNBQVM7QUFBQSxNQUNQLE9BQU87QUFBQTtBQUFBLFFBRUwsS0FBSyxRQUFRLGtDQUFXLE9BQU87QUFBQSxNQUNqQztBQUFBLElBQ0Y7QUFBQSxJQUNBLFFBQVE7QUFBQTtBQUFBLE1BRU4sTUFBTTtBQUFBO0FBQUE7QUFBQSxNQUVOLE1BQU07QUFBQTtBQUFBLE1BRU4sTUFBTTtBQUFBO0FBQUEsTUFFTixNQUFNO0FBQUE7QUFBQSxNQUVOLFlBQVk7QUFBQTtBQUFBLE1BRVosT0FBTztBQUFBLFFBQ0wsUUFBUTtBQUFBLFVBQ04sUUFBUTtBQUFBLFVBQ1IsSUFBSTtBQUFBO0FBQUEsVUFFSixjQUFjO0FBQUEsUUFDaEI7QUFBQSxRQUNBLFNBQVM7QUFBQSxVQUNQLFFBQVE7QUFBQSxVQUNSLElBQUk7QUFBQTtBQUFBLFVBRUosY0FBYztBQUFBLFFBQ2hCO0FBQUEsTUFDRjtBQUFBO0FBQUEsTUFFQSxRQUFRO0FBQUEsUUFDTixhQUFhLENBQUMsd0JBQXdCO0FBQUEsTUFDeEM7QUFBQSxJQUNGO0FBQUEsSUFDQSxPQUFPO0FBQUE7QUFBQSxNQUVMLHVCQUF1QjtBQUFBO0FBQUEsTUFFdkIsc0JBQXNCO0FBQUE7QUFBQSxNQUV0QixXQUFXO0FBQUEsTUFDWCxlQUFlO0FBQUEsUUFDYixRQUFRO0FBQUE7QUFBQTtBQUFBO0FBQUE7QUFBQTtBQUFBLFVBTU4sY0FBYztBQUFBLFlBQ1osS0FBSyxDQUFDLE9BQU8sY0FBYyxPQUFPO0FBQUEsWUFDbEMsU0FBUyxDQUFDLGdCQUFnQix5QkFBeUI7QUFBQSxZQUNuRCxLQUFLLENBQUMsYUFBYSw0QkFBNEIsVUFBVTtBQUFBLFVBQzNEO0FBQUEsUUFDRjtBQUFBLE1BQ0Y7QUFBQSxJQUNGO0FBQUE7QUFBQSxJQUVBLFNBQVM7QUFBQTtBQUFBLE1BRVAsTUFBTSxDQUFDLGFBQWE7QUFBQTtBQUFBLE1BRXBCLE1BQU0sQ0FBQyxVQUFVO0FBQUE7QUFBQSxNQUVqQixlQUFlO0FBQUEsSUFDakI7QUFBQTtBQUFBLElBRUEsU0FBUztBQUFBLE1BQ1AsSUFBSTtBQUFBLE1BQ0osT0FBTztBQUFBO0FBQUEsTUFFUCxVQUFVLEVBQUUsZUFBZSxNQUFNLENBQUM7QUFBQTtBQUFBLE1BRWxDLHFCQUFxQjtBQUFBLFFBQ25CLFVBQVUsQ0FBQyxLQUFLLFFBQVEsUUFBUSxJQUFJLEdBQUcsZUFBZSxDQUFDO0FBQUEsUUFDdkQsVUFBVTtBQUFBLE1BQ1osQ0FBQztBQUFBO0FBQUEsTUFFRCxPQUFPO0FBQUE7QUFBQSxNQUVQLGVBQWU7QUFBQTtBQUFBLE1BRWYsQ0FBQztBQUFBLElBQ0g7QUFBQTtBQUFBLElBRUEsTUFBTTtBQUFBLE1BQ0osU0FBUyxDQUFDLG9CQUFvQjtBQUFBLE1BQzlCLGFBQWE7QUFBQSxJQUNmO0FBQUEsRUFDRjtBQUNGOyIsCiAgIm5hbWVzIjogW10KfQo=
