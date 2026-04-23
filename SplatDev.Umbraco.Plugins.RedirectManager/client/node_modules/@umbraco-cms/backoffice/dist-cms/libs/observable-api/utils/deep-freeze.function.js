/**
 * Deep freeze an object and all its properties.
 * @param {T} inObj - The object to deep freeze.
 * @returns {T} - The deep frozen object.
 * @description - Deep freezes an object and all its properties.
 * @example <caption>Example of deep freezing an object.</caption>
 * const myObject = {a: 1, b: {c: 2}};
 * const frozenObject = deepFreeze(myObject);
 * frozenObject.a = 3; // Throws an error.
 * frozenObject.b.c = 4; // Throws an error.
 */
export const deepFreeze = Object.freeze(function deepFreezeImpl(inObj) {
    if (inObj != null && typeof inObj === 'object') {
        Object.freeze(inObj);
        Object.getOwnPropertyNames(inObj)?.forEach(function (prop) {
            if (
            // eslint-disable-next-line no-prototype-builtins
            inObj.hasOwnProperty(prop) &&
                inObj[prop] != null &&
                typeof inObj[prop] === 'object' &&
                !Object.isFrozen(inObj[prop])) {
                deepFreeze(inObj[prop]);
            }
        });
    }
    return inObj;
});
