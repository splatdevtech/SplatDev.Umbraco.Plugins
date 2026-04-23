(function () {
    "use strict";

    function ExifController($http) {
        var vm = this;
        vm.mediaKey = "";
        vm.filePath = "";
        vm.data = null;
        vm.error = null;

        var baseUrl = "/umbraco/api/exif/";

        vm.lookupByKey = function () {
            vm.data = null;
            vm.error = null;
            $http.get(baseUrl + "GetByMediaKey?mediaKey=" + encodeURIComponent(vm.mediaKey))
                .then(function (r) { vm.data = r.data; })
                .catch(function (e) { vm.error = e.data || "Not found."; });
        };

        vm.lookupByPath = function () {
            vm.data = null;
            vm.error = null;
            $http.get(baseUrl + "GetByFilePath?filePath=" + encodeURIComponent(vm.filePath))
                .then(function (r) { vm.data = r.data; })
                .catch(function (e) { vm.error = e.data || "Not found."; });
        };
    }

    angular.module("umbraco").controller("ExifController", ExifController);
}());
