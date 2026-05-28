import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: {
        "newsletter-subscribers-dashboard.element": "src/newsletter-subscribers-dashboard.element.ts",
        "newsletter-campaigns-dashboard.element": "src/newsletter-campaigns-dashboard.element.ts",
        "newsletter-analytics-dashboard.element": "src/newsletter-analytics-dashboard.element.ts",
      },
      formats: ["es"],
    },
    outDir: "../App_Plugins/SplatDev.Newsletter/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
