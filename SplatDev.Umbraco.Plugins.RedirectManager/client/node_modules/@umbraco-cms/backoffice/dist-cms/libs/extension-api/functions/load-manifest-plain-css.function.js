/**
 *
 * @param property
 */
export async function loadManifestPlainCss(property) {
    const propType = typeof property;
    if (propType === 'function') {
        // Promise function
        const result = await property();
        if (typeof result === 'object' && result != null) {
            const exportValue = ('css' in result ? result.css : undefined) || ('default' in result ? result.default : undefined);
            if (exportValue && typeof exportValue === 'string') {
                return exportValue;
            }
        }
    }
    else if (propType === 'string') {
        // Import string
        const result = await import(/* @vite-ignore */ property);
        if (typeof result === 'object' && result != null) {
            const exportValue = ('css' in result ? result.css : undefined) || ('default' in result ? result.default : undefined);
            if (exportValue && typeof exportValue === 'string') {
                return exportValue;
            }
        }
    }
    return undefined;
}
