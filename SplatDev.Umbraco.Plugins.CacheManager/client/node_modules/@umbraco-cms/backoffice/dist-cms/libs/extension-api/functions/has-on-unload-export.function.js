/**
 * Validate if an ESModule has exported a function called `onUnload`
 * @param obj
 */
export function hasOnUnloadExport(obj) {
    return obj !== null && typeof obj === 'object' && 'onUnload' in obj;
}
