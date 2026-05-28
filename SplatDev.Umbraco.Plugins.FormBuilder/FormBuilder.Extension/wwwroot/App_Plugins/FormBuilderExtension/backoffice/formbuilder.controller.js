angular.module("umbraco").controller("SplatDev.FormBuilder.Controller", [
  "$scope", "$http", "$q",
  function ($scope, $http, $q) {
    var api = "/umbraco/api/formbuilder/";

    $scope.forms = [];
    $scope.loading = false;
    $scope.activeView = "list";
    $scope.currentForm = null;
    $scope.fieldTypes = [];

    $scope.loadForms = function () {
      $scope.loading = true;
      $http.get(api + "GetAllForms").then(function (res) {
        $scope.forms = res.data;
      })["finally"](function () { $scope.loading = false; });
    };

    $scope.loadFieldTypes = function () {
      $http.get(api + "GetFieldTypes").then(function (res) {
        $scope.fieldTypes = res.data;
      });
    };

    $scope.createForm = function () {
      var name = prompt("Form name:");
      if (!name) return;
      $http.post(api + "CreateForm", { name: name }).then(function () {
        $scope.loadForms();
      });
    };

    $scope.editForm = function (form) {
      $scope.activeView = "editor";
      $http.get(api + "GetForm?id=" + form.id).then(function (res) {
        $scope.currentForm = res.data;
        $scope.loadFieldTypes();
      });
    };

    $scope.saveForm = function () {
      if (!$scope.currentForm) return;
      var body = {
        name: $scope.currentForm.name,
        category: $scope.currentForm.category || "",
        fields: ($scope.currentForm.fields || []).map(function (f) {
          return {
            alias: f.alias,
            label: f.label,
            type: f.type,
            required: f.isRequired,
            placeholder: f.placeholder || null,
            regex: f.regex || null,
            minLength: f.minLength || null,
            sortOrder: f.sortOrder
          };
        })
      };
      $http.put(api + "UpdateForm?id=" + $scope.currentForm.id, body).then(function () {
        alert("Saved.");
        $scope.backToList();
      });
    };

    $scope.deleteForm = function (form) {
      if (!confirm("Delete " + form.name + "?")) return;
      $http["delete"](api + "DeleteForm?id=" + form.id).then(function () {
        $scope.loadForms();
      }, function (err) {
        alert("Delete failed: " + err.data);
      });
    };

    $scope.duplicateForm = function (form) {
      if (!confirm("Duplicate " + form.name + "?")) return;
      $http.post(api + "DuplicateForm?id=" + form.id).then(function () {
        $scope.loadForms();
      });
    };

    $scope.addField = function () {
      if (!$scope.currentForm) return;
      var alias = prompt("Field alias (camelCase):");
      if (!alias) return;
      $scope.currentForm.fields = $scope.currentForm.fields || [];
      $scope.currentForm.fields.push({
        alias: alias,
        label: alias,
        type: "text",
        isRequired: false,
        placeholder: "",
        regex: "",
        minLength: 0,
        sortOrder: $scope.currentForm.fields.length
      });
    };

    $scope.removeField = function (idx) {
      $scope.currentForm.fields.splice(idx, 1);
    };

    $scope.backToList = function () {
      $scope.activeView = "list";
      $scope.currentForm = null;
      $scope.loadForms();
    };

    $scope.loadForms();
    $scope.loadFieldTypes();
  }
]);
