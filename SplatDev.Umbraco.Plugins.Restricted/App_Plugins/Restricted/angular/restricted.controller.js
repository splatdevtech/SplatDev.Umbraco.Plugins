angular.module("umbraco").controller("RestrictedDashboardController", [
    "$scope",
    "$http",
    function ($scope, $http) {
        $scope.loading = true;
        $scope.restrictedNodes = [];
        $scope.nodeId = "";
        $scope.loginPageNodeId = "";
        $scope.errorPageNodeId = "";
        $scope.memberGroups = "";
        $scope.statusMessage = "";
        $scope.errorMessage = "";

        $scope.loadRestrictedNodes = function () {
            $scope.loading = true;
            $http.get("/umbraco/api/restricted/GetRestrictedNodes")
                .then(function (response) {
                    $scope.restrictedNodes = response.data || [];
                })
                .catch(function () {
                    $scope.errorMessage = "Failed to load restricted nodes.";
                })
                .finally(function () {
                    $scope.loading = false;
                });
        };

        $scope.restrictNode = function () {
            if (!$scope.nodeId) return;
            var groups = $scope.memberGroups.split(",").map(function (g) { return g.trim(); }).filter(function (g) { return g; });
            var payload = {
                nodeId: parseInt($scope.nodeId, 10),
                loginPageNodeId: $scope.loginPageNodeId,
                errorPageNodeId: $scope.errorPageNodeId,
                memberGroups: groups
            };
            $http.post("/umbraco/api/restricted/RestrictNode", payload)
                .then(function (response) {
                    $scope.statusMessage = response.data.message;
                    $scope.loadRestrictedNodes();
                })
                .catch(function () {
                    $scope.errorMessage = "Failed to restrict node.";
                });
        };

        $scope.unrestrictNode = function (nodeId) {
            $http.delete("/umbraco/api/restricted/UnrestrictNode?nodeId=" + nodeId)
                .then(function () {
                    $scope.statusMessage = "Node " + nodeId + " unrestricted.";
                    $scope.loadRestrictedNodes();
                })
                .catch(function () {
                    $scope.errorMessage = "Failed to unrestrict node " + nodeId + ".";
                });
        };

        // Init
        $scope.loadRestrictedNodes();
    }
]);
