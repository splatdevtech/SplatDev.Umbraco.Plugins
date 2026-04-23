export const UMB_CONTEXT_REQUEST_EVENT_TYPE = 'umb:context-request';
export const UMB_DEBUG_CONTEXT_EVENT_TYPE = 'umb:debug-contexts';
/**
 * @class UmbContextRequestEventImplementation
 * @augments {Event}
 * @implements {UmbContextRequestEvent}
 */
export class UmbContextRequestEventImplementation extends Event {
    constructor(contextAlias, apiAlias, callback, stopAtContextMatch = true) {
        super(UMB_CONTEXT_REQUEST_EVENT_TYPE, { bubbles: true, composed: true, cancelable: true });
        this.contextAlias = contextAlias;
        this.apiAlias = apiAlias;
        this.callback = callback;
        this.stopAtContextMatch = stopAtContextMatch;
    }
    clone() {
        return new UmbContextRequestEventImplementation(this.contextAlias, this.apiAlias, this.callback, this.stopAtContextMatch);
    }
}
export class UmbContextDebugRequest extends Event {
    constructor(callback) {
        super(UMB_DEBUG_CONTEXT_EVENT_TYPE, { bubbles: true, composed: true, cancelable: false });
        this.callback = callback;
    }
}
