(function () {

    amplify.subscribe($ca.notification.serverUserSignOutEvent, function afterSignOut() {
        amplify.unsubscribe($ca.notification.serverUserSignOutEvent, afterSignOut);
        $ca.isCurrentUserValid(false);
        $cs.redirectToHome();
    });

    var dlgAuth = $("#divUserAuthentication-j"),
        dlgAuthBind = dlgAuth.find(".userAuthenticationBinder-j"),
        afterAuthFunc;

    dlgAuth.on("shown",
            function (event) {
                var thisDlg = $(this),
                    callbackAfterAuth = thisDlg.data("callbackAfterAuth"),
                    redirectUrl = thisDlg.data("redirectUrlAfterAuth"),
                    userRegistration = $cm.UserRegistration.create(),
                    userSignIn = $cm.UserSignIn.create();

                afterAuthFunc = function() {
                    dlgAuth.modal('hide');
                    if ($.isFunction(callbackAfterAuth)) {
                        callbackAfterAuth();
                    }
                    if (redirectUrl) {
                        $cs.redirect(redirectUrl);
                    }

                };
                amplify.subscribe($ca.notification.serverUserAuthenticatedEvent, afterAuthFunc);

                var viewModel = { registerUser: userRegistration, signInUser: userSignIn };
                viewModel.registerUser.beginChanges();
                viewModel.signInUser.beginChanges();

                ko.applyBindings(viewModel, dlgAuthBind[0]);
            });

    dlgAuth.on("hidden", function (event, ui) {
        ko.unapplyBindings(dlgAuthBind, false);
        amplify.unsubscribe($ca.notification.serverUserAuthenticatedEvent, afterAuthFunc);
    });

})();