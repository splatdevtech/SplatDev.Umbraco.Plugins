angular.module('umbraco').controller('CacheWarmerController', function (cacheResources, notificationsService) {
    const vm = this;
    vm.loading = false;
    vm.latestTask = [];
    vm.notFound = [];
    vm.stats = null;

    vm.clearCache = function () {
        vm.loading = true;
        cacheResources.clearCache().then(function () {
            notificationsService.success("Success", "Cache cleared");
            vm.loading = false;
        }, function () {
            notificationsService.error("Error", "Failed to clear cache");
            vm.loading = false;
        });
    }

    vm.refreshCache = function () {
        vm.loading = true;
        cacheResources.refreshCache().then(function () {
            notificationsService.success("Success", "Cache refreshed");
            vm.loading = false;
        }, function () {
            notificationsService.error("Error", "Failed to refresh cache");
            vm.loading = false;
        });
    }

    vm.getLatestTask = function () {
        vm.loading = true;
        cacheResources.getLatestTask().then(function (results) {
            vm.latestTask = results.data;
            vm.loading = false;
        });
    }

    vm.getNotFound = function () {
        vm.loading = true;
        cacheResources.getNotFound().then(function (results) {
            vm.notFound = results.data;
            vm.loading = false;
        });
    }

    vm.getStats = function () {
        vm.loading = true;
        cacheResources.getStats().then(function (results) {
            vm.stats = results.data;
            vm.loading = false;
        });
    }

    vm.refreshLogs = function () {
        vm.getLatestTask();
        vm.getNotFound();
        vm.getStats();
    }

    vm.getLatestTask();
    vm.getNotFound();
    vm.getStats();
});