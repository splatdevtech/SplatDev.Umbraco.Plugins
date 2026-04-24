/**
 *
 * @param object
 */
export function hasApiExport(object) {
    return typeof object === 'object' && object !== null && 'api' in object;
}
