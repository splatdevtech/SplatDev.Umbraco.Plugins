import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: {
        "workflow-queue.element": "src/workflow-queue.element.ts",
        "workflow-editor.element": "src/workflow-editor.element.ts",
      },
      formats: ["es"],
    },
    outDir: "../App_Plugins/SplatDev.Workflow/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
