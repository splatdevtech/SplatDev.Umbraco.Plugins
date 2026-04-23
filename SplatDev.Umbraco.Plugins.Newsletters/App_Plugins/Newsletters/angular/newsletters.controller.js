(function () {
    "use strict";

    angular
        .module("umbraco")
        .controller("NewslettersDashboardController", NewslettersDashboardController);

    function NewslettersDashboardController($scope, $http, notificationsService) {
        var vm = this;
        var apiBase = "/umbraco/api/newsletters";

        vm.subscribers = [];
        vm.campaigns = [];
        vm.loading = false;
        vm.activeTab = "subscribers";

        vm.loadSubscribers = loadSubscribers;
        vm.loadCampaigns = loadCampaigns;
        vm.sendCampaign = sendCampaign;
        vm.setTab = setTab;

        init();

        function init() {
            loadSubscribers();
            loadCampaigns();
        }

        function setTab(tab) {
            vm.activeTab = tab;
        }

        function loadSubscribers() {
            vm.loading = true;
            $http.get(apiBase + "/subscribers")
                .then(function (response) {
                    vm.subscribers = response.data;
                })
                .catch(function () {
                    notificationsService.error("Newsletters", "Failed to load subscribers.");
                })
                .finally(function () {
                    vm.loading = false;
                });
        }

        function loadCampaigns() {
            $http.get(apiBase + "/campaigns")
                .then(function (response) {
                    vm.campaigns = response.data;
                })
                .catch(function () {
                    notificationsService.error("Newsletters", "Failed to load campaigns.");
                });
        }

        function sendCampaign(campaign) {
            if (!confirm("Send campaign '" + campaign.subject + "' to all confirmed subscribers?")) return;

            $http.post(apiBase + "/send", { campaignId: campaign.id })
                .then(function () {
                    notificationsService.success("Newsletters", "Campaign sent successfully.");
                    loadCampaigns();
                })
                .catch(function (response) {
                    var msg = (response.data && response.data.message) ? response.data.message : "Failed to send campaign.";
                    notificationsService.error("Newsletters", msg);
                });
        }
    }

})();
