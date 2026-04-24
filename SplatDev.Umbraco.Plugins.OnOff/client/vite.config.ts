import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/onoff-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "onoff-dashboard.element.js",
    },
    outDir: "../App_Plugins/OnOff/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
