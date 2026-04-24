/**
 *
 * @param object
 */
export function hasElementExport(object) {
    return typeof object === 'object' && object !== null && 'element' in object;
}
