export const getDefaultConfig = (args) => {
    return {
        build: {
            target: 'es2022',
            lib: {
                entry: args.entry || ['index.ts', 'manifests.ts', 'umbraco-package.ts'],
                formats: ['es'],
            },
            outDir: args.dist,
            emptyOutDir: true,
            sourcemap: true,
            rollupOptions: {
                external: args.external || [/^@umbraco-cms/],
            },
        },
        plugins: args.plugins,
        base: args.base,
    };
};
