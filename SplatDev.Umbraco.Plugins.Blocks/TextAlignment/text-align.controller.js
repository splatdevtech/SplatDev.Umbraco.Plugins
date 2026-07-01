angular.module("umbraco").controller("splatDev.TextAlign.Controller", function ($scope) {
    const vm = this;
    vm.align = 'left' // default alignment;
    vm.alignments = ['left', 'center', 'right', 'justified'];

    vm.$onInit = async function () {
        await init();
    }

    async function init() {
        if ($scope.model.value !== undefined && $scope.model.value != '') {
            vm.align = $scope.model.value;
        }
    }

    vm.select = function (align) {
        vm.align = align;
        $scope.model.value = align;
    }

});