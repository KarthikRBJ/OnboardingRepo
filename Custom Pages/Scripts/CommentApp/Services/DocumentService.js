(function () {

    var documentService = function ($http) {


        var getDocuments = function () {
            return $http.get("http://192.168.0.148/Relativity/CustomPages/b9c4168d-a204-4478-b15b-89110f7abebb/Home/DocumentList")
                  .then(function (response) {
                      return response.data;
                  });
        };

        return {
            getDocuments: getDocuments
        };

    };


    var module = angular.module("commentApp");
    module.factory("documentService", documentService);

}());