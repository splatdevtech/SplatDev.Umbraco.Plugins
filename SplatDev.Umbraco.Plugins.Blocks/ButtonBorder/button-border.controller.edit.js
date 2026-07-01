angular.module("umbraco").controller("GoogleAddressEditorController", function ($scope, $routeParams, addressResources) {
    const vm = this;
    vm.apiKey = '';
    vm.gMapsUrl = 'https://maps.googleapis.com/maps/api/js?key=';
    vm.loading = true;
    vm.location = null;
    vm.locationSelected = false;

    vm.$onInit = async function () {
        await init();
    }

    vm.$onDestroy = function () {
    }

    vm.destroy = function () {
        let obj = document.getElementById('google-maps-address');
        let script = document.getElementById('google-maps-script');
        if (script !== null)
            obj.removeChild(script);
    }

    vm.getGeolocation = async function (address) {
        vm.loading = !vm.loading;

        vm.data = await addressResources.getGeolocation(address);
        vm.loading = !vm.loading;
        return vm.data;

    }

    vm.addGoogleMapsScript = async function (url) {
        let obj = document.getElementById('google-maps-address');
        if (document.getElementById('google-maps-script') === null) {
            let node = document.createElement('script');
            node.src = url;
            node.type = 'text/javascript';
            node.id = 'google-maps-script';
            obj.append(node);
        }
    }

    vm.save = async function () {
        vm.destroy();
        $scope.model.submit(vm.location);
    }

    vm.close = function () {
        if ($scope.model.close) {
            $scope.model.close();
        }
    }
    function initMap() {
        var location = new google.maps.LatLng(vm.location.geo.lat, vm.location.geo.lon);

        var options = {
            zoom: 17,
            center: location
        }
        const map = new google.maps.Map(document.getElementById("map"), options);
        addMarker(location, map);
    }
    function addMarker(location, map) {
        marker = new google.maps.Marker({
            position: location,
            map: map
        })
    }
    async function init() {
        let response = await addressResources.getSettings()
        vm.apiKey = response.data.GoogleApiKey
        vm.gMapsUrl = `${vm.gMapsUrl}${vm.apiKey}&libraries=places`
        vm.addGoogleMapsScript(vm.gMapsUrl).then(() => {
            setTimeout(() => {
                if ($scope.model.location !== '' && $scope.model.location !== undefined) {
                    vm.location = $scope.model.location;
                    initMap();
                    vm.locationSelected = true;
                }
                $scope.initAutoComplete();
                vm.loading = false;
                $scope.$apply();
            }, 500)
        })
    }

    $scope.initAutoComplete = function () {
        const options = {
            componentRestrictions: { country: ["us"] },
            fields: ["address_components", "place_id", "geometry", "name"],
            types: ["address"],
        };
        const address1 = document.getElementById('address1');
        address1.focus();
        let autoComplete = new google.maps.places.Autocomplete(address1, options);
        google.maps.event.addListener(autoComplete, 'place_changed', function () {
            let place = autoComplete.getPlace();
            vm.location = place;
            vm.location.geo = {
                lat: place.geometry.location.lat(),
                lon: place.geometry.location.lng()
            };
            vm.location.addressLine1 = '';
            for (const component of place.address_components) {
                const componentType = component.types[0];

                switch (componentType) {
                    case "street_number":
                        vm.location.addressLine1 = `${component.long_name}`;
                        break;
                    case "route":
                        vm.location.addressLine1 += component.short_name;
                        break;
                    case "postal_code":
                        vm.location.zip = component.long_name;
                        break;
                    case "postal_code_suffix":
                        vm.location.zip = `${vm.location.zip}-${component.long_name}`;
                        break;
                    case "administrative_area_level_2":
                        vm.location.county = component.long_name;
                        break;
                    case "locality":
                        vm.location.city = component.long_name;
                        break;
                    case "administrative_area_level_1":
                        vm.location.state = component.short_name;
                        break;
                    case "country":
                        vm.location.country = component.long_name;
                        break;
                }
            }
            vm.locationSelected = true;
            initMap();
            $scope.$apply();
        });
    }
});