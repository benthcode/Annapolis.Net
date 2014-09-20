(function (global) {

    if (global.cpkb == undefined) {
        global.cpkb = global.$c = {};
        global.cpkb.app = global.$ca = {};
        global.cpkb.locale = global.$cl = {};

        //get resource text by key
        $cl.get = global.$clg = function (key) {
            if (key in cpkb.locale) { return cpkb.locale[key]; }
            return key;
        };

        //define notification type and and event for the server response
        $ca.notification = {
            type: {
                success: "success", error: "error", alert: "alert",
                warning: "warning", information: "information", confirmation: "confirmation"
            },

            clientNotificationReceiveEvent: "client.notification.received",

            serverNotificationReceivedEvent: "server.notification.received",
            serverUserAuthenticatedEvent: "server.notification.userAuthenticated",
            serverUserSignOutEvent: "server.notification.userSignOut",
            tokenUserCreated: "notification.tokenUserCreated",
            userAuthenticationRequired: "notification.userAuthenticationRequired",

            selectableListValueChanged: "notification.selectableListValueChanged",
            pageNumberChanged: "notification.pageNumberChanged",

            progressOperationBeginEvent: "progressOperationBeginEvent",
            progressOperationEndEvent: "progressOperationEndEvent"

        };

        $ca.page = { firstPageNumber: 1 };

        $ca.validateCurrentUser = function (user) {
            if (!$ca.user || !$ca.user.userName() || $ca.user.isAuthenticated() !== true) return false;
            return true;
        };
        $ca.isCurrentUserValid = ko.observable(false);
        amplify.subscribe($ca.notification.tokenUserCreated, function (tokenUser) {
            $ca.user = tokenUser;
            $ca.isCurrentUserValid($ca.validateCurrentUser());
        });

    }

})(window);