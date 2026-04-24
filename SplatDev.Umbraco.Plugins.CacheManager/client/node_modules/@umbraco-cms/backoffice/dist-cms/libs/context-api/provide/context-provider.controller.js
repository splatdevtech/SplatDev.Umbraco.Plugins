import { UmbContextProvider } from './context-provider.js';
export class UmbContextProviderController extends UmbContextProvider {
    #host;
    #controllerAlias;
    get controllerAlias() {
        return this.#controllerAlias;
    }
    constructor(host, contextAlias, instance) {
        super(host.getHostElement(), contextAlias, instance);
        this.#host = host;
        // Makes the controllerAlias unique for this instance, this enables multiple Contexts to be provided under the same name. (This only makes sense cause of Context Token Discriminators)
        // This does mean that if someone provides a context with the same name, but with a different instance, it will not override the previous instance. But its good since it enables extensions to provide contexts at the same scope of other contexts.
        this.#controllerAlias = contextAlias.toString() + '_' + instance.constructor?.name;
        // If this API is already provided with this alias? Then we do not want to register this controller:
        const existingControllers = host.getUmbControllers((x) => x.controllerAlias === this.controllerAlias);
        if (existingControllers.length > 0 &&
            existingControllers[0].providerInstance?.() === instance) {
            // This just an additional awareness feature to make devs Aware, the alternative would be adding it anyway, but that would destroy existing controller of this alias.
            // Back out, this instance is already provided, by another controller.
            throw new Error(`Context API: The context of '${this.controllerAlias}' and instance '${instance.constructor?.name ?? 'unnamed'}' is already provided by another Context Provider Controller.`);
        }
        else {
            host.addUmbController(this);
        }
    }
    destroy() {
        if (this.#host) {
            const host = this.#host;
            this.#host = undefined;
            host.removeUmbController(this);
        }
        super.destroy();
    }
}
