/**
 *
 * @param target
 * @param source
 */
export function assignToFrozenObject(target, source) {
    return Object.assign(Object.create(Object.getPrototypeOf(target)), target, source);
}
