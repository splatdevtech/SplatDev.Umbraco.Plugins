import { createObservablePart } from '../utils/create-observable-part.function.js';
import { UmbBasicState } from './basic-state.js';
/**
 * @class UmbClassState
 * @augments {UmbBasicState<T>}
 * @description - This state can hold class instance which has a equal method to compare in coming instances for changes.
 */
export class UmbClassState extends UmbBasicState {
    constructor(initialData) {
        super(initialData);
    }
    /**
     * @function createObservablePart
     * @param {(mappable: UmbClassStateData | undefined) => unknown} mappingFunction - Method to return the part for this Observable to return.
     * @param {(previousResult: unknown, currentResult: unknown) => boolean} [memoizationFunction] - Method to Compare if the data has changed. Should return true when data is different.
     * @returns {Observable<unknown>} - an observable.
     * @description - Creates an Observable from this State.
     */
    asObservablePart(mappingFunction, memoizationFunction) {
        return createObservablePart(this._subject, mappingFunction, memoizationFunction);
    }
    /**
     * @function setValue
     * @param {UmbClassStateData | undefined} data - The next data for this state to hold.
     * @description - Set the data of this state, if data is different than current this will trigger observations to update.
     */
    setValue(data) {
        if (!this._subject)
            return;
        const oldValue = this._subject.getValue();
        if (data && oldValue?.equal(data))
            return;
        this._subject.next(data);
    }
}
