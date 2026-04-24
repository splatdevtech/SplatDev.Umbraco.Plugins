/**
 * Validate if an ESModule export has a function called 'onInit'
 * @param obj
 */
export function hasInitExport(obj) {
    return obj !== null && typeof obj === 'object' && 'onInit' in obj;
}
