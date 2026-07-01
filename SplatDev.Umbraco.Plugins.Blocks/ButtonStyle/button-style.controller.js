angular.module("umbraco").controller("splatDev.ButtonStyle.Controller", function ($scope) {
    const vm = this;
    vm.style = 'outline' // default style;
    vm.styles = ['outline', 'primary', 'secondary', 'alert', 'success'];

    vm.$onInit = async function () {
        await init();
    }

    async function init() {
        if ($scope.model.value !== undefined && $scope.model.value != '') {
            vm.style = $scope.model.value;
        }
    }
    vm.select = function (style) {
        $scope.model.value = style;
        vm.style = style;
    }
    vm.class = function (style) {
        let active = style === vm.style ? 'active' : '';
        return `btn-${style} ${active}`;
    }
});