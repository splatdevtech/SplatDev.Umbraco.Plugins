import { createExtensionApi } from '../functions/create-extension-api.function.js';
import { UmbBaseExtensionInitializer } from './base-extension-initializer.controller.js';
/**
 * This Controller manages a single Extension and its API instance.
 * When the extension is permitted to be used, its API will be instantiated and available for the consumer.
 * @example
 * ```ts
 * const controller = new UmbExtensionApiController(host, extensionRegistry, alias, [], (permitted, ctrl) => { ctrl.api.helloWorld() }));
 * ```
 * @class UmbExtensionApiController
 */
export class UmbExtensionApiInitializer extends UmbBaseExtensionInitializer {
    #api;
    #constructorArguments;
    /**
     * The api that is created for this extension.
     * @readonly
     * @type {(class | undefined)}
     */
    get api() {
        return this.#api;
    }
    /**
     * The props that are passed to the class.
     * @type {Record<string, any>}
     * @memberof UmbExtensionApiController
     * @example
     * ```ts
     * const controller = new UmbExtensionApiController(host, extensionRegistry, alias, [], onPermissionChanged);
     * controller.props = { foo: 'bar' };
     * ```
     * Is equivalent to:
     * ```ts
     * controller.component.foo = 'bar';
     * ```
     */
    /*
    #properties?: Record<string, unknown>;
    get properties() {
        return this.#properties;
    }
    set properties(newVal) {
        this.#properties = newVal;
        // TODO: we could optimize this so we only re-set the changed props.
        this.#assignProperties();
    }
    */
    constructor(host, extensionRegistry, alias, constructorArguments, onPermissionChanged) {
        super(host, extensionRegistry, 'extApi_', alias, onPermissionChanged);
        this.#constructorArguments = constructorArguments;
        this._init();
    }
    /*
    #assignProperties = () => {
        if (!this._api || !this.#properties) return;

        // TODO: we could optimize this so we only re-set the updated props.
        Object.keys(this.#properties).forEach((key) => {
            (this._api as any)[key] = this.#properties![key];
        });
    };
    */
    async _conditionsAreGood() {
        const manifest = this.manifest; // In this case we are sure its not undefined.
        const newApi = await createExtensionApi(this._host, manifest, this.#constructorArguments);
        if (!this._isConditionsPositive) {
            // We are not positive anymore, so we will back out of this creation.
            return false;
        }
        this.#api = newApi;
        if (this.#api) {
            this.#api.manifest = manifest;
            //this.#assignProperties();
            return true; // we will confirm we have a component and are still good to go.
        }
        console.warn('Manifest did not provide any useful data for a api class to construct.');
        return false; // we will reject the state, we have no component, we are not good to be shown.
    }
    async _conditionsAreBad() {
        // Destroy the api:
        if (this.#api) {
            if ('destroy' in this.#api) {
                this.#api.destroy();
            }
            this.#api = undefined;
        }
    }
    destroy() {
        super.destroy();
        this.#constructorArguments = undefined;
    }
}
