import { UMB_CONTEXT_REQUEST_EVENT_TYPE } from '../consume/context-request.event.js';
import { UmbContextProvideEventImplementation } from './context-provide.event.js';
/**
 * @class UmbContextBoundary
 */
export class UmbContextBoundary {
    #eventTarget;
    #contextAlias;
    /**
     * Creates an instance of UmbContextBoundary.
     * @param {EventTarget} eventTarget - the host element for this context provider
     * @param {string | UmbContextToken} contextIdentifier - a string or token to identify the context
     * @param {*} instance - the instance to provide
     * @memberof UmbContextBoundary
     */
    constructor(eventTarget, contextIdentifier) {
        this.#eventTarget = eventTarget;
        const idSplit = contextIdentifier.toString().split('#');
        this.#contextAlias = idSplit[0];
        this.#eventTarget.addEventListener(UMB_CONTEXT_REQUEST_EVENT_TYPE, this.#handleContextRequest);
    }
    /**
     * @private
     * @param {UmbContextRequestEvent} event - the event to handle
     * @memberof UmbContextBoundary
     */
    #handleContextRequest = ((event) => {
        if (event.contextAlias !== this.#contextAlias)
            return;
        if (event.stopAtContextMatch) {
            // Since the alias matches, we will stop it from bubbling further up. But we still allow it to ask the other Contexts of the element. Hence not calling `event.stopImmediatePropagation();`
            event.stopPropagation();
        }
    });
    /**
     * @memberof UmbContextBoundary
     */
    hostConnected() {
        this.#eventTarget.dispatchEvent(new UmbContextProvideEventImplementation(this.#contextAlias));
    }
    /**
     * @memberof UmbContextBoundary
     */
    hostDisconnected() { }
    destroy() {
        this.#eventTarget = undefined;
    }
}
