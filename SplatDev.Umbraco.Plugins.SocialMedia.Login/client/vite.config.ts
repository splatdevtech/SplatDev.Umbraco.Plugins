import { defineConfig } from "vite";

export default defineConfig({
  build: {
    lib: {
      entry: "src/socialmedia-login-dashboard.element.ts",
      formats: ["es"],
      fileName: () => "socialmedia-login-dashboard.element.js",
    },
    outDir: "../App_Plugins/SocialMediaLogin/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
