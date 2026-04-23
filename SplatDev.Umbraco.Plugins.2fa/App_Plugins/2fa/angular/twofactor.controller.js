angular.module("umbraco").controller("TwoFactorDashboardController", [
    "$scope",
    "$http",
    function ($scope, $http) {
        $scope.loading = false;
        $scope.memberId = null;
        $scope.status = null;
        $scope.setupSecret = null;
        $scope.totpCode = "";
        $scope.backupCodes = [];
        $scope.message = null;

        $scope.checkStatus = function () {
            if (!$scope.memberId) return;
            $scope.loading = true;
            $http.get("/umbraco/api/twofactor/IsEnabled?memberId=" + $scope.memberId)
                .then(function (r) {
                    $scope.status = r.data.enabled;
                })
                .finally(function () { $scope.loading = false; });
        };

        $scope.setup = function () {
            $scope.loading = true;
            $http.post("/umbraco/api/twofactor/SetupTotp?memberId=" + $scope.memberId)
                .then(function (r) {
                    $scope.setupSecret = r.data.secretKey;
                    $scope.message = null;
                })
                .finally(function () { $scope.loading = false; });
        };

        $scope.verify = function () {
            $scope.loading = true;
            $http.post("/umbraco/api/twofactor/VerifyTotp?memberId=" + $scope.memberId + "&code=" + $scope.totpCode)
                .then(function (r) {
                    if (r.data.valid) {
                        $scope.message = { type: "success", text: "2FA enabled successfully!" };
                        $scope.status = true;
                        $scope.setupSecret = null;
                    } else {
                        $scope.message = { type: "error", text: "Invalid code. Please try again." };
                    }
                })
                .finally(function () { $scope.loading = false; });
        };

        $scope.generateBackupCodes = function () {
            $scope.loading = true;
            $http.post("/umbraco/api/twofactor/GenerateBackupCodes?memberId=" + $scope.memberId + "&count=8")
                .then(function (r) {
                    $scope.backupCodes = r.data.codes || [];
                    $scope.message = { type: "success", text: "Backup codes generated. Save them securely!" };
                })
                .finally(function () { $scope.loading = false; });
        };

        $scope.disable = function () {
            if (!confirm("Disable 2FA for this member?")) return;
            $scope.loading = true;
            $http.post("/umbraco/api/twofactor/Disable?memberId=" + $scope.memberId)
                .then(function () {
                    $scope.status = false;
                    $scope.setupSecret = null;
                    $scope.backupCodes = [];
                    $scope.message = { type: "success", text: "2FA disabled." };
                })
                .finally(function () { $scope.loading = false; });
        };
    }
]);
