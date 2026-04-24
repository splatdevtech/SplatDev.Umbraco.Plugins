import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/hiddencontent-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "hiddencontent-dashboard.element.js",
    },
    outDir: "../App_Plugins/HiddenContent/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
