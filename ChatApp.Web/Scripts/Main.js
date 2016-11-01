/*
var connection = $.hubConnection();
var ChatHubProxy = connection.createHubProxy('ChatHub');
ChatHubProxy.on('IncomingMessage', function (messageModel) {
    console.log(messageModel);
});
ChatHubProxy.on('UserJoined', function (userModel) {
    console.log("userJoined");
    console.log(userModel);
});
ChatHubProxy.on('UserLeft', function (userModel) {
    console.log("userLeft");
    console.log(userModel);
});
ChatHubProxy.on('StartMessageFileUpload', function (messageID) {
    console.log("MssageID:" + messageID);
});

ChatHubProxy.on('MessageReaded', function (userModel {UserID : 123, Name = "ReZiL_dEdE"}, messageID GUID) {
    Diğer kullanıcılara bildirimde bulunmak için kullanıcak.
    Mesajın ReaderUsers arrayine eklemek gerekecek ki mesaj tekrar okundu olarak sunucuya gönderilmesin.
});
ChatHubProxy.on('InitMessages', function (messages) {
    meesages = message model array
    kullanıcı signalr ye bağlandığında son okuduğu mesaja göre mesajları topluca burayageliyor.
    öncesinde ve sonrasında 30 message olabiliyor en fazla.
    son okunan mesajın LastReadedMessage property si true
    
});


var server =  function (hubProxy, scrollMoveMessageCallback/*message model array ve scrollUp'ı parametre alan, sendMessageCallback/*messageID nullsa başarısız)
{
    var hp  = hubProxy;
    this.MessageReaded = function (messageID)
    {
        mesajın okduğunu sunucuya bildirilecek sunucu diğer kullanıcılara bu mesajın bu kullanıcı yarafından okunduğunu bildirecek. ve son okunan olarak kaydedilecek.
        bu method çağırılırken message modelde ki ReadedUsers ların id si kontrol edilmeli ki gereksiz bildirimler sunucuya gelmesin.
        
        hp.invoke('MessageReaded', messageID);
    }
    this.SendMessage = function (messageModel)
    {
        hp.invoke("SendMessage",messageModel/*{ Message: "Deneme Message", AttachType: 0, ReplyToMessageID: null }).done(function (result) {
            sendMessageCallback(result)
        });
    }
    this.ScrollMoveMessages = function (messageID ,scrollUp )
    {
        hp.invoke("ScrollMoveMessages",messageID, scrollUp).done(function (result) {
            scrollMoveMessageCallback(result,scrollUp);
        });
    }
}

var _server = new server(ChatHubProxy, function (messages, scrollUp) { }, function (messageID) { })

connection.start().done(function () {
    _server.SendMessage({ Message: "Deneme Message", AttachType: 0, ReplyToMessageID: null })
});
*/