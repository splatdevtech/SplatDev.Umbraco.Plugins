'use strict';
//based off https://proto.io/freebies/onoff/
angular.module('umbraco')
    .controller('OnOffButtonController',
        function ($scope) {
            var vm = this;

            vm.OnText = $scope.model.config.onText;
            vm.OffText = $scope.model.config.offText;

            if ($scope.model.value === null || $scope.model.value === '') {
                $scope.model.value = $scope.model.config.checked;
            }

            $scope.model.value = $scope.model.value === 'True';
            vm.checked = $scope.model.value;

            vm.change = function () {
                $scope.model.value = !$scope.model.value;
                vm.checked = $scope.model.value;
            }
        });