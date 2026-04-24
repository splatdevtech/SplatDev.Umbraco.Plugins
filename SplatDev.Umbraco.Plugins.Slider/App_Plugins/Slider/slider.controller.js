(function () {
    'use strict';

    angular.module('umbraco').controller('Slider.DashboardController', [
        '$http',
        function ($http) {
            var vm = this;
            vm.loading = true;
            vm.sliders = [];

            function init() {
                $http.get('/umbraco/api/slider/GetSliders')
                    .then(function (response) {
                        vm.sliders = response.data;
                    })
                    .finally(function () {
                        vm.loading = false;
                    });
            }

            init();
        }
    ]);
})();
