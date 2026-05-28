angular.module("umbraco").controller("Backups.DashboardController", [
    "$http", "$scope", "$timeout", "notificationsService",
    function ($http, $scope, $timeout, notificationsService) {
        "use strict";
        var vm = this;
        var API = "/umbraco/api/backups";

        vm.loading = true;
        vm.creating = false;
        vm.restoring = false;
        vm.activeTab = "backups";
        vm.showCreateForm = false;
        vm.backups = [];
        vm.cloudProviders = [];

        vm.form = {
            name: "",
            includeMedia: true,
            scope: 7,
            compress: true,
            encrypt: false,
            encryptionKey: "",
            cloudProviderIds: []
        };

        vm.restoreForm = {
            backupName: "",
            scope: 7,
            overwriteExisting: false,
            decryptionKey: ""
        };

        vm.scheduleForm = {
            enabled: false,
            cronExpression: "0 2 * * *",
            scope: 7,
            compress: true,
            encrypt: false,
            encryptionKey: "",
            cloudProviderIds: [],
            keepLocal: true
        };

        vm.scopes = [
            { value: 1, label: "Content" },
            { value: 2, label: "Media" },
            { value: 4, label: "Database" },
            { value: 3, label: "Content + Media" },
            { value: 7, label: "Full (Content + Media + Database)" }
        ];

        vm.cronPresets = [
            { label: "Every day at 2 AM", value: "0 2 * * *" },
            { label: "Every 6 hours", value: "0 */6 * * *" },
            { label: "Every Sunday at midnight", value: "0 0 * * 0" },
            { label: "First of month at 3 AM", value: "0 3 1 * *" },
            { label: "Custom", value: "" }
        ];

        vm.historyFilter = "";
        vm.historySortField = "createdAt";
        vm.historySortReverse = true;
        vm.providerTestResults = {};

        function init() {
            loadProviders();
            loadBackups();
        }

        function loadProviders() {
            $http.get(API + "/GetCloudProviders").then(function (resp) {
                vm.cloudProviders = resp.data || [];
            }, function () {
                vm.cloudProviders = [];
            });
        }

        function loadBackups() {
            vm.loading = true;
            $http.get(API + "/GetAll").then(function (resp) {
                vm.backups = resp.data || [];
                vm.loading = false;
            }, function () {
                vm.loading = false;
                notificationsService.error("Backups", "Failed to load backup history.");
            });
        }

        vm.setTab = function (tab) {
            vm.activeTab = tab;
        };

        vm.formatSize = function (bytes) {
            if (!bytes || bytes === 0) return "0 B";
            var units = ["B", "KB", "MB", "GB", "TB"];
            var i = Math.floor(Math.log(bytes) / Math.log(1024));
            if (i >= units.length) i = units.length - 1;
            return (bytes / Math.pow(1024, i)).toFixed(i === 0 ? 0 : 1) + " " + units[i];
        };

        vm.getScopeLabel = function (value) {
            for (var i = 0; i < vm.scopes.length; i++) {
                if (vm.scopes[i].value === value) return vm.scopes[i].label;
            }
            return "Unknown";
        };

        vm.timeAgo = function (dateStr) {
            if (!dateStr) return "";
            var now = new Date();
            var date = new Date(dateStr);
            var diff = Math.floor((now - date) / 1000);
            if (diff < 60) return "just now";
            if (diff < 3600) return Math.floor(diff / 60) + "m ago";
            if (diff < 86400) return Math.floor(diff / 3600) + "h ago";
            if (diff < 604800) return Math.floor(diff / 86400) + "d ago";
            return date.toLocaleDateString();
        };

        vm.filteredBackups = function () {
            if (!vm.historyFilter) return vm.backups;
            var q = vm.historyFilter.toLowerCase();
            return vm.backups.filter(function (b) {
                return b.name.toLowerCase().indexOf(q) !== -1 ||
                    (b.extension && b.extension.toLowerCase().indexOf(q) !== -1);
            });
        };

        vm.sortHistory = function (field) {
            if (vm.historySortField === field) {
                vm.historySortReverse = !vm.historySortReverse;
            } else {
                vm.historySortField = field;
                vm.historySortReverse = false;
            }
        };

        vm.sortIcon = function (field) {
            if (vm.historySortField !== field) return "icon-navigation";
            return vm.historySortReverse ? "icon-navigation-down" : "icon-navigation-up";
        };

        vm.createBackup = function () {
            if (!vm.form.name) {
                notificationsService.warning("Backups", "Please enter a name for the backup.");
                return;
            }
            if (vm.form.encrypt && !vm.form.encryptionKey) {
                notificationsService.warning("Backups", "Please enter an encryption key.");
                return;
            }

            vm.creating = true;
            var request = {
                name: vm.form.name,
                includeMedia: vm.form.includeMedia,
                scope: vm.form.scope,
                compress: vm.form.compress,
                encrypt: vm.form.encrypt,
                encryptionKey: vm.form.encryptionKey,
                cloudProviderIds: vm.form.cloudProviderIds || []
            };

            $http.post(API + "/Create", request).then(function (resp) {
                vm.creating = false;
                vm.showCreateForm = false;
                vm.form.name = "";
                vm.form.encryptionKey = "";
                notificationsService.success("Backups", "Backup created successfully.");
                loadBackups();
            }, function (err) {
                vm.creating = false;
                var msg = (err.data && err.data.message) || "Failed to create backup.";
                notificationsService.error("Backups", msg);
            });
        };

        vm.deleteBackup = function (name) {
            if (!confirm("Delete backup '" + name + "'? This cannot be undone.")) return;
            $http.delete(API + "/Delete?name=" + encodeURIComponent(name)).then(function () {
                notificationsService.success("Backups", "Backup deleted.");
                loadBackups();
            }, function (err) {
                var msg = (err.data && err.data.message) || "Failed to delete backup.";
                notificationsService.error("Backups", msg);
            });
        };

        vm.restoreBackup = function () {
            if (!vm.restoreForm.backupName) {
                notificationsService.warning("Backups", "Please select a backup to restore.");
                return;
            }

            var confirmMsg = "Restore backup '" + vm.restoreForm.backupName + "'?";
            if (vm.restoreForm.overwriteExisting) {
                confirmMsg += "\n\nWARNING: This will overwrite existing content!";
            }
            if (!confirm(confirmMsg)) return;

            vm.restoring = true;
            var options = {
                scope: vm.restoreForm.scope,
                overwriteExisting: vm.restoreForm.overwriteExisting,
                decryptionKey: vm.restoreForm.decryptionKey
            };

            $http.post(
                API + "/Restore?backupPath=" + encodeURIComponent(vm.restoreForm.backupName),
                options
            ).then(function (resp) {
                vm.restoring = false;
                var result = resp.data;
                if (result.success) {
                    var msg = "Restore complete. Content: " + result.restoredContentCount +
                        ", Media: " + result.restoredMediaCount;
                    notificationsService.success("Backups", msg);
                } else {
                    var errors = (result.errors || []).join("; ");
                    notificationsService.warning("Backups", "Restore completed with errors: " + errors);
                }
                vm.restoreForm.backupName = "";
                vm.restoreForm.decryptionKey = "";
            }, function (err) {
                vm.restoring = false;
                var msg = (err.data && err.data.message) || "Restore failed.";
                notificationsService.error("Backups", msg);
            });
        };

        vm.testProvider = function (providerId) {
            vm.providerTestResults[providerId] = { testing: true };
            $http.post(API + "/TestProvider?providerId=" + encodeURIComponent(providerId)).then(function (resp) {
                vm.providerTestResults[providerId] = {
                    testing: false,
                    valid: resp.data.valid,
                    tested: true
                };
            }, function () {
                vm.providerTestResults[providerId] = {
                    testing: false,
                    valid: false,
                    tested: true
                };
            });
        };

        vm.selectCronPreset = function () {
            var sel = vm.scheduleForm.selectedPreset;
            if (sel) {
                vm.scheduleForm.cronExpression = sel;
            }
        };

        vm.refreshBackups = function () {
            loadBackups();
        };

        vm.totalSize = function () {
            var total = 0;
            for (var i = 0; i < vm.backups.length; i++) {
                total += vm.backups[i].sizeBytes || 0;
            }
            return total;
        };

        vm.toggleProvider = function (list, id) {
            var idx = list.indexOf(id);
            if (idx > -1) {
                list.splice(idx, 1);
            } else {
                list.push(id);
            }
        };

        vm.isBackupEncrypted = function (name) {
            for (var i = 0; i < vm.backups.length; i++) {
                if (vm.backups[i].name === name) return vm.backups[i].isEncrypted;
            }
            return false;
        };

        $scope.$on("$destroy", function () {});

        init();
    }
]);
