angular.module("umbraco").controller("splatDev.ButtonBorder.Controller", function ($scope) {
    const vm = this;
    vm.border = 'round' // default border;
    vm.borders = ['square', 'round', 'bullet'];

    vm.$onInit = async function () {
        await init();
    }

    async function init() {
        if ($scope.model.value !== undefined && $scope.model.value != '') {
            vm.border = $scope.model.value;
        }
    }

    vm.select = function (border) {
        $scope.model.value = border;
        vm.border = border;
    }

    vm.class = function (border) {
        let active = border === vm.border ? 'active' : '';
        return `btn-${border} ${active}`;
    }
});