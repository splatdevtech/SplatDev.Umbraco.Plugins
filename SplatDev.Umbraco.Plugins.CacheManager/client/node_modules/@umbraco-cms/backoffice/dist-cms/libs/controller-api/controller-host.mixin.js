/**
 * This mixin enables a class to host controllers.
 * This enables controllers to be added to the life cycle of this element.
 * @param {object} superClass - superclass to be extended.
 * @mixin
 * @returns {UmbControllerHost} - A class which extends the given superclass.
 */
export const UmbControllerHostMixin = (superClass) => {
    class UmbControllerHostBaseClass extends superClass {
        #controllers = [];
        #attached = false;
        getHostElement() {
            return undefined;
        }
        /**
         * Tests if a controller is assigned to this element.
         * @param {UmbController} ctrl - The controller to check for.
         * @returns {boolean} - true if the controller is assigned
         */
        hasUmbController(ctrl) {
            return this.#controllers.indexOf(ctrl) !== -1;
        }
        /**
         * Retrieve controllers matching a filter of this element.
         * @param {Function} filterMethod - filter method
         * @returns {Array<UmbController>} - currently assigned controllers passing the filter method.
         */
        getUmbControllers(filterMethod) {
            return this.#controllers.filter(filterMethod);
        }
        /**
         * Append a controller to this element.
         * @param {UmbController} ctrl - the controller to append to this host.
         */
        addUmbController(ctrl) {
            // If this specific class is already added, then skip out.
            if (this.#controllers.indexOf(ctrl) !== -1) {
                return;
            }
            // Check if there is one already with same unique
            this.removeUmbControllerByAlias(ctrl.controllerAlias);
            this.#controllers.push(ctrl);
            if (this.#attached) {
                // If a controller is created on a already attached element, then it will be added directly. This might not be optimal. As the controller it self has not finished its constructor method jet. therefor i postpone the call: [NL]
                Promise.resolve().then(() => {
                    // Extra check to see if we are still attached and still added at this point:
                    if (this.#attached && this.#controllers.includes(ctrl)) {
                        ctrl.hostConnected();
                    }
                });
            }
        }
        /**
         * Remove a controller from this element.
         * Notice this will also destroy the controller.
         * @param {UmbController} ctrl - The controller to remove and destroy from this host.
         */
        removeUmbController(ctrl) {
            const index = this.#controllers.indexOf(ctrl);
            if (index !== -1) {
                this.#controllers.splice(index, 1);
                if (this.#attached) {
                    ctrl.hostDisconnected();
                }
                ctrl.destroy();
            }
        }
        /**
         * Remove a controller from this element by its alias.
         * Notice this will also destroy the controller.
         * @param {string | symbol} controllerAlias
         */
        removeUmbControllerByAlias(controllerAlias) {
            if (controllerAlias) {
                this.#controllers.forEach((x) => {
                    if (x.controllerAlias === controllerAlias) {
                        this.removeUmbController(x);
                    }
                });
            }
        }
        hostConnected() {
            this.#attached = true;
            // Note: this might not be optimal, as if hostDisconnected remove one of the controllers, then the next controller will be skipped.
            this.#controllers.forEach((ctrl) => ctrl.hostConnected());
        }
        hostDisconnected() {
            this.#attached = false;
            // Note: this might not be optimal, as if hostDisconnected remove one of the controllers, then the next controller will be skipped.
            this.#controllers.forEach((ctrl) => ctrl.hostDisconnected());
        }
        destroy() {
            let ctrl;
            let prev = null;
            // Note: A very important way of doing this loop, as foreach will skip over the next item if the current item is removed.
            while ((ctrl = this.#controllers[0])) {
                ctrl.destroy();
                // Help developer realize that they made a mistake in code:
                if (ctrl === prev) {
                    throw new Error(`Controller with controller alias: '${ctrl.controllerAlias?.toString()}' and class name: '${ctrl.constructor.name}', does not remove it self when destroyed. This can cause memory leaks. Please fix this issue.\r\nThis usually occurs when you have a destroy() method that doesn't call super.destroy().`);
                }
                prev = ctrl;
            }
            this.#controllers.length = 0;
            this.#attached = false;
        }
    }
    return UmbControllerHostBaseClass;
};
