import { createObservablePart, strictEqualityMemoization } from '../utils/index.js';
import { UmbBasicState } from './basic-state.js';
/**
 * @class UmbStringState
 * @augments {UmbBasicState<T>}
 * @description - A state holding string data, this ensures the data is unique, not updating any Observes unless there is an actual change of the value, by using `===`.
 */
export class UmbStringState extends UmbBasicState {
    constructor(initialData) {
        super(initialData);
    }
    asObservablePart(mappingFunction, memoizationFunction) {
        return createObservablePart(this._subject, mappingFunction, memoizationFunction ?? strictEqualityMemoization);
    }
}
