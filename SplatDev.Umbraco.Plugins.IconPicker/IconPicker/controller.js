angular.module("umbraco").controller("splatdev.IconPicker.Controller", ['$scope', '$http', function ($scope, $http) {
    'use strict';
    //`${icon.familyStylesByLicense.free[0].style} ${icon.id}`;
    let vm = this;
    let version = '6.3.0';
    vm.icons = [];
    vm.expanded = false;
    vm.loading = true;
    vm.selected = '';
    vm.renderIcon = [];
    vm.limit = 23;
    vm.undo = [];
    const query = `{
                        release(version: "6.x") {
                        icons(license: "free") {
                            id
                            label
                            familyStylesByLicense{
                            free {
                                style
                            }
                            }
                            shim {
                            id
                            }
                        }
                        }
                    }
                `;

    function getIcons() {
        return $http({
            method: 'POST',
            url: 'https://api.fontawesome.com',
            data: JSON.stringify({ query })
        });
    }

    vm.$onInit = function () {
        let data;
        if ($scope.model.value !== undefined) {
            vm.selected = $scope.model.value;
            vm.undo[0] = vm.selected.split(' ');
            vm.renderIcon = vm.selected.split(' ');
        }
        getIcons().then((response) => {
            data = response.data.data;
            vm.icons = data.release.icons;

            getSelected();

            //console.log(`FontAwesome Icon Picker: Lisf of free icons for version \'${version}\' loaded.`)
            vm.loading = false;
        })
    }

    vm.loadMore = function () {
        vm.limit += 24
    }

    vm.select = function (icon) {
        vm.selected = `${icon.familyStylesByLicense.free[0].style} ${icon.id}`;
        getSelected();
        $scope.model.value = vm.selected;
    }

    vm.undo = function () {
        vm.selected = `${vm.undo[0][0]} ${vm.undo[0][1]}`;
        getSelected();
        $scope.model.value = vm.selected;
    }

    vm.clear = function () {
        vm.renderIcon = [];
        vm.selected = '';
        $scope.model.value = vm.selected;
    }

    function getSelected() {
        if (vm.selected !== '') {
            let icon = vm.selected.split(' ');
            let iconSelection = vm.icons.find(x => x.selected);
            if (iconSelection) iconSelection.selected = false;
            let selected = vm.icons.indexOf(vm.icons.find(x => x.id === icon[1]));
            if (selected !== -1) {
                vm.icons[selected].selected = true;
                vm.renderIcon = icon;
            }
        }
    }
}]);