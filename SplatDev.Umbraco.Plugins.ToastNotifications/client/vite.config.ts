import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/toastnotifications-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "toastnotifications-dashboard.element.js",
    },
    outDir: "../App_Plugins/ToastNotifications/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
