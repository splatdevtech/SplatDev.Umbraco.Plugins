import { UmbControllerBase } from './controller-base.class.js';
/**
 * This base provides the necessary for a class to become a context-api controller.
 *
 */
export class UmbContextBase extends UmbControllerBase {
    constructor(host, contextToken) {
        super(host, contextToken.toString());
        this.provideContext(contextToken, this);
    }
}
