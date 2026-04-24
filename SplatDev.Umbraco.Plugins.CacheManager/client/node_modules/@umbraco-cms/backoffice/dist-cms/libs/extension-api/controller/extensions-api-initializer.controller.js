import { UmbBaseExtensionsInitializer, } from './base-extensions-initializer.controller.js';
import { UmbExtensionApiInitializer } from './extension-api-initializer.controller.js';
/**
 * This Controller manages a set of Extensions and their Manifest.
 * When one or more extensions is permitted to be used, the callback gives all permitted extensions and their manifest.
 * @example
 * ```ts
TODO: Correct this, start using builder pattern:
 * const controller = new UmbExtensionsApiInitializer(host, extensionRegistry, type, ['constructor argument 1', 'constructor argument '], filter?, (permitted, ctrl) => { console.log("Extension is permitted and this is the manifest: ", ctrl.manifest) }));
 * ```
 * @class UmbExtensionsApiInitializer
 */
export class UmbExtensionsApiInitializer extends UmbBaseExtensionsInitializer {
    //
    #extensionRegistry;
    /*
    #props?: Record<string, unknown>;

    public get properties() {
        return this.#props;
    }
    public set properties(props: Record<string, unknown> | undefined) {
        this.#props = props;
        this._extensions.forEach((controller) => {
            controller.properties = props;
        });
    }
    */
    #constructorArgs;
    constructor(host, extensionRegistry, type, constructorArguments, filter, onChange, controllerAlias, args) {
        super(host, extensionRegistry, type, filter, onChange, controllerAlias, args);
        this.#extensionRegistry = extensionRegistry;
        this.#constructorArgs = constructorArguments;
        this._init();
    }
    _createController(manifest) {
        const extController = new UmbExtensionApiInitializer(this, this.#extensionRegistry, manifest.alias, this.#constructorArgs, this._extensionChanged);
        return extController;
    }
    destroy() {
        super.destroy();
        this.#constructorArgs = undefined;
        this.#extensionRegistry = undefined;
    }
}
