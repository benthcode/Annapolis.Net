$(document).ready(function () {

    //ko.applyBindings($ca.user, $("#divLoginOnPanel-j")[0]);
   
    amplify.subscribe($ca.notification.serverUserAuthenticatedEvent, function () {
        $.post("/account/loginonpanel", function (data) {
            $('#divMenuLoginOnPanelHolder-j').html(data);
            $("#layoutSignOutMenu-j").click(function () {
                $ca.user.signOut();
            });
        });
    });

    $("#layoutSignInMenu-j, #layoutRegisterMenu-j").click(function () {
        $cs.auth.requireAuthentication();
    });

    $("#layoutSignOutMenu-j").click(function () {
        $ca.user.signOut();
    });

    (function () {

        var progressBarDiv = $('<div class="progressbar-centered" style="display:none"><img src="/Content/chesapeakebay/images/progress_indicator.gif" /></div>')
        $('body').append(progressBarDiv);
        amplify.subscribe($ca.notification.progressOperationBeginEvent, function () {
            progressBarDiv.show();
        });

        amplify.subscribe($ca.notification.progressOperationEndEvent, function () {
            progressBarDiv.hide();
        });
        
    })();

    
});