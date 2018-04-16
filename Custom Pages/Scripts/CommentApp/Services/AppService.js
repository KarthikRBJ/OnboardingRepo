(function () {

    var appService = function ($http) {


        var getTheme = function () {
            return $http.get("http://192.168.0.148/Relativity/CustomPages/b9c4168d-a204-4478-b15b-89110f7abebb/Home/getTheme")
                  .then(function (response) {
                      return response.data;
                  });
        };

        var changeTheme = function (theme) {
            return $http.get("http://192.168.0.148/Relativity/CustomPages/b9c4168d-a204-4478-b15b-89110f7abebb/Home/changeTheme/?value=" +theme)
                  .then(function (response) {
                      return response.data;
                  });
        };

        return {
            getTheme: getTheme,
            changeTheme: changeTheme
        };

    };


    var module = angular.module("commentApp");
    module.factory("appService", appService);

}());