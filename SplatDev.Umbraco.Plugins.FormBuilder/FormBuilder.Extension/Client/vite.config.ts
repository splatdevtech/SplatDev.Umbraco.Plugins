import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: {
        "form-list-dashboard": "src/dashboards/form-list-dashboard.element.ts",
        "form-editor-dashboard": "src/dashboards/form-editor-dashboard.element.ts",
        "welcome-dashboard": "src/dashboards/welcome-dashboard-element.ts",
      },
      formats: ["es"],
    },
    outDir: "../wwwroot/App_Plugins/FormBuilderExtension",
    emptyOutDir: true,
    sourcemap: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
