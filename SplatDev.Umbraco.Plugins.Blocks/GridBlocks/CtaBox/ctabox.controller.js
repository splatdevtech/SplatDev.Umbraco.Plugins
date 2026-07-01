angular.module("umbraco").controller("splatDev.CtaBox.Controller", function ($scope) {
    'use strict';
    const vm = this;
    vm.buttonCss = '';
    vm.settings = null;

    vm.toLower = function (text) {
        return text !== undefined ? text.toLowerCase() : '';
    }
    vm.$onInit = function () {
        let block = $scope.block.data;
        let position = block.buttonPosition.split(' ');

        vm.settings = block;
        vm.button = block.button[0];
        vm.button.css = `${position[0]}-${position[1]} btn-${block.borders} btn-${block.style}`;
    }
});