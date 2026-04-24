import { UmbExtensionManifestInitializer } from './extension-manifest-initializer.controller.js';
import { UmbBaseExtensionsInitializer, } from './base-extensions-initializer.controller.js';
/**
 */
export class UmbExtensionsManifestInitializer extends UmbBaseExtensionsInitializer {
    //
    #extensionRegistry;
    constructor(host, extensionRegistry, type, filter, onChange, controllerAlias, args) {
        super(host, extensionRegistry, type, filter, onChange, controllerAlias, args);
        this.#extensionRegistry = extensionRegistry;
        this._init();
    }
    _createController(manifest) {
        return new UmbExtensionManifestInitializer(this, this.#extensionRegistry, manifest.alias, this._extensionChanged);
    }
    destroy() {
        super.destroy();
        this.#extensionRegistry = undefined;
    }
}
