import { UmbBaseExtensionInitializer } from './base-extension-initializer.controller.js';
/**
 * This Controller manages a single Extension and its Manifest.
 * When the extension is permitted to be used, the manifest is available for the consumer.
 * @example
 * ```ts
 * const controller = new UmbExtensionManifestController(host, extensionRegistry, alias, (permitted, ctrl) => { console.log("Extension is permitted and this is the manifest: ", ctrl.manifest) }));
 * ```
 * @class UmbExtensionManifestController
 */
export class UmbExtensionManifestInitializer extends UmbBaseExtensionInitializer {
    constructor(host, extensionRegistry, alias, onPermissionChanged) {
        super(host, extensionRegistry, 'extManifest_', alias, onPermissionChanged);
        this._init();
    }
    async _conditionsAreGood() {
        return true;
    }
    async _conditionsAreBad() {
        // Destroy the element/class.
    }
}
