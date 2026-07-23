import { defineConfig } from "vite";
import { resolve } from "path";

export default defineConfig({
  build: {
    lib: {
      entry: {
        "emailtemplates-dashboard": resolve(__dirname, "src/emailtemplates-dashboard.element.ts"),
        "emailstyles-dashboard": resolve(__dirname, "src/emailstyles-dashboard.element.ts"),
      },
      formats: ["es"],
      fileName: (_format, entryName) => `${entryName}.element.js`,
    },
    outDir: "../App_Plugins/SplatDev.EmailTemplates/dist",
    emptyOutDir: true,
    rollupOptions: {
      external: [/^@umbraco/],
    },
  },
});
