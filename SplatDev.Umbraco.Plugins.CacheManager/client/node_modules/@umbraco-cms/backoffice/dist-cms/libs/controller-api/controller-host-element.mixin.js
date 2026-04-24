import { UmbControllerHostMixin } from './controller-host.mixin.js';
/**
 * This mixin enables a web-component to host controllers.
 * This enables controllers to be added to the life cycle of this element.
 * @param {object} superClass - superclass to be extended.
 * @mixin
 * @returns {UmbControllerHostElement} - The class that extends the superClass and implements the UmbControllerHostElement interface.
 */
export const UmbControllerHostElementMixin = (superClass) => {
    class UmbControllerHostElementClass extends UmbControllerHostMixin(superClass) {
        getHostElement() {
            return this;
        }
        connectedCallback() {
            super.connectedCallback?.();
            this.hostConnected();
        }
        disconnectedCallback() {
            super.disconnectedCallback?.();
            this.hostDisconnected();
        }
    }
    return UmbControllerHostElementClass;
};
