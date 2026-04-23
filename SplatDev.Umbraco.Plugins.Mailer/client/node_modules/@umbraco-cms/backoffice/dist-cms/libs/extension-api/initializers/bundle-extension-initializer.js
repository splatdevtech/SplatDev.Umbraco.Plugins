import { loadManifestPlainJs } from '../functions/load-manifest-plain-js.function.js';
import { UmbExtensionInitializerBase } from './extension-initializer-base.js';
/**
 * Extension initializer for the `bundle` extension type
 */
export class UmbBundleExtensionInitializer extends UmbExtensionInitializerBase {
    constructor(host, extensionRegistry) {
        super(host, extensionRegistry, 'bundle');
    }
    async instantiateExtension(manifest) {
        if (manifest.js) {
            const js = await loadManifestPlainJs(manifest.js);
            if (js) {
                Object.keys(js).forEach((key) => {
                    const value = js[key];
                    if (Array.isArray(value)) {
                        this.extensionRegistry.registerMany(value);
                    }
                    else if (typeof value === 'object') {
                        this.extensionRegistry.register(value);
                    }
                });
            }
        }
    }
    async unloadExtension(manifest) {
        if (manifest.js) {
            const js = await loadManifestPlainJs(manifest.js);
            if (js) {
                Object.keys(js).forEach((key) => {
                    const value = js[key];
                    if (Array.isArray(value)) {
                        this.extensionRegistry.unregisterMany(value.map((v) => v.alias));
                    }
                    else if (typeof value === 'object') {
                        this.extensionRegistry.unregister(value.alias);
                    }
                });
            }
        }
    }
}
