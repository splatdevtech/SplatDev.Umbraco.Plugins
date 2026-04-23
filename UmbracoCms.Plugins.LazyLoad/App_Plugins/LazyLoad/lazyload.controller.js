(function () {
    'use strict';

    angular.module('umbraco').controller('LazyLoad.DashboardController', [
        '$http', 'notificationsService',
        function ($http, notificationsService) {
            var vm = this;
            vm.loading = true;
            vm.settings = {};

            function init() {
                $http.get('/umbraco/api/lazyload/GetSettings')
                    .then(function (response) {
                        vm.settings = response.data;
                    })
                    .finally(function () {
                        vm.loading = false;
                    });
            }

            vm.save = function () {
                $http.post('/umbraco/api/lazyload/SaveSettings', vm.settings)
                    .then(function () {
                        notificationsService.success('Saved', 'Lazy Load settings saved successfully.');
                    });
            };

            init();
        }
    ]);
})();
