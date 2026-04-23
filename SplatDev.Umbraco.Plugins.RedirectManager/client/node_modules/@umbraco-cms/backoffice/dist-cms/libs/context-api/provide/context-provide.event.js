export const UMB_CONTEXT_PROVIDE_EVENT_TYPE = 'umb:context-provide';
/**
 * @class UmbContextProvideEventImplementation
 * @augments {Event}
 * @implements {UmbContextProvideEvent}
 */
export class UmbContextProvideEventImplementation extends Event {
    constructor(contextAlias) {
        super(UMB_CONTEXT_PROVIDE_EVENT_TYPE, { bubbles: true, composed: true });
        this.contextAlias = contextAlias;
    }
    clone() {
        return new UmbContextProvideEventImplementation(this.contextAlias);
    }
}
export const isUmbContextProvideEventType = (event) => {
    return event.type === UMB_CONTEXT_PROVIDE_EVENT_TYPE;
};
export const UMB_CONTEXT_UNPROVIDED_EVENT_TYPE = 'umb:context-unprovided';
/**
 * @class UmbContextUnprovidedEventImplementation
 * @augments {Event}
 * @implements {UmbContextUnprovidedEvent}
 */
export class UmbContextUnprovidedEventImplementation extends Event {
    constructor(contextAlias, instance) {
        super(UMB_CONTEXT_UNPROVIDED_EVENT_TYPE, { bubbles: true, composed: true });
        this.contextAlias = contextAlias;
        this.instance = instance;
    }
}
export const isUmbContextUnprovidedEventType = (event) => {
    return event.type === UMB_CONTEXT_UNPROVIDED_EVENT_TYPE;
};
