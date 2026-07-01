angular.module("umbraco").controller("splatDev.IconList.Controller", function ($scope) {
    'use strict';
    const vm = this;
    $scope.iconList = null;
    $scope.direction = 'col-12'
    vm.$onInit = async function () {
        $scope.iconList = $scope.block.data.list.contentData;
        vm.directionClass();
    }
    vm.directionClass = function () {
        if ($scope.block.data.direction === 'Horizontal') $scope.direction = 'col-lg-4 col-md-4 col-sm-4';
        if ($scope.block.data.direction === 'Vertical') $scope.direction = 'col-12';
    }
});