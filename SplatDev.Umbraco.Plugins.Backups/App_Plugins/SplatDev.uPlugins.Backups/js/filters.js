angular.module('umbraco')
    .filter('formatDate', function () {
        return function (input) {
            const date = new Date(input);
            const year = date.toLocaleString("en-US", { year: "numeric", timeZone: "UTC" });

            return `${date.getMonth() + 1}/${date.getDay() + 1}/${year} ${("0" + date.getHours()).slice(-2)}:${("0" + date.getMinutes()).slice(-2)}:${("0" + date.getSeconds()).slice(-2) }`;
        };
    })

angular.module('umbraco')
    .filter('beautify', function () {
        return function (input) {
            return input.replace(/([A-Z])/g, ' $1').trim();
        };
    })

angular.module('umbraco')
    .filter('isTypeOf', function () {
        return function (input, key, type) {
            return typeof array[key] === type
        };
    })

angular.module('umbraco')
    .filter('isArray', function () {
        return function (input) {
            return Array.isArray(input);
        };
    })

angular.module('umbraco')
    .filter('isSensitive', function () {
        return function (prop) {
            if (prop === undefined) return false;
            if (prop.includes('Key') || prop.includes('Id') || prop.includes('Secret')) return true;
            return false;
        };
    })