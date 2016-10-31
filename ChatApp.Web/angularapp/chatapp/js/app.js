/**
 * Created by ejder on 10/29/2016.
 */

var app = angular.module("chatApp", ["ngAnimate"]);

var DataService = ["$rootScope", function($rootScope){

    var getUserList = function(){
        //burada serverin kullanici listesini getirmesi lazim
        return [
            {nick:"Ejder", avatar:"http://sap-certification.info/img/default-avatar.jpg", id:"001"},
            {nick:"Ejder", avatar:"http://sap-certification.info/img/default-avatar.jpg", id:"002"},
            {nick:"Ejder", avatar:"http://sap-certification.info/img/default-avatar.jpg", id:"003"},
            {nick:"Ejder", avatar:"http://sap-certification.info/img/default-avatar.jpg", id:"004"}
        ];
    };
    var getMessageList = function(){
        //burada server message listesini gonderiyor
        return [
            {
                id:"0001",
                text:"Text message text message text message text message",
                timeStamp: new Date().getTime(),
                replyTo: "Ejder: Reply edilen message",
                image: "http://imgsv.imaging.nikon.com/lineup/lens/zoom/normalzoom/af-s_dx_18-140mmf_35-56g_ed_vr/img/sample/sample1_l.jpg",
                owner:{
                    id:"0001",
                    nick:"Ejder",
                    avatar:"http://sap-certification.info/img/default-avatar.jpg"
                }
            }
        ];
    };

    //bu callbacklerin de signair gonderince cagirilmasi lazim
    var userAdded = function(user){
        $rootScope.$broadcast("userAdded", user);
    };
    var userUpdated = function(user){
        $rootScope.$broadcast("userUpdated", user);
    };
    var userRemoved = function(userId){
        $rootScope.$broadcast("userRemoved", userId);
    };
    var messageAdded = function(message){
        $rootScope.$broadcast("messageAdded", message);
    };
    var messageUpdated = function(message){
        $rootScope.$broadcast("messageUpdated", message);
    };
    var messageRemoved = function(messageId){
        $rootScope.$broadcast("messageRemoved", messageId);
    };

    return {
        getUserList: getUserList,
        getMessageList: getMessageList
    };
}];

var UserListController = ["$rootScope", "$scope", "DataService", function($rootScope, $scope, DataService){
    $scope.users = DataService.getUserList();

    $scope.userClick = function(user){
        $rootScope.$broadcast("userClicked", user);
    };
    $scope.$on("userAdded", function(event, user){
       $scope.users.push(user);
    });
    $scope.$on("userUpdated", function(event, user){
        for(var i=0; i<$scope.users.length; i++){
            if($scope.users[i].id == user.id){
                $scope.users[i] = user;
                break;
            }
        }
    });
    $scope.$on("userRemoved", function(event, userId){
        $scope.users = _.remove($scope.users, function(user){
            return (user.id == userId);
        });
    });
}];
var UserDirective = function(){
    return {
        templateUrl:"user.html",
        controller: UserListController,
        restrict: "E",
        transclude: false,
        scope: {
            nick:"=nick",
            avatar:"=avatar"
        }
    };
};
var MessageListController = ["DataService", "$rootScope", "$scope", function(DataService, $rootScope, $scope){
    $scope.messages = DataService.getMessageList();
    $scope.messageClick = function(message){
        $rootScope.$broadcast("messageClicked", message);
    }
    $scope.$on("messageAdded", function(event, message){
        $scope.messages.push(message);
    });
    $scope.$on("messageUpdated", function(event, message){
        for(var i=0; i<$scope.messages.length; i++){
            if($scope.messages[i].id == message.id){
                $scope.messages[i] = message;
                break;
            }
        }
    });
    $scope.$on("messageRemoved", function(event, messageId){
        $scope.messages = _.remove($scope.messages, function(message){
            return (message.id == messageId);
        });
    });
}];
var MessageDirective = function(){
    return {
        templateUrl:"message.html",
        controller:MessageListController,
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
        image: null,
        owner:{
            id:null,
            nick:null,
            avatar:null
        }
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
        entry.replyTo = message.owner.nick + ": " + message.text;
    });

    $scope.$on("userClicked", function(event, user){
        var entry = getInstance();
        if(entry.text){
            entry.text += " @"+user.nick+" ";
        }else{
            entry.text = "@"+user.nick+" ";
        }
    });

}];

app.service("DataService", DataService);
app.controller("UserListController", UserListController);
app.controller("MessageListController", MessageListController);
app.controller("EntryController", EntryController);
app.directive("chatUser", UserDirective);
app.directive("chatMessage", MessageDirective);
