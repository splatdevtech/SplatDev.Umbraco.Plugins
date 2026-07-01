angular.module("umbraco").controller("splatDev.ServiceList.Controller", function ($scope, $routeParams, contentResource) {
    const vm = this;
    $scope.services = null;

    function lowerCaseFirstLetter(str) {
        return (str.charAt(0).toLowerCase() + str.slice(1)).replaceAll(' ', '');
    }
    async function load() {
        var list = $scope.block.data.services.split(',');
        $scope.services = [];
        for (let item of list) {
            let response = await contentResource.getById(item, $routeParams.mculture);
            let variant = response.variants[0];
            let delay = variant.tabs.find(x => x.alias === 'animations').properties.find(x => x.alias === 'delay').value;
            let service = {
                class: lowerCaseFirstLetter(variant.tabs.find(x => x.alias === 'animations').properties.find(x => x.alias === 'entrance').value[0]),
                style: `height: 141px; animation-delay: ${delay}ms;`,
                delay: delay,
                title: variant.name,
                summary: variant.tabs.find(x => x.alias === 'content').properties.find(x => x.alias === 'pageSummary').value,
                icon: variant.tabs.find(x => x.alias === 'content/details').properties.find(x => x.alias === 'icon').value.split(' ')
            };
            $scope.services.push(service);
        }
    }
    vm.$onInit = async function () {
        await load();
    }
});