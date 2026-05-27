import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/backup-manager-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "backup-manager-dashboard.element.js",
    },
    outDir: "../App_Plugins/BackupManager/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
