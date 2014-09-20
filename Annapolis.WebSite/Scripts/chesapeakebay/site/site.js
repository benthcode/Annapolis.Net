(function (global) {

    var app = global.cpkb.app;

    var showNotification = function (item) {
        if (item.isVisible()) {
            $.noty.closeAll();
            noty({
                text: item.message(),
                type: item.type() || app.notification.information,
                timeout: item.timeOut() <= 0 ? false : item.timeOut(), 
                modal: item.isModal(),
                buttons: false,
                layout: 'center',
                theme: 'defaultTheme',
                animation: {
                    open: { height: 'toggle' },
                    close: { height: 'toggle' },
                    easing: 'swing',
                    speed: 500 
                },
                force: false, 

                maxVisible: 5, 
                closeWith: ['click'],
               
            });
        }
    };

    amplify.subscribe(app.notification.serverNotificationReceivedEvent, function (m) {
        if (!m || !m.serverNotifications || !m.serverNotifications() || !ko.isObservable(m.serverNotifications)) return;
        ko.utils.arrayForEach(m.serverNotifications(), function (item) {
            showNotification(item);
        });
        m.clearServerNotifications();
    });

    amplify.subscribe(app.notification.clientNotificationReceiveEvent, function (m) {
        showNotification(m);
    });

    amplify.subscribe(app.notification.userAuthenticationRequired, function (redirectUrl, callbackAfterAuth) {
        $cs.auth.requireAuthentication(redirectUrl, callbackAfterAuth);
    });

    global.cpkb.site = global.$cs = {};

    $cs.eventfn = {};

    $cs.reload = function (timeout) {
        var time = 1500, t = parseInt(timeout);
        if (!isNaN(t)) {
            time = t;
        }
        setTimeout(function () { location.reload(); }, time);
    };
    $cs.redirectToHome = function () {
        $cs.redirect("/");
    };
    $cs.redirect = function (url, timeout) {
        var time = 1500, t = parseInt(timeout);
        if (!isNaN(t)) {
            time = t;
        }
        if (jQuery.type(url) === "string") {
            setTimeout(function () { window.location.href = url }, time);
        }
    };

    $cs.notify = function (notification) {
        var noti = {};
        if (typeof (notification) === "string") {
            noti.message = notification;
        } else {
            $.extend(true, noti, notification);
        }

        if (noti.type === undefined || noti.type === null) { noti.type = app.notification.type.information; }
        if (noti.timeOut === undefined || noti.timeOut === null) { noti.timeOut = 2000; }
        if (noti.isModal === undefined || noti.isModal === null) { noti.isModal = false; }
        if (noti.isVisible === undefined || noti.isVisible === null) { noti.isVisible = true; }

        amplify.publish(app.notification.clientNotificationReceiveEvent, new $cm.Notification(noti));
    };

    $cs.auth = {};
    $cs.auth.requireAuthentication = function (redirectUrl, callbackAfterAuth) {

        if ($ca.user.hasSignedIn() === true) {
            if (!$ca.user.isApproved()) {
                $cs.notify({ message: $clg("Membership.ClientError.SignInNotApproved") + $clg("Membership.ClientError.ContactAdminstrator") });
                return;
            } else if ($ca.user.isLocked()) {
                $cs.notify({ message: $clg("Membership.ClientError.SingInButLocked") + $clg("Membership.ClientError.ContactAdminstrator") });
                return;
            } else if ($.isFunction(callbackAfterAuth)) {
                callbackAfterAuth();
            }

            if (redirectUrl) { $cs.redirect(redirectUrl, duration); }
        }
        else {
            var authDlg = $('#divUserAuthentication-j');
            authDlg.removeData();
            authDlg.data("redirectUrlAfterAuth", redirectUrl);
            authDlg.data("callbackAfterAuth", callbackAfterAuth);
            authDlg.modal();
        }
    }

    //auth-required   auth-permission  auth-fn  auth-link   auth-duration 
    $(document).on('click', "[auth-required]", function () {
        var node = $(this),
            required = node.attr("auth-required") === 'true',
            permission = node.attr("auth-permission"),
            callbackfn = $cs.eventfn[node.attr("auth-fn")],
            duration = node.attr("auth-duration") || 0,
            link = node.attr("auth-link");

        if (required === true) {
            if ($ca.isCurrentUserValid() === true) {

                if ($.isFunction(callbackfn)) { callbackfn(); }
                if (link) { $cs.redirect(link, duration); }

            } else {
                amplify.publish(app.notification.userAuthenticationRequired, link, callbackfn);
            }
        }
    });


    $("#divPageTopic-j").on('click', ".resizableImg-j", function () {
       
        $("#divPageTopic-j .resizableImg-clone-j").remove();

        var element = $(this), clone, position;

        clone = $(element.clone());
        clone.removeClass('resizableImg-j');
        clone.addClass('resizableImg-clone-j');
        position = element.position();

        clone.css("top", position.top).css("left", position.left).css("z-index", 1000);
        clone.on("click", function (e) {
            clone.animate({
                height: element.height(),
                width: element.width()
            }, 700, function () {
                clone.remove();
            });
        });

        clone.appendTo(element.parent());
        clone.css("position", "absolute").animate({
            height: "160px",
            width: "160px"
        }, 700);

    });

    $(function () {
        $.scrollUp({
            scrollName: 'scrollUp', // Element ID
            scrollDistance: 150, // Distance from top/bottom before showing element (px)
            scrollFrom: 'top', // 'top' or 'bottom'
            scrollSpeed: 300, // Speed back to top (ms)
            easingType: 'linear', // Scroll to top easing (see http://easings.net/)
            animation: 'fade', // Fade, slide, none
            animationInSpeed: 200, // Animation in speed (ms)
            animationOutSpeed: 200, // Animation out speed (ms)
            scrollText: 'Scroll to top', // Text for element, can contain HTML
            scrollTitle: false, // Set a custom <a> title if required. Defa ults to scrollText
            scrollImg: true, // Set true to use image
            activeOverlay: false, // Set CSS color to display scrollUp active point, e.g '#00FFFF'
            zIndex: 2147483647 // Z-Index for the overlay
        });
    });

})(window);

