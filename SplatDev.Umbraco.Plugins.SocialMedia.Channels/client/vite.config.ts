import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/socialmedia-channels-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "socialmedia-channels-dashboard.element.js",
    },
    outDir: "../App_Plugins/SocialMediaChannels/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
