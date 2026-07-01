angular.module("umbraco").controller("splatDev.FeaturedList.Controller", function ($scope, $routeParams, contentResource, mediaResource) {
    'use strict';
    const vm = this;
    $scope.featuredList = null;
    vm.$onInit = async function () {
        $scope.featuredList = $scope.block.data;
        $scope.featuredList.arrayList = [];
        $scope.featuredList.arrayBoxes = [];
        let list = $scope.featuredList.list.split(',');
        for (let item of list) {
            let response = await contentResource.getById(item, $routeParams.mculture);
            let variant = response.variants[0];
            let img = variant.tabs.find(x => x.alias === 'content/details')?.properties.find(x => x.alias === 'displayImage').value[0].mediaKey;
            let icon = variant.tabs.find(x => x.alias === 'navigation')?.properties.find(x => x.alias === 'icon')?.value.split(' ');
            let featItem = {
                title: variant.tabs.find(x => x.alias === 'content').properties.find(x => x.alias === 'pageTitle').value,
                summary: variant.tabs.find(x => x.alias === 'content').properties.find(x => x.alias === 'pageSummary').value,
                img: img,
                icon: icon
            };
            $scope.featuredList.arrayList.push(featItem);
        }

        let boxes = $scope.block.data.boxes.contentData;
        for (let box of boxes) {
            let item = {
                title: box.title,
                text: box.text,
                icon: box.icon.split(' '),
                links: box.links
            };
            $scope.featuredList.arrayBoxes.push(item);
        }
    }
});