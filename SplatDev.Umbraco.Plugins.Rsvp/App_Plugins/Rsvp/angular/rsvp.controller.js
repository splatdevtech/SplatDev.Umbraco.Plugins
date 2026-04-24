(function () {
    "use strict";

    angular.module("umbraco").controller("Rsvp.DashboardController", [
        "$scope",
        "$http",
        function ($scope, $http) {
            var vm = this;
            vm.events = [];
            vm.selectedEvent = null;
            vm.attendees = [];
            vm.loading = false;
            vm.error = null;

            var apiBase = "/umbraco/api/rsvp";

            vm.load = function () {
                vm.loading = true;
                vm.error = null;
                $http.get(apiBase + "/getevents")
                    .then(function (response) {
                        vm.events = response.data;
                    })
                    .catch(function (err) {
                        vm.error = "Failed to load events. " + (err.data || err.statusText);
                    })
                    .finally(function () {
                        vm.loading = false;
                    });
            };

            vm.selectEvent = function (event) {
                vm.selectedEvent = event;
                vm.loadAttendees(event.id);
            };

            vm.loadAttendees = function (eventId) {
                $http.get(apiBase + "/getattendees?eventId=" + eventId)
                    .then(function (response) {
                        vm.attendees = response.data;
                    })
                    .catch(function (err) {
                        vm.error = "Failed to load attendees. " + (err.data || err.statusText);
                    });
            };

            vm.cancelRegistration = function (attendeeId) {
                if (!confirm("Cancel this registration?")) return;
                $http.post(apiBase + "/cancelregistration?attendeeId=" + attendeeId)
                    .then(function () {
                        vm.attendees = vm.attendees.filter(function (a) { return a.id !== attendeeId; });
                        vm.loadAttendees(vm.selectedEvent.id);
                    })
                    .catch(function (err) {
                        vm.error = "Cancel failed. " + (err.data || err.statusText);
                    });
            };

            vm.deleteEvent = function (id) {
                if (!confirm("Delete this event and all registrations?")) return;
                $http.delete(apiBase + "/deleteevent?id=" + id)
                    .then(function () {
                        vm.events = vm.events.filter(function (e) { return e.id !== id; });
                        if (vm.selectedEvent && vm.selectedEvent.id === id) {
                            vm.selectedEvent = null;
                            vm.attendees = [];
                        }
                    })
                    .catch(function (err) {
                        vm.error = "Delete failed. " + (err.data || err.statusText);
                    });
            };

            vm.statusLabel = function (status) {
                switch (status) {
                    case 0: return "Confirmed";
                    case 1: return "Waitlisted";
                    case 2: return "Cancelled";
                    default: return status;
                }
            };

            vm.load();
        }
    ]);
}());
