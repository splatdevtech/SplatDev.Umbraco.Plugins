import { createExtensionElement } from '../functions/create-extension-element.function.js';
import { UmbBaseExtensionInitializer } from './base-extension-initializer.controller.js';
/**
 * This Controller manages a single Extension and its Element.
 * When the extension is permitted to be used, its Element will be instantiated and available for the consumer.
 * @example
 * ```ts
 * const controller = new UmbExtensionElementController(host, extensionRegistry, alias, (permitted, ctrl) => { console.log("Extension is permitted and this is the element: ", ctrl.component) }));
 * ```
 * @class UmbExtensionElementController
 */
export class UmbExtensionElementInitializer extends UmbBaseExtensionInitializer {
    #defaultElement;
    #component;
    /**
     * The component that is created for this extension.
     * @readonly
     * @type {(HTMLElement | undefined)}
     */
    get component() {
        return this.#component;
    }
    /**
     * The props that are passed to the component.
     * @type {Record<string, any>}
     * @memberof UmbElementExtensionController
     * @example
     * ```ts
     * const controller = new UmbElementExtensionController(host, extensionRegistry, alias, onPermissionChanged);
     * controller.props = { foo: 'bar' };
     * ```
     * Is equivalent to:
     * ```ts
     * controller.component.foo = 'bar';
     * ```
     */
    #properties;
    get properties() {
        return this.#properties;
    }
    set properties(newVal) {
        this.#properties = newVal;
        // TODO: we could optimize this so we only re-set the changed props.
        this.#assignProperties();
    }
    constructor(host, extensionRegistry, alias, onPermissionChanged, defaultElement) {
        super(host, extensionRegistry, 'extElement_', alias, onPermissionChanged);
        this.#defaultElement = defaultElement;
        this._init();
    }
    #assignProperties = () => {
        if (!this.#component || !this.#properties)
            return;
        // TODO: we could optimize this so we only re-set the updated props.
        Object.keys(this.#properties).forEach((key) => {
            this.#component[key] = this.#properties[key];
        });
    };
    async _conditionsAreGood() {
        const manifest = this.manifest; // In this case we are sure its not undefined.
        const newComponent = await createExtensionElement(manifest, this.#defaultElement);
        if (!this._isConditionsPositive) {
            // We are not positive anymore, so we will back out of this creation.
            return false;
        }
        this.#component = newComponent;
        if (this.#component) {
            this.#assignProperties();
            this.#component.manifest = manifest;
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
    }
    destroy() {
        super.destroy();
        this.#properties = undefined;
    }
}
