(function () {
    "use strict";

    function ExamineExtensionsController($http) {
        var vm = this;

        vm.loading = true;
        vm.indexes = [];
        vm.selectedIndex = null;
        vm.rebuildIndex = null;
        vm.query = "";
        vm.pageSize = 20;
        vm.results = null;
        vm.rebuildMessage = null;

        var baseUrl = "/umbraco/api/examineextensions/";

        function init() {
            $http.get(baseUrl + "GetIndexes").then(function (response) {
                vm.indexes = response.data;
                if (vm.indexes.length > 0) {
                    vm.selectedIndex = vm.indexes[0];
                    vm.rebuildIndex = vm.indexes[0];
                }
                vm.loading = false;
            }).catch(function () {
                vm.loading = false;
            });
        }

        vm.search = function () {
            if (!vm.query) return;
            $http.post(baseUrl + "Search", {
                query: vm.query,
                indexName: vm.selectedIndex,
                page: 1,
                pageSize: vm.pageSize
            }).then(function (response) {
                vm.results = response.data;
            });
        };

        vm.rebuild = function () {
            vm.rebuildMessage = null;
            $http.post(baseUrl + "RebuildIndex", JSON.stringify(vm.rebuildIndex), {
                headers: { "Content-Type": "application/json" }
            }).then(function (response) {
                vm.rebuildMessage = response.data.message;
            });
        };

        init();
    }

    angular.module("umbraco").controller("ExamineExtensionsController", ExamineExtensionsController);
}());
