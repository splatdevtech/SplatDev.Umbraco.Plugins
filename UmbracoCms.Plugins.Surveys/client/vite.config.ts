import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/surveys-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "surveys-dashboard.element.js",
    },
    outDir: "../App_Plugins/Surveys/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
