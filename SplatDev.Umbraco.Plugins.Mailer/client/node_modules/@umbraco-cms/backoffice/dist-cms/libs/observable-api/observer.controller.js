import { UmbObserver } from './observer.js';
export class UmbObserverController extends UmbObserver {
    #host;
    #alias;
    get controllerAlias() {
        return this.#alias;
    }
    constructor(host, source, callback, alias) {
        super(source, callback);
        this.#host = host;
        this.#alias = alias;
        // Lets check if controller is already here:
        // No we don't want this, as multiple different controllers might be looking at the same source.
        /*
        if (this._subscriptions.has(source)) {
            const subscription = this._subscriptions.get(source);
            subscription?.unsubscribe();
        }
        */
        host.addUmbController(this);
    }
    destroy() {
        this.#host?.removeUmbController(this);
        this.#host = undefined;
        super.destroy();
    }
}
