(function () {


    var app = angular.module('commentApp');
    
    var commentController = function ($scope, commentService, $routeParams) {

        $scope.parentCommentId = $routeParams.ParentCommentAI;
        $scope.commentId = $routeParams.commentAI;
        var onCommentsLoad = function (data) {
           
            $scope.childs = data;
        }
        
        

        var onCommentLoad = function (data) {
           
            $scope.commentData = data;
            $scope.commentText = $scope.commentData.Name;
            
            
        }

        var onCommentsAuditLoad = function (data) {
            
            $scope.audit = data;
            $scope.showAudit = true;
        }
       
        var onError = function (reason) {
            $scope.error = reason;
            console.log($scope.error);
        }

         $scope.getCommentAudit = function (commentAI) {
            commentService.getCommentAudit(commentAI).then(onCommentsAuditLoad, onError);
        }

       
        
        if ($scope.commentId != undefined) {
            commentService.getCommentDataByRSAPI($scope.commentId).then(onCommentLoad, onError);
            
        }

        if ($scope.parentCommentId != undefined) {
            commentService.getCommentsChilds($scope.parentCommentId).then(onCommentsLoad, onError);
            //commentService.getCommentsChildsByObjectManager($scope.parentCommentId).then(onCommentsLoad, onError);
        }
        $scope.showReplys = function (comment, id) {
            $scope.showTree = true;
            var array = comment.CommentChilds;
            if (array.length == 0) {
                document.getElementById("tree").innerHTML = "<b>Without Reply's</b>";
            } else {
                document.getElementById("tree").innerHTML = "<b>Reply's</b>";
                $scope.drawCommentTree(comment, id);
            }
        }

        $scope.drawCommentTree = function (comment, id) {
            var array = comment.CommentChilds;
            elemento1 = document.createElement('ul');
            elemento1.id = comment.ArtifactId + "c";
            elemento2 = document.getElementById(id);
            elemento2.appendChild(elemento1);

            array.forEach(function (element) {
                elemento3 = document.createElement('li');
                elemento3.id = element.ArtifactId;
                elemento3.className = "element textComment";
                elemento4 = document.getElementById(comment.ArtifactId + "c");
                elemento4.appendChild(elemento3);
                var d = document.getElementById(element.ArtifactId);

                if (element.imageBase64) {
                    d = document.getElementById(element.ArtifactId);
                    d.innerHTML += "<img src='" + element.imageBase64 + "' />"
                }
                d.innerHTML += "<p class = 'orange_relativity' >Comment: " + element.Name + "</p>";
                d.innerHTML += "<p class = 'orange_relativity'>Created by: <b>" + element.CreatedBy.Name + "<b></p>";
                d.innerHTML += "<p class = 'orange_relativity'>Created On: " + element.CreatedOn + "</p>";
                $scope.drawCommentTree(element, comment.ArtifactId + "c");
            })

        }

    };


    app.controller("commentController", ["$scope", "commentService", "$routeParams", commentController])
}());