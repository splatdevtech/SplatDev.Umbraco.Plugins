import { UmbClassMixin } from './class.mixin.js';
/**
 * This mixin enables a web-component to host controllers.
 * This enables controllers to be added to the life cycle of this element.
 *
 */
export class UmbControllerBase extends UmbClassMixin(EventTarget) {
}
