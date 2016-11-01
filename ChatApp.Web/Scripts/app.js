/**
 * Created by ejder on 10/29/2016.
 */

var app = angular.module("chatApp", ["ngAnimate"]);

var DataService = ["$rootScope", "$q", function ($rootScope, $q) {

    var connection = $.hubConnection();
    var ChatHubProxy = connection.createHubProxy('ChatHub');
    var connected = false;

    ChatHubProxy.on('InitChatRoom', function (messages, otherOnlineUsers) {
        $rootScope.$broadcast("InitMessages", messages);
        $rootScope.$broadcast("InitUsers", otherOnlineUsers);
    });

    /*Messages*/
    ChatHubProxy.on('IncomingMessage', function (messageModel) {
        $rootScope.$broadcast("IncomingMessage", messageModel);
    });

    messageSeen = function (messageID) {
        ChatHubProxy.invoke('MessageReaded', messageID);
    }

    loadMessagesByScroll = function (messageID) {
        if (connected) {
            return ChatHubProxy.invoke("ScrollMoveMessages", messageID, true);
        }
    }

    /*Users*/
    ChatHubProxy.on('UserJoined', function (user) {
        $rootScope.$broadcast("UserJoined", user);
    });

    connection.start().done(function () {
        connected = true;
    });

    return {
        messageSeen: messageSeen,
        loadMessagesByScroll: loadMessagesByScroll
    };
}];

var UserListController = ["$rootScope", "$scope", "DataService", function($rootScope, $scope, DataService){
    $scope.users = [];

    $scope.$on("InitUsers", function (event, users) {
        $scope.$apply(function () {
            $scope.users = users;
        });
    });

    $scope.userClick = function(user){
        $rootScope.$broadcast("userClicked", user);
    };


    $scope.$on("UserJoined", function (event, user) {
        $scope.users.push(user);
        $scope.$apply();
    });

    $scope.$on("UserLeft", function (event, leftUser) {
        remainingUsers = _.remove($scope.users, function (user) {
            return (user.UserID == leftUser.UserID);
        });
        $scope.$apply(function () {
            $scope.users = remainingUsers;
        });
    });

}];
var UserDirective = function(){
    return {
        templateUrl:"/Chat/UserTemplate",
        restrict: "E",
        transclude: false,
        scope: {
            nick:"=nick",
            avatar:"=avatar"
        }
    };
};
var MessageListController = ["$rootScope", "$scope", "DataService", "$timeout", function ($rootScope, $scope, DataService, $timeout) {

    $scope.messages = [];

    /*
    $scope.$watch(function () {
        return messageListEl.scrollTop;
    },
    function (oldVal, newVal) {
        if (newVal == 0) {
            DataService.loadMessagesByScroll()
            .done(function (result) {
                if (result) {
                    $scope.messages.unshift(result);
                }
            });
        }
    });
    */

    scrollToBottom = function () {
        $timeout(function () {
            messageListEl = document.querySelector("#chatroom__message-list--scrollable");
            messageListEl.scrollTop = messageListEl.scrollHeight;
        });
    }

    $scope.$on("InitMessages", function (event, messages) {
        $scope.$apply(function () {
            $scope.messages = messages;
            scrollToBottom();
        });
    });

    $scope.$on("IncomingMessage", function (event, message) {
        $scope.$apply(function () {
            $scope.messages.push(message);
            scrollToBottom();
        });
    });

    $scope.messageClick = function(message){
        $rootScope.$broadcast("messageClicked", message);
    }

}];

var MessageDirective = function(){
    return {
        templateUrl:"/Chat/MessageTemplate",
        retrict:"E",
        transclude:false,
        scope:{
            text:"=?text",
            timeStamp: "=?date",
            image:"=?image",
            ownerNick:"=ownernick",
            ownerAvatar:"=owneravatar",
            replyTo:"=?replyto"
        }
    };
};

var EntryFactory = function(){
    return {
        text:null,
        timeStamp: new Date().getTime(),
        replyTo: null,
        image: null
    };
}

var EntryController = ["$scope", function($scope){
    $scope.currentMessage = null;

    var getInstance = function(){
        if(!$scope.currentMessage){
            $scope.currentMessage = new EntryFactory();
        }
        return $scope.currentMessage;
    }

    $scope.cleanReply = function(){
        var entry = getInstance();
        entry.replyTo = null;
    }

    $scope.$on("messageClicked", function(event, message){
        var entry = getInstance();
        console.log(message);
        //entry.replyTo = message.owner.nick + ": " + message.text;
    });

    $scope.$on("userClicked", function(event, user){
        var entry = getInstance();
        if (entry.text) {
            entry.text += " ";
        }
        entry.text = "@" + user.Name + " ";
    });

}];

app.service("DataService", DataService);
app.controller("UserListController", UserListController);
app.controller("MessageListController", MessageListController);
app.controller("EntryController", EntryController);
app.directive("chatUser", UserDirective);
app.directive("chatMessage", MessageDirective);
