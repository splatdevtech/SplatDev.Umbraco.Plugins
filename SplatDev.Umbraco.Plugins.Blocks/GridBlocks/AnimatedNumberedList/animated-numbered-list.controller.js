angular.module("umbraco").controller("splatDev.AnimatedNumberedList.Controller", function ($scope) {
    const vm = this;
    vm.settings = null;

    vm.toLower = function (text) {
        if (text === undefined) return text;
        return text.toLowerCase();
    }
    vm.$onInit = function () {
        let block = $scope.block.data;

        vm.settings = block;
    }
});