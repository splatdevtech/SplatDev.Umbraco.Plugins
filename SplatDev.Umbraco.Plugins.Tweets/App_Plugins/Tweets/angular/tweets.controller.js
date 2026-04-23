(function () {
    "use strict";

    angular
        .module("umbraco")
        .controller("TweetsDashboardController", TweetsDashboardController);

    function TweetsDashboardController($scope, $http, notificationsService) {
        var vm = this;
        var apiBase = "/umbraco/api/tweets";

        vm.tweets = [];
        vm.loading = false;
        vm.refreshing = false;

        vm.loadTweets = loadTweets;
        vm.refreshCache = refreshCache;

        init();

        function init() {
            loadTweets();
        }

        function loadTweets() {
            vm.loading = true;
            $http.get(apiBase + "/feed")
                .then(function (response) {
                    vm.tweets = response.data;
                })
                .catch(function () {
                    notificationsService.error("Tweets", "Failed to load cached tweets.");
                })
                .finally(function () {
                    vm.loading = false;
                });
        }

        function refreshCache() {
            vm.refreshing = true;
            $http.post(apiBase + "/refresh", {})
                .then(function (response) {
                    var count = response.data && response.data.count ? response.data.count : 0;
                    notificationsService.success("Tweets", "Cache refreshed. " + count + " tweet(s) loaded.");
                    loadTweets();
                })
                .catch(function (response) {
                    var msg = (response.data && response.data.error) ? response.data.error : "Failed to refresh tweet cache.";
                    notificationsService.error("Tweets", msg);
                })
                .finally(function () {
                    vm.refreshing = false;
                });
        }
    }

})();
