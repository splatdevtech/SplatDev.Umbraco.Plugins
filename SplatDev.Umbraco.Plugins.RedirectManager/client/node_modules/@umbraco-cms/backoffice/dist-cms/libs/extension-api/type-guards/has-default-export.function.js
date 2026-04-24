/**
 *
 * @param object
 */
export function hasDefaultExport(object) {
    return typeof object === 'object' && object !== null && 'default' in object;
}
