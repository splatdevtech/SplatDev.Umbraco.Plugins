import { createExtensionElementWithApi } from '../functions/create-extension-element-with-api.function.js';
import { UmbBaseExtensionInitializer } from './base-extension-initializer.controller.js';
/**
 * This Controller manages a single Extension initializing its Element and API.
 * When the extension is permitted to be used, its Element and API will be instantiated and available for the consumer.
 * @example
 * ```ts
 * const controller = new UmbExtensionApiAndElementInitializer(host, extensionRegistry, alias, (permitted, ctrl) => { console.log("Extension is permitted and this is the element: ", ctrl.component) }));
 * ```
 * @class UmbExtensionElementAndApiInitializer
 */
export class UmbExtensionElementAndApiInitializer extends UmbBaseExtensionInitializer {
    #defaultElement;
    #defaultApi;
    #component;
    #api;
    #constructorArguments;
    /**
     * The component that is created for this extension.
     * @readonly
     * @type {(HTMLElement | undefined)}
     */
    get component() {
        return this.#component;
    }
    /**
     * The api that is created for this extension.
     * @readonly
     * @type {(class | undefined)}
     */
    get api() {
        return this.#api;
    }
    /**
     * The props that are passed to the component.
     * @type {Record<string, any>}
     * @memberof UmbElementExtensionController
     * @example
     * ```ts
     * const controller = new UmbElementExtensionController(host, extensionRegistry, alias, onPermissionChanged);
     * controller.elementProps = { foo: 'bar' };
     * ```
     * Is equivalent to:
     * ```ts
     * controller.component.foo = 'bar';
     * ```
     */
    #elProps;
    get elementProps() {
        return this.#elProps;
    }
    set elementProps(newVal) {
        this.#elProps = newVal;
        // TODO: we could optimize this so we only re-set the changed props.
        this.#assignElProps();
    }
    /**
     * The props that are passed to the api.
     * @type {Record<string, any>}
     * @memberof UmbElementExtensionController
     * @example
     * ```ts
     * const controller = new UmbElementExtensionController(host, extensionRegistry, alias, onPermissionChanged);
     * controller.apiProperties = { foo: 'bar' };
     * ```
     * Is equivalent to:
     * ```ts
     * controller.api.foo = 'bar';
     * ```
     */
    #apiProps;
    get apiProps() {
        return this.#apiProps;
    }
    set apiProps(newVal) {
        this.#apiProps = newVal;
        // TODO: we could optimize this so we only re-set the changed props.
        this.#assignApiProps();
    }
    constructor(host, extensionRegistry, alias, constructorArguments, onPermissionChanged, defaultElement, defaultApi) {
        super(host, extensionRegistry, 'extApiAndElement_', alias, onPermissionChanged);
        this.#constructorArguments = constructorArguments;
        this.#defaultElement = defaultElement;
        this.#defaultApi = defaultApi;
        this._init();
    }
    #assignElProps = () => {
        if (!this.#component || !this.#elProps)
            return;
        // TODO: we could optimize this so we only re-set the updated props.
        Object.keys(this.#elProps).forEach((key) => {
            this.#component[key] = this.#elProps[key];
        });
    };
    #assignApiProps = () => {
        if (!this.#api || !this.#apiProps)
            return;
        // TODO: we could optimize this so we only re-set the updated props.
        Object.keys(this.#apiProps).forEach((key) => {
            this.#api[key] = this.#apiProps[key];
        });
    };
    async _conditionsAreGood() {
        const manifest = this.manifest; // In this case we are sure its not undefined.
        const { element: newComponent, api: newApi } = await createExtensionElementWithApi(manifest, this.#constructorArguments, this.#defaultElement, this.#defaultApi);
        if (!this._isConditionsPositive) {
            newApi?.destroy?.();
            if (newComponent && 'destroy' in newComponent) {
                newComponent.destroy();
            }
            // We are not positive anymore, so we will back out of this creation.
            return false;
        }
        this.#api = newApi;
        if (this.#api) {
            this.#assignApiProps();
            this.#api.manifest = manifest;
        }
        else {
            console.warn('Manifest did not provide any useful data for a api to be created.');
        }
        this.#component = newComponent;
        if (this.#component) {
            this.#assignElProps();
            this.#component.manifest = manifest;
            if (this.#api) {
                this.#component.api = newApi;
            }
            return true; // we will confirm we have a component and are still good to go.
        }
        else {
            console.warn('Manifest did not provide any useful data for a web component to be created.');
        }
        return false; // we will reject the state, we have no component, we are not good to be shown.
    }
    async _conditionsAreBad() {
        // Destroy the element:
        if (this.#component) {
            if ('destroy' in this.#component) {
                this.#component.destroy();
            }
            this.#component = undefined;
        }
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
        this.#elProps = undefined;
    }
}
