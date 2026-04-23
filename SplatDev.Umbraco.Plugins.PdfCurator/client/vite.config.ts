import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/pdf-curator-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "pdf-curator-dashboard.element.js",
    },
    outDir: "../App_Plugins/PdfCurator/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
