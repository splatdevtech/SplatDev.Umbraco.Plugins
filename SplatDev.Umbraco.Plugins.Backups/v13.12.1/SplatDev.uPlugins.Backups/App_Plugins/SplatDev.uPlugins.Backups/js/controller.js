angular.module("umbraco").controller("splatDev.SiteBackup.Controller", function ($scope, $http, notificationsService, localizationService) {
    'use strict';
    const baseUrl = '/umbraco/backoffice/api/Backup/';
    const vm = this;
    vm.model = {
        details: null,
        props: [],
        performedDb: null,
        performedFiles: null,
        dbKeys: [],
        filesKeys: [],
        busy: false,
        showDetails: false
    };

    let notifications = {
        successDb: '',
        successFiles: '',
        success: '',
        failure: '',
        deleted: ''
    }

    vm.deleteBackup = (key) => {
        if (confirm("Are you sure you want to delete this archive? This cannot be undone!")) {
            $http.delete(`${baseUrl}DeleteBackup?filename=${key}`).then(() => {
                notificationsService.success(notifications.deleted);
                getPerformed();
            })
        }
    }

    vm.backupAll = () => {
        vm.model.busy = true;
        $http.post(`${baseUrl}FullBackup`).then(response => {
            if (response.data) {
                notificationsService.success(notifications.success);
                getPerformed();
            }
            else {
                console.log(response);
                notificationsService.error(notifications.failure);
            }
            vm.model.busy = false;
        })
    }

    vm.backupDb = () => {
        vm.model.busy = true;
        $http.post(`${baseUrl}DatabaseBackup`).then(response => {
            if (response.data) {
                notificationsService.success(notifications.successDb);
                getPerformed();
            }
            else {
                console.log(response)
                notificationsService.error(notifications.failure);
            }
            vm.model.busy = false;
        })
    }

    vm.backupFiles = () => {
        vm.model.busy = true;
        $http.post(`${baseUrl}FilesBackup`).then(response => {
            if (response.data) {
                notificationsService.success(notifications.successFiles);
                getPerformed();
            }
            else {
                console.log(response);
                notificationsService.error(notifications.failure);
            }
            vm.model.busy = false;
        })
    }

    vm.getBackupFilename = (key, array) => {
        return array[`${key}`];
    }

    function getBackupDetails() {
        $http.get(`${baseUrl}GetBackupDetails`).then(response => {
            vm.model.details = response.data;
            vm.model.props = Object.keys(vm.model.details)
        })
    }

    function getPerformed() {
        $http.get(`${baseUrl}GetBackupsPerformed`).then(response => {
            vm.model.performedDb = response.data.databaseBackups;
            vm.model.performedFiles = response.data.fileBackups;
            if (vm.model.performedDb != undefined && vm.model.performedDb != null) {
                vm.model.dbKeys = Object.keys(vm.model.performedDb);
            }
            if (vm.model.performedFiles != undefined && vm.model.performedFiles != null) {
                vm.model.filesKeys = Object.keys(vm.model.performedFiles);
            }
        })
    }

    async function init() {
        getBackupDetails();
        getPerformed();
        notifications.success = await localizationService.localize('notifications_success')
        notifications.successDb = await localizationService.localize('notifications_successdb')
        notifications.successFiles = await localizationService.localize('notifications_successfiles')
        notifications.failure = await localizationService.localize('notifications_failure')
        notifications.deleted = await localizationService.localize('notifications_deleted')
    }
    init();
});