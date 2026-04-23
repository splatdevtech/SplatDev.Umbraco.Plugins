import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/copyvalue-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "copyvalue-dashboard.element.js",
    },
    outDir: "../App_Plugins/CopyValue/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
