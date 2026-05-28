import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/settings-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "settings-dashboard.element.js",
    },
    outDir: "../App_Plugins/Settings/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
