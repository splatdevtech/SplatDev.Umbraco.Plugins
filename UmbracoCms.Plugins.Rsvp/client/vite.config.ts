import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/rsvp-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "rsvp-dashboard.element.js",
    },
    outDir: "../App_Plugins/Rsvp/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
