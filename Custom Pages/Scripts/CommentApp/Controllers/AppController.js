(function () {


    var app = angular.module('commentApp');

    var appController = function ($scope, appService, documentService) {

        var onThemeLoad = function (data) {
            $scope.theme = data;
            console.log($scope.theme);
            $scope.setTheme($scope.theme.value);
        }

        var onError = function (reason) {

            $scope.error = "Error loading the theme";
        }

        $scope.changeTheme = function (theme) {

            appService.changeTheme(theme).then($scope.setTheme(theme), onError);
        }

        $scope.setTheme = function (value) {
            if (value) {
                $scope.theme.textValue = "LIGHT";
                $scope.appClass = "background_light";
                $scope.primaryColor = "primary_color";
                $scope.secondColor = "second_color";
                $scope.secondBackgroundColor = "background_light";
                $scope.thirdBackgroundColor = "third_background_light";
                $scope.thirdColor = "third_light_color";
                $scope.fourthBackgroundColor = "fourth_background_light";
            } else {
                $scope.theme.textValue = "DARK";
                $scope.appClass = "background_dark";
                $scope.primaryColor = "primary_dark_color";
                $scope.secondColor = "second_dark_color";
                $scope.secondBackgroundColor = "second_background_dark";
                $scope.thirdBackgroundColor = "third_background_dark";
                $scope.thirdColor = "third_light_color";
                $scope.fourthBackgroundColor = "fourth_background_dark";
            }
        }
        appService.getTheme().then(onThemeLoad, onError);
       
    };


    app.controller("appController", ["$scope", "appService", "documentService", appController])
}());