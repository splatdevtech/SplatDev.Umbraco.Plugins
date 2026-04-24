export class UmbControllerEvent extends Event {
    constructor(type) {
        super(type, { bubbles: false, composed: false, cancelable: false });
    }
}
