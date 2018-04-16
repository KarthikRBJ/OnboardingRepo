(function () {

    var commentService = function ($http) {


        var getCommentsChilds = function (artifactId) {
            return $http.get("http://192.168.0.148/Relativity/CustomPages/b9c4168d-a204-4478-b15b-89110f7abebb/Home/GetCommentReplys/?artifactId=" + artifactId)
                  .then(function (response) {
                      return response.data;
                  });
        };

        var getCommentsChildsByObjectManager = function (artifactId) {
            return $http.get("http://192.168.0.148/Relativity/CustomPages/b9c4168d-a204-4478-b15b-89110f7abebb/Home/GetReplysByObjectManager/?artifactId=" + artifactId)
                  .then(function (response) {
                      return response.data;
                  });
        };


        var getCommentDataByObjectManager = function (artifactId) {
            return $http.get("http://192.168.0.148/Relativity/CustomPages/b9c4168d-a204-4478-b15b-89110f7abebb/Home/GetCommentData/?commentAI=" + artifactId)
                  .then(function (response) {
                      return response.data;
                  });
        }

        var getCommentDataByRSAPI = function (artifactId) {
            return $http.get("http://192.168.0.148/Relativity/CustomPages/b9c4168d-a204-4478-b15b-89110f7abebb/Home/GetCommentDataByRsapi/?commentAI=" + artifactId)
                  .then(function (response) {
                      return response.data;
                  });
        }
        var getCommentAudit = function (commentAI) {
            return $http.get("http://192.168.0.148/Relativity/CustomPages/b9c4168d-a204-4478-b15b-89110f7abebb/Home/getCommentAudit/?commentId=" + commentAI)
                  .then(function (response) {
                      return response.data;
                  });
        }
        

        



        return {
            getCommentsChilds: getCommentsChilds,
            getCommentDataByObjectManager: getCommentDataByObjectManager,
            getCommentsChildsByObjectManager: getCommentsChildsByObjectManager,
            getCommentAudit: getCommentAudit,
            getCommentDataByRSAPI: getCommentDataByRSAPI
        };

    };


    var module = angular.module("commentApp");
    module.factory("commentService", commentService);

}());