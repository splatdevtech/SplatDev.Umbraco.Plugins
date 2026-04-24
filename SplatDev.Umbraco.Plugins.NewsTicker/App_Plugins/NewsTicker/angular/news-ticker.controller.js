(function () {
    "use strict";

    angular
        .module("umbraco")
        .controller("NewsTickerDashboardController", NewsTickerDashboardController);

    function NewsTickerDashboardController($scope, $http, notificationsService) {
        var vm = this;
        var apiBase = "/umbraco/api/newsticker";

        vm.items = [];
        vm.settings = {};
        vm.loading = false;
        vm.newItem = { text: "", url: "", isActive: true, sortOrder: 0 };

        vm.loadItems = loadItems;
        vm.addItem = addItem;
        vm.toggleItem = toggleItem;
        vm.deleteItem = deleteItem;

        init();

        function init() {
            loadItems();
            loadSettings();
        }

        function loadItems() {
            vm.loading = true;
            $http.get(apiBase + "/items")
                .then(function (response) {
                    vm.items = response.data;
                })
                .catch(function () {
                    notificationsService.error("News Ticker", "Failed to load ticker items.");
                })
                .finally(function () {
                    vm.loading = false;
                });
        }

        function loadSettings() {
            $http.get(apiBase + "/settings")
                .then(function (response) {
                    vm.settings = response.data;
                });
        }

        function addItem() {
            if (!vm.newItem.text) {
                notificationsService.warning("News Ticker", "Item text is required.");
                return;
            }

            $http.post(apiBase + "/items", vm.newItem)
                .then(function () {
                    notificationsService.success("News Ticker", "Item added.");
                    vm.newItem = { text: "", url: "", isActive: true, sortOrder: 0 };
                    loadItems();
                })
                .catch(function () {
                    notificationsService.error("News Ticker", "Failed to add item.");
                });
        }

        function toggleItem(item) {
            item.isActive = !item.isActive;
            $http.put(apiBase + "/items/" + item.id, item)
                .then(function () {
                    notificationsService.success("News Ticker", "Item updated.");
                })
                .catch(function () {
                    item.isActive = !item.isActive; // revert
                    notificationsService.error("News Ticker", "Failed to update item.");
                });
        }

        function deleteItem(item) {
            if (!confirm("Delete this ticker item?")) return;

            $http.delete(apiBase + "/items/" + item.id)
                .then(function () {
                    notificationsService.success("News Ticker", "Item deleted.");
                    loadItems();
                })
                .catch(function () {
                    notificationsService.error("News Ticker", "Failed to delete item.");
                });
        }
    }

})();
