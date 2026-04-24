import { UmbExtensionElementInitializer } from './extension-element-initializer.controller.js';
import { UmbBaseExtensionsInitializer, } from './base-extensions-initializer.controller.js';
/**
 */
export class UmbExtensionsElementInitializer extends UmbBaseExtensionsInitializer {
    //
    #extensionRegistry;
    #defaultElement;
    #props;
    get properties() {
        return this.#props;
    }
    set properties(props) {
        this.#props = props;
        this._extensions.forEach((controller) => {
            controller.properties = props;
        });
    }
    constructor(host, extensionRegistry, type, filter, onChange, controllerAlias, defaultElement, args) {
        super(host, extensionRegistry, type, filter, onChange, controllerAlias, args);
        this.#extensionRegistry = extensionRegistry;
        this.#defaultElement = defaultElement;
        this._init();
    }
    _createController(manifest) {
        const extController = new UmbExtensionElementInitializer(this, this.#extensionRegistry, manifest.alias, this._extensionChanged, this.#defaultElement);
        extController.properties = this.#props;
        return extController;
    }
    destroy() {
        super.destroy();
        this.#props = undefined;
        this.#extensionRegistry = undefined;
    }
}
