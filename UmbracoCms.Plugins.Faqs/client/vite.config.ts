import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/faqs-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "faqs-dashboard.element.js",
    },
    outDir: "../App_Plugins/Faqs/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
