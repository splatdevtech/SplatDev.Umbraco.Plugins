(function () {
    "use strict";

    angular.module("umbraco").controller("Surveys.DashboardController", [
        "$scope",
        "$http",
        function ($scope, $http) {
            var vm = this;
            vm.surveys = [];
            vm.loading = false;
            vm.error = null;

            var apiBase = "/umbraco/api/surveys";

            vm.load = function () {
                vm.loading = true;
                vm.error = null;
                $http.get(apiBase + "/getall")
                    .then(function (response) {
                        vm.surveys = response.data;
                    })
                    .catch(function (err) {
                        vm.error = "Failed to load surveys. " + (err.data || err.statusText);
                    })
                    .finally(function () {
                        vm.loading = false;
                    });
            };

            vm.deleteSurvey = function (id) {
                if (!confirm("Delete this survey and all its responses?")) return;
                $http.delete(apiBase + "/delete?id=" + id)
                    .then(function () {
                        vm.surveys = vm.surveys.filter(function (s) { return s.id !== id; });
                    })
                    .catch(function (err) {
                        vm.error = "Delete failed. " + (err.data || err.statusText);
                    });
            };

            vm.publishToggle = function (survey) {
                survey.isPublished = !survey.isPublished;
                $http.put(apiBase + "/update?id=" + survey.id, survey)
                    .catch(function (err) {
                        survey.isPublished = !survey.isPublished; // revert
                        vm.error = "Update failed. " + (err.data || err.statusText);
                    });
            };

            // Initial load
            vm.load();
        }
    ]);
}());
