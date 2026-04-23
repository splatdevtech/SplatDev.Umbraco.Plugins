import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/quick-poll-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "quick-poll-dashboard.element.js",
    },
    outDir: "../App_Plugins/QuickPoll/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
