import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/seo-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "seo-dashboard.element.js",
    },
    outDir: "../App_Plugins/SEO/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
