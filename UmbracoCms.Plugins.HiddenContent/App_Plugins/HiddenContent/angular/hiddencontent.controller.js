angular.module("umbraco").controller("HiddenContentDashboardController", [
    "$scope",
    "$http",
    function ($scope, $http) {
        $scope.loading = true;
        $scope.hiddenNodes = [];
        $scope.checkNodeId = "";
        $scope.checkResult = null;
        $scope.bulkIds = "";
        $scope.statusMessage = "";
        $scope.errorMessage = "";

        $scope.loadHiddenNodes = function () {
            $scope.loading = true;
            $http.get("/umbraco/api/hiddencontent/GetHiddenNodes")
                .then(function (response) {
                    $scope.hiddenNodes = response.data || [];
                })
                .catch(function () {
                    $scope.errorMessage = "Failed to load hidden nodes.";
                })
                .finally(function () {
                    $scope.loading = false;
                });
        };

        $scope.hideNode = function (nodeId) {
            $http.post("/umbraco/api/hiddencontent/HideNode?nodeId=" + nodeId)
                .then(function (response) {
                    $scope.statusMessage = response.data.message;
                    $scope.loadHiddenNodes();
                })
                .catch(function () { $scope.errorMessage = "Failed to hide node."; });
        };

        $scope.showNode = function (nodeId) {
            $http.post("/umbraco/api/hiddencontent/ShowNode?nodeId=" + nodeId)
                .then(function (response) {
                    $scope.statusMessage = response.data.message;
                    $scope.loadHiddenNodes();
                })
                .catch(function () { $scope.errorMessage = "Failed to show node."; });
        };

        $scope.checkNode = function () {
            if (!$scope.checkNodeId) return;
            $http.get("/umbraco/api/hiddencontent/IsHidden?nodeId=" + $scope.checkNodeId)
                .then(function (response) {
                    $scope.checkResult = response.data;
                })
                .catch(function () { $scope.checkResult = null; });
        };

        $scope.bulkHide = function () {
            var ids = $scope.bulkIds.split(",").map(function (s) { return parseInt(s.trim(), 10); }).filter(function (n) { return !isNaN(n); });
            if (!ids.length) return;
            $http.post("/umbraco/api/hiddencontent/BulkHide", { nodeIds: ids })
                .then(function (response) {
                    $scope.statusMessage = response.data.message;
                    $scope.loadHiddenNodes();
                })
                .catch(function () { $scope.errorMessage = "Bulk hide failed."; });
        };

        $scope.bulkShow = function () {
            var ids = $scope.bulkIds.split(",").map(function (s) { return parseInt(s.trim(), 10); }).filter(function (n) { return !isNaN(n); });
            if (!ids.length) return;
            $http.post("/umbraco/api/hiddencontent/BulkShow", { nodeIds: ids })
                .then(function (response) {
                    $scope.statusMessage = response.data.message;
                    $scope.loadHiddenNodes();
                })
                .catch(function () { $scope.errorMessage = "Bulk show failed."; });
        };

        // Init
        $scope.loadHiddenNodes();
    }
]);
