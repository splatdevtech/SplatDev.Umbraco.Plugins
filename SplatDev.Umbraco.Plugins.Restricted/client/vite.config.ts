import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/restricted-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "restricted-dashboard.element.js",
    },
    outDir: "../App_Plugins/Restricted/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
