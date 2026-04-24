export class UmbObserver {
    #source;
    #callback;
    #subscription;
    constructor(source, callback) {
        this.#source = source;
        if (callback) {
            this.#callback = callback;
            this.#subscription = source.subscribe(callback);
        }
    }
    /**
     * provides a promise which is resolved ones the observer got a value that is not undefined.
     * Notice this promise will resolve immediately if the Observable holds an empty array or empty string.
     *
     */
    asPromise() {
        // Notice, we do not want to store and reuse the Promise, cause this promise guarantees that the value is not undefined when resolved. and reusing the promise would not ensure that.
        return new Promise((resolve) => {
            let initialCallback = true;
            let wantedToClose = false;
            const subscription = this.#source.subscribe((value) => {
                if (value !== undefined) {
                    if (initialCallback) {
                        wantedToClose = true;
                    }
                    else {
                        subscription.unsubscribe();
                        if (!this.#callback) {
                            this.destroy();
                        }
                    }
                    resolve(value);
                }
            });
            initialCallback = false;
            if (wantedToClose) {
                subscription.unsubscribe();
                if (!this.#callback) {
                    this.destroy();
                }
            }
        });
    }
    hostConnected() {
        // Notice: This will not re-subscribe if this controller was destroyed. Only if the subscription was closed.
        if (this.#subscription?.closed && this.#callback) {
            this.#subscription = this.#source.subscribe(this.#callback);
        }
    }
    hostDisconnected() {
        // No cause then it cant re-connect, if the same element just was moved in DOM. [NL]
        // I do not agree with my self anymore ^^. I think we should unsubscribe here, to help garbage collector and prevent unforeseen side effects of observations continuing while element are out of the DOM. [NL]
        this.#subscription?.unsubscribe();
    }
    destroy() {
        if (this.#subscription) {
            this.#subscription.unsubscribe();
            this.#callback = undefined;
            this.#subscription = undefined;
        }
        this.#source = undefined;
    }
}
