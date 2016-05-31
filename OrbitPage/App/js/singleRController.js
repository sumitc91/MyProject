$(document).ready(function () {
    ConnectWithSignalRPushNotification();
});

function ConnectWithSignalRPushNotification() {
    var _name = "sumitxyz";
    $("#spnName").text(_name);
    $("#txtMsg").val('');

    var chatProxy = $.connection.ChatHub;



    $.connection.hub.start().done(function () {

        chatProxy.server.registerClient($.cookie('uservertexid'));

        $("#btnSend").click(function () {
            chatProxy.server.broadCastMessage($("#spnName").text(), $("#txtMsg").val());
            $("#txtMsg").val('').focus();
        });
    });

    chatProxy.client.addMessage = function (message) {
        console.log(message);
        scope.$apply(function () {
            scope.UpdateUserOnlineStatusFromPushNotification(message);
        });
    };

    //sendChatMessage
    chatProxy.client.sendChatMessage = function (displayName, userVertexId, imageUrl, msg, messageImage) {
        console.log(displayName + " --> "+msg);
        var scope = angular.element(document.getElementById("main")).scope();
        scope.$apply(function () {
            
            scope.UpdateChatMessageFromPushNotification(displayName, userVertexId, imageUrl, msg, messageImage);
           
        });
    };

    chatProxy.client.addNotificationMessage = function (code,message) {
        console.log(code);
        var scope = angular.element(document.getElementById("main")).scope();
        scope.$apply(function () {
            if (code == 1) {
                console.log("1 : "+code);
                scope.clientNotificationDetailResponseInfoUpdateFromPushNotification();
            }
            else if (code == 2) {
                console.log("2 : " + code);
                scope.clientFriendRequestNotificationDetailResponseInfoUpdateFromPushNotification(message);
            }
            else if (code == 3) {
                console.log("3 : " + message);
                scope.clientOrbitFeedNotificationDetailResponseInfoUpdateFromPushNotification(message);
            } else {
                console.log("push notification logic failed.");
            }
        });
    };

    chatProxy.client.receiveMessage = function (msgFrom, msg) {
        //$("#divChat").append("<li><strong>" + msgFrom + "</strong>:&nbsp;&nbsp;" + msg + "</li>");
        showToastMessage("Success", "<strong>" + msgFrom + "</strong>:&nbsp;&nbsp;" + msg);
    };
}

//$(function () {
//    // Proxy created on the fly          
//    var signalRHub = $.connection.SignalRHub;

//    // Get the user name and store it to prepend to messages.
//    //$('#displayname').val(prompt('Enter your name:', ''));

//    // Declare a function on the chat hub so the server can invoke it          
//    signalRHub.client.sendMessage = function (name, message) {
        
//        //alert(message);

//        showToastMessage("Success", name + ": sent Message - " + message);
//    };

//    signalRHub.server.registerClient("sumitchourasia91@gmail.com");
    
//    signalRHub.client.addMessage = function (message) {
//        alert("user registered : " + message);

//    };

//    signalRHub.client.updateBeforeLoginUserProjectDetails = function (totalProjects, successRate, totalUsers, projectCategories) {
//        //alert("updateBeforeLoginUserProjectDetailsService");
//        //alert("testing");
        
//        showToastMessage("Success", name + ": sent Message - " + message);
//        //showStickyToastMessage("Success", totalProjects + " - " + successRate + " - " + totalUsers + " - " + projectCategories);
//    };

//    // Start the connection
//    $.connection.hub.start().done(function () {
//        $("#send").click(function () {
//            // Call the chat method on the server
//            signalRHub.server.send($('#displayname').val(), $('#msg').val());
//        });
//    });
//});
