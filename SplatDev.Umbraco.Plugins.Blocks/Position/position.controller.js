angular.module("umbraco").controller("splatDev.Position.Controller", function ($scope) {
    const vm = this;
    vm.position = ['top', 'left'] // default position;
    vm.positions = [['top', 'left'], ['top', 'center'], ['top', 'right'],
                    ['middle', 'left'], ['middle', 'center'], ['middle', 'right'],
                    ['bottom', 'left'], ['bottom', 'center'], ['bottom', 'right']];

    vm.$onInit = async function () {
        await init();
    }

    async function init() {
        if ($scope.model.value !== undefined && $scope.model.value != '') {
            populate($scope.model.value);
        }
    }

    vm.setPosition = function (pos) {
        let position = `${pos[0]} ${pos[1]}`;
        $scope.model.value = position;
        vm.position = pos;
    }

    vm.isPositionActive = function (pos) {
        return vm.position[0] === pos[0] && vm.position[1] === pos[1];
    }

    function populate(position) {
        let pos = position.split(' ');
        vm.position = pos;
    }
});