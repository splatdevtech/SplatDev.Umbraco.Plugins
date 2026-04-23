/**
 *
 * @param manifest
 */
export function isManifestElementNameType(manifest) {
    return typeof manifest === 'object' && manifest !== null && manifest.elementName !== undefined;
}
