/**
 * @function classEqualMemoization
 * @param {UmbClassStateData | undefined} previousValue - The previous value to compare.
 * @param {UmbClassStateData | undefined} currentValue - The current value to compare.
 * @returns {boolean} - Returns true if the values are identical.
 * @description - Compares the two values using strict equality.
 */
export function classEqualMemoization(previousValue, currentValue) {
    return ((previousValue && currentValue && previousValue.equal(currentValue)) ||
        (previousValue === undefined && currentValue === undefined));
}
