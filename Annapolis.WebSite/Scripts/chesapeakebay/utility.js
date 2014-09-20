(function (global) {

    $c.utility = global.$cu = {};

    var utility = $cu,
        noti = $ca.notification;


    utility.extend = function (a, b) {
        $.extend(a, b);
    };

    utility.emptyGuid = function () {
        return "00000000-0000-0000-0000-000000000000";
    };

    utility.isValidGuid = function (id, isEmptyGuidValid) {
        if (id == null || id == '') return false;
        isEmptyGuidValid = isEmptyGuidValid || false;
        if (!isEmptyGuidValid) {
            if (id == "00000000-0000-0000-0000-000000000000") return false;
        }
        var reg = /^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}(\}){0,1}$/;
        return reg.test(id);
    }

    utility.checkNotificationType = function (data) {
        if (data && data.serverNotifications && data.serverNotifications.length > 0) {
            for (var tp in noti.type) {
                if (noti.type[tp] == data.serverNotifications[0].type) {
                    return tp;
                }
            }
        }
        return null;
    };

    utility.isServerSuccess = function (data) {
        if (!data || $.type(data.serverStatus) !== "boolean") return false;
        return data.serverStatus;
    }

    utility.isSuccessNotification = function (data) {
        return utility.checkNotificationType(data) === noti.type.success;
    }

    utility.isErrorNotification = function (data) {
        return utility.checkNotificationType(data) === noti.type.error;
    }

})(window);