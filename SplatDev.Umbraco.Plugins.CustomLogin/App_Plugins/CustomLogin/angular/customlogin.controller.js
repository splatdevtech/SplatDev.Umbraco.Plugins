angular.module("umbraco").controller("CustomLoginDashboardController", [
    "$scope",
    "$http",
    function ($scope, $http) {
        var apiBase = "/umbraco/api/customlogin";
        var dayNames = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];
        var dayNamesEs = ["Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado"];

        $scope.loading = true;
        $scope.saving = false;
        $scope.message = null;
        $scope.dayNames = dayNames;
        $scope.dayNamesEs = dayNamesEs;
        $scope.activeTab = "branding";

        $scope.settings = {
            brandName: "",
            logoUrl: "",
            logoAlternativeUrl: "",
            backgroundImageUrl: "",
            supportEmail: "",
            backgroundColor: "",
            primaryColor: "",
            textColor: "",
            curvesColor: "",
            showCurves: true,
            showImagePanel: true,
            imageBorderRadius: "",
            contentBackground: "",
            contentWidth: "",
            contentHeight: "",
            contentBorderRadius: "",
            alignItems: "",
            headerFontSize: "",
            headerFontSizeLarge: "",
            headerSecondaryFontSize: "",
            buttonBorderRadius: "",
            allowPasswordReset: true,
            enableSso: false,
            greetings: ["", "", "", "", "", "", ""],
            greetingsEs: ["", "", "", "", "", "", ""],
            timeoutBackgroundImageUrl: "",
            timeoutInstructionText: "",
            timeoutInstructionTextEs: "",
            customCss: ""
        };

        $scope.tabs = [
            { key: "branding", label: "Branding" },
            { key: "colors", label: "Colors" },
            { key: "layout", label: "Layout" },
            { key: "typography", label: "Typography" },
            { key: "greetings", label: "Greetings" },
            { key: "timeout", label: "Timeout" },
            { key: "security", label: "Security" },
            { key: "css", label: "Custom CSS" }
        ];

        $scope.setTab = function (tab) {
            $scope.activeTab = tab;
        };

        $scope.load = function () {
            $scope.loading = true;
            $http.get(apiBase + "/GetSettings")
                .then(function (response) {
                    var data = response.data || {};
                    angular.extend($scope.settings, data);
                    if (!Array.isArray($scope.settings.greetings) || $scope.settings.greetings.length < 7) {
                        $scope.settings.greetings = ["", "", "", "", "", "", ""];
                    }
                    if (!Array.isArray($scope.settings.greetingsEs) || $scope.settings.greetingsEs.length < 7) {
                        $scope.settings.greetingsEs = ["", "", "", "", "", "", ""];
                    }
                })
                .finally(function () {
                    $scope.loading = false;
                });
        };

        $scope.save = function () {
            $scope.saving = true;
            $scope.message = null;
            $http.post(apiBase + "/SaveSettings", $scope.settings)
                .then(function () {
                    $scope.message = { type: "success", text: "Settings saved. CSS and localization files regenerated." };
                })
                .catch(function () {
                    $scope.message = { type: "error", text: "Failed to save settings." };
                })
                .finally(function () {
                    $scope.saving = false;
                });
        };

        $scope.load();
    }
]);
