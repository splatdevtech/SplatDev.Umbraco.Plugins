(function () {
    "use strict";

    angular.module("umbraco").controller("QuickPoll.DashboardController", [
        "$scope",
        "$http",
        function ($scope, $http) {
            var vm = this;
            vm.polls = [];
            vm.loading = false;
            vm.error = null;
            vm.newPoll = { question: "", options: ["", ""], isActive: true };

            var apiBase = "/umbraco/api/quickpoll";

            vm.load = function () {
                vm.loading = true;
                vm.error = null;
                $http.get(apiBase + "/getall")
                    .then(function (response) {
                        vm.polls = response.data;
                    })
                    .catch(function (err) {
                        vm.error = "Failed to load polls. " + (err.data || err.statusText);
                    })
                    .finally(function () {
                        vm.loading = false;
                    });
            };

            vm.deletePoll = function (id) {
                if (!confirm("Delete this poll and all its votes?")) return;
                $http.delete(apiBase + "/delete?id=" + id)
                    .then(function () {
                        vm.polls = vm.polls.filter(function (p) { return p.id !== id; });
                    })
                    .catch(function (err) {
                        vm.error = "Delete failed. " + (err.data || err.statusText);
                    });
            };

            vm.addOption = function () {
                vm.newPoll.options.push("");
            };

            vm.removeOption = function (index) {
                if (vm.newPoll.options.length > 2) {
                    vm.newPoll.options.splice(index, 1);
                }
            };

            vm.createPoll = function () {
                if (!vm.newPoll.question.trim()) {
                    vm.error = "Question is required.";
                    return;
                }
                var options = vm.newPoll.options.filter(function (o) { return o.trim(); });
                if (options.length < 2) {
                    vm.error = "At least 2 options are required.";
                    return;
                }

                var payload = {
                    question: vm.newPoll.question,
                    isActive: vm.newPoll.isActive,
                    options: options.map(function (text, idx) {
                        return { optionText: text, sortOrder: idx, voteCount: 0 };
                    })
                };

                $http.post(apiBase + "/create", payload)
                    .then(function (response) {
                        vm.polls.unshift(response.data);
                        vm.newPoll = { question: "", options: ["", ""], isActive: true };
                    })
                    .catch(function (err) {
                        vm.error = "Create failed. " + (err.data || err.statusText);
                    });
            };

            vm.load();
        }
    ]);
}());
