import { UmbContextConsumer } from './context-consumer.js';
export class UmbContextConsumerController extends UmbContextConsumer {
    #controllerAlias = Symbol();
    #host;
    get controllerAlias() {
        return this.#controllerAlias;
    }
    constructor(host, contextAlias, callback) {
        super(() => host.getHostElement(), contextAlias, callback);
        this.#host = host;
        host.addUmbController(this);
    }
    destroy() {
        this.#host?.removeUmbController(this);
        this.#host = undefined;
        super.destroy();
    }
}
