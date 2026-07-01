angular.module("umbraco.resources").factory("addressResources", function ($http, $q) {
    const baseApiUrl = "/umbraco/backoffice/api/GlobalSettingsApi";
    function getSettings() {
        return $http.get(`${baseApiUrl}/Get`);
    }
    function getGeolocation(address) {
        let geocoder = new google.maps.Geocoder();
        let deferred = $q.defer()
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status === google.maps.GeocoderStatus.OK) {
                let selected = results[0];
                let geoLoc = {
                    latitude: selected.geometry.location.lat(),
                    longitude: selected.geometry.location.lng()
                }
                deferred.resolve(geoLoc);
            }
        });
        return deferred.promise
    }
    function getAddressSuggestions(address) {
        let geocoder = new google.maps.Geocoder();
        let deferred = $q.defer()
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status === google.maps.GeocoderStatus.OK) {
                deferred.resolve(results);
            }
            //else { deferred.reject(`Address not found: ${address}`) }
        });
        return deferred.promise
    }
    return {
        getSettings: getSettings,
        getGeolocation: getGeolocation,
        getAddressSuggestions: getAddressSuggestions
    }
})