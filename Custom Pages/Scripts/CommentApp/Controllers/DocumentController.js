(function () {


    var app = angular.module('commentApp');

    var documentController = function ($scope, documentService) {

        var onDocumentsLoad = function (data) {
            $scope.documents = data;
        }

        var onError = function (reason) {
            $scope.error = reason;
            console.log($scope.error);
        }
        documentService.getDocuments().then(onDocumentsLoad, onError);
    };


    app.controller("documentController", ["$scope", "documentService", documentController])
}());