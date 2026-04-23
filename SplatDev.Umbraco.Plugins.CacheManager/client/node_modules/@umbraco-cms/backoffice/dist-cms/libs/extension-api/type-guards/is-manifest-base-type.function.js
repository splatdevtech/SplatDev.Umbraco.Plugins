/**
 *
 * @param x
 */
export function isManifestBaseType(x) {
    return typeof x === 'object' && x !== null && 'alias' in x;
}
