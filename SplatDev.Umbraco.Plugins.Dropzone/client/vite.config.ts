import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/dropzone-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "dropzone-dashboard.element.js",
    },
    outDir: "../App_Plugins/Dropzone/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
