import { UmbExtensionElementAndApiInitializer } from './extension-element-and-api-initializer.controller.js';
import { UmbBaseExtensionsInitializer, } from './base-extensions-initializer.controller.js';
/**
 */
export class UmbExtensionsElementAndApiInitializer extends UmbBaseExtensionsInitializer {
    //
    #extensionRegistry;
    #defaultElement;
    #defaultApi;
    #constructorArgs;
    #elProps;
    #apiProps;
    get elementProperties() {
        return this.#elProps;
    }
    set elementProperties(props) {
        this.#elProps = props;
        this._extensions.forEach((controller) => {
            controller.elementProps = props;
        });
    }
    get apiProperties() {
        return this.#apiProps;
    }
    set apiProperties(props) {
        this.#apiProps = props;
        this._extensions.forEach((controller) => {
            controller.apiProps = props;
        });
    }
    constructor(host, extensionRegistry, type, constructorArguments, filter, onChange, controllerAlias, defaultElement, defaultApi, args) {
        super(host, extensionRegistry, type, filter, onChange, controllerAlias, args);
        this.#extensionRegistry = extensionRegistry;
        this.#constructorArgs = constructorArguments;
        this.#defaultElement = defaultElement;
        this.#defaultApi = defaultApi;
        this._init();
    }
    _createController(manifest) {
        const extController = new UmbExtensionElementAndApiInitializer(this, this.#extensionRegistry, manifest.alias, this.#constructorArgs, this._extensionChanged, this.#defaultElement, this.#defaultApi);
        extController.elementProps = this.#elProps;
        extController.apiProps = this.#apiProps;
        return extController;
    }
    destroy() {
        super.destroy();
        this.#constructorArgs = undefined;
        this.#elProps = undefined;
        this.#apiProps = undefined;
        this.#extensionRegistry = undefined;
    }
}
