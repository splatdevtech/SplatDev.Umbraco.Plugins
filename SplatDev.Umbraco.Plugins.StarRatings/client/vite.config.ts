import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/star-ratings-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "star-ratings-dashboard.element.js",
    },
    outDir: "../App_Plugins/StarRatings/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
