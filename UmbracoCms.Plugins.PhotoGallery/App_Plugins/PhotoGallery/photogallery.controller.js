(function () {
    'use strict';

    angular.module('umbraco').controller('PhotoGallery.DashboardController', [
        '$http',
        function ($http) {
            var vm = this;
            vm.loading = true;
            vm.albums = [];

            function init() {
                $http.get('/umbraco/api/photogallery/GetAlbums')
                    .then(function (response) {
                        vm.albums = response.data;
                    })
                    .finally(function () {
                        vm.loading = false;
                    });
            }

            init();
        }
    ]);
})();
