import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/socialmedia-share-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "socialmedia-share-dashboard.element.js",
    },
    outDir: "../App_Plugins/SocialMediaShare/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
