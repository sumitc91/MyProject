$(function () {
    // Proxy created on the fly          
    var signalRHub = $.connection.SignalRHub;

    // Get the user name and store it to prepend to messages.
    //$('#displayname').val(prompt('Enter your name:', ''));

    // Declare a function on the chat hub so the server can invoke it          
    signalRHub.client.sendMessage = function (name, message) {
        
        //alert(message);

        showToastMessage("Success", name + ": sent Message - " + message);
    };

    signalRHub.server.registerClient("sumitchourasia91@gmail.com");
    
    signalRHub.client.addMessage = function (message) {
        alert("user registered : " + message);

    };

    signalRHub.client.updateBeforeLoginUserProjectDetails = function (totalProjects, successRate, totalUsers, projectCategories) {
        //alert("updateBeforeLoginUserProjectDetailsService");
        //alert("testing");
        
        showToastMessage("Success", name + ": sent Message - " + message);
        //showStickyToastMessage("Success", totalProjects + " - " + successRate + " - " + totalUsers + " - " + projectCategories);
    };

    // Start the connection
    $.connection.hub.start().done(function () {
        $("#send").click(function () {
            // Call the chat method on the server
            signalRHub.server.send($('#displayname').val(), $('#msg').val());
        });
    });
});

$(function () {

    $("#sendControls2").hide("fast");

    // Proxy created on the fly

    var notificationHub = $.connection.SignalRHub;

    // Declare a function on the hub so the server can invoke it

    notificationHub.client.addMessage = function (message) {

        $("#messages2").append("<li>" + message + "</li>");

    };

    // Start the connection

    $.connection.hub.start().done(function () {

        $("#register2").click(function () {

            // Register client.

            notificationHub.server.registerClient($("#user2").val());

            $("#registration2").hide("fast");

            $("#sendControls2").show("fast");

        });

        $("#send2").click(function () {

            // Call the method on the server

            notificationHub.server.addNotification($("#msg2").val(), $("#toUsr2").val());

        });

    });

});