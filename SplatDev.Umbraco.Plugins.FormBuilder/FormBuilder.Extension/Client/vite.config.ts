import { defineConfig } from "vite";

export default defineConfig({
    build: {
        lib: {
            entry: "src/dashboards/welcome-dashboard-element.ts",
            formats: ["es"],
            fileName: "welcome-dashboard",
        },
        outDir: "../wwwroot/App_Plugins/FormBuilderExtension",
        emptyOutDir: true,
        sourcemap: true,
        rollupOptions: {
            external: [/^@umbraco/],
        },
    }
});
