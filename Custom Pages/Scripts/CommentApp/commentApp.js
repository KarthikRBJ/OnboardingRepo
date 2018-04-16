(function () {


    var app = angular.module('commentApp', ["ngRoute"]);

    app.config(function ($routeProvider) {
        $routeProvider
              .when("/commentApp", {
                  templateUrl: "http://192.168.0.148/Relativity/CustomPages/b9c4168d-a204-4478-b15b-89110f7abebb/Scripts/CommentApp/Views/App/main.html",
                  controller: "appController"
              })
              .when("/documentList", {
                  templateUrl: "http://192.168.0.148/Relativity/CustomPages/b9c4168d-a204-4478-b15b-89110f7abebb/Scripts/CommentApp/Views/Document/DocumentList.html",
                  controller: "documentController"
              })
            .when("/commentList/:ParentCommentAI", {
                templateUrl: "http://192.168.0.148/Relativity/CustomPages/b9c4168d-a204-4478-b15b-89110f7abebb/Scripts/CommentApp/Views/Comment/CommentChilds.html",
                controller: "commentController"
            })
            .when("/commentDetails/:commentAI", {
                templateUrl: "http://192.168.0.148/Relativity/CustomPages/b9c4168d-a204-4478-b15b-89110f7abebb/Scripts/CommentApp/Views/Comment/CommentDetails.html",
                controller: "commentController"
             })

        
              .otherwise({ redirectTo: "/documentList" })

    });

}());