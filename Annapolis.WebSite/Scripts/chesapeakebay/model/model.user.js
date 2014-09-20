//depend on circle.model
(function ($m) {

    var app = $ca;

    $m.UserRegistration = function (jsonObj, args) {

        $m.EditableViewModel.call(this, jsonObj, args);

        var self = this;
        self.$type = "UserRegistration";
        self.url = "/account";
        self.confirmPassword = ko.observable(jsonObj.password);

        self.userName.extend({ required: { params: true, message: $clg("Membership.ClientError.UserNameIsEmpty") } })
                         .extend({ minLength: { params: 4, message: $clg("Membership.ClientError.UserNameIsTooShort") } })
                         .extend({ pattern: { params: /^([\w\d_]|[\u4e00-\u9fa5]){2,18}$/, message: $clg("Membership.ClientError.InvalidUserNamePattern") } });
        self.password.extend({ required: { params: true, message: $clg("Membership.ClientError.PasswordIsEmpty") } })
                        .extend({ minLength: { params: 6, message: $clg("Membership.ClientError.PasswordIsTooShort") } });
        self.confirmPassword.extend({ sameAs: { params: self.password, message: $clg("Membership.ClientError.ConfirmPasswordNotMatched") } });
        self.registerEmail.extend({ required: { params: true, message: $clg("Membership.ClientError.RegisterEmailIsEmpty") } })
                        .extend({ pattern: { params: /^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$/, message: $clg("Membership.ClientError.InvalidEmail") } });

    };
    $m.UserRegistration.prototype = function () {
        var register = function () {
            var self = this, isValid;
            if (self.isClientValid() && self.holdChanges()) {
                self.commitChanges();
                self.beginChanges();
                var option = {
                    async: false,
                    url: self.url + "/register",
                    type: "POST",
                    success: function (data, status) {
                        if ($.type(data) === "string") {
                            data = JSON.parse(data);
                        }
                        if ($cu.isServerSuccess(data) === true) {
                            self.updateFromJson(data, app.user);
                            amplify.publish(app.notification.serverUserAuthenticatedEvent, self);
                            isValid = $ca.validateCurrentUser();
                            $ca.isCurrentUserValid(isValid);
                        } else {
                            self.updateFromJson(data);
                        }
                    }
                };
                self.basicSelfModelAjax(option, false);
            }
        };
        return {
            register: register
        };
    }();
    $.extend($m.UserRegistration.prototype, $m.EditableViewModel.prototype);
    $m.UserRegistration.create = function (obj) {
        var defaultObj = { userName: '', registerEmail: '', contactEmail: '', password: '' };
        return new $m.UserRegistration($.extend(true, defaultObj, obj));
    };

    $m.UserSignIn = function (jsonObj, args) {

        var opt = { editableExtender: { isCookiePersistent: { editable: false } } };
        $m.EditableViewModel.call(this, jsonObj, $.extend(true, opt, args));

        var self = this;
        self.$type = "UserSignIn";
        self.url = "/account";

        self.identifier.extend({ required: { params: true, message: $clg("Membership.ClientError.UserNameOrEmailIsEmpty") } })
                       .extend({ pattern: { params: /^(([\w\d_]|[\u4e00-\u9fa5]){2,18})|(([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+))$/, message: $clg("Membership.ClientError.UserNameOrEmailIsInvalid") } });
        self.password.extend({ required: { params: true, message: $clg("Membership.ClientError.PasswordIsEmpty") } })
                     .extend({ minLength: { params: 6, message: $clg("Membership.ClientError.PasswordIsTooShort") } });
    };
    $m.UserSignIn.prototype = function () {
        var signIn = function () {
            var self = this, isValid;
            if (self.isClientValid() && self.holdChanges()) {
                self.commitChanges();
                self.beginChanges();
                var option = {
                    async: false,
                    url: self.url + "/signIn",
                    type: "POST",
                    success: function (data, status) {
                        if ($.type(data) === "string") {
                            data = JSON.parse(data);
                        }
                        if ($cu.isServerSuccess(data) === true) {
                            self.updateFromJson(data, app.user);
                            amplify.publish(app.notification.serverUserAuthenticatedEvent, self);
                            isValid = $ca.validateCurrentUser();
                            $ca.isCurrentUserValid(isValid);
                        } else {
                            self.updateFromJson(data);
                        }
                    }
                };
                self.basicSelfModelAjax(option, false);
            }
        };
        return {
            signIn: signIn
        };
    }();
    $.extend($m.UserSignIn.prototype, $m.EditableViewModel.prototype);
    $m.UserSignIn.create = function (obj) {
        var defaultObj = { identifier: '', password: '', isCookiePersistent: true };
        return new $m.UserSignIn($.extend(true, defaultObj, obj));
    };

    $m.TokenUser = function (jsonObj, args) {

        $m.ViewModel.call(this, jsonObj, args);

        var self = this;
        self.$type = "TokenUser";
        self.url = "/account";

        self.isAuthenticated = ko.computed(function () {
            var isAuth = self.isApproved() && !self.isLocked();
            return isAuth;
        });

        self.signOut = function () {
            var option = {
                async: false,
                url: self.url + "/signout",
                type: "POST",
                success: function (data, status) {
                    if ($.type(data) === "string") {
                        data = JSON.parse(data);
                    }
                    if ($cu.isServerSuccess(data) === true) {
                        self.updateFromJson(data, app.user);
                        amplify.publish(app.notification.serverUserSignOutEvent, self);
                        $ca.isCurrentUserValid(false);
                    }
                }
            };
            self.basicSelfModelAjax(option, false);
        };

        self.hasSignedIn = function () {
            if (ko.isObservable(self.token)) {
                var tk = self.token();
                if (tk != null && tk != '') {
                    return true;
                }
            }
            return false;
        };

        amplify.publish(app.notification.tokenUserCreated, self);
    };
    $.extend($m.TokenUser.prototype, $m.ViewModel.prototype);

    $m.UserPasswordUpdate = function (jsonObj, args) {

        $m.EditableViewModel.call(this, jsonObj, args);

        var self = this;
        self.$type = "UserPasswordUpdate";
        self.url = "/account";
        self.confirmNewPassword = ko.observable(jsonObj.newPassword);

        self.oldPassword.extend({ required: { params: true, message: $clg("Membership.ClientError.PasswordIsEmpty") } })
                        .extend({ minLength: { params: 6, message: $clg("Membership.ClientError.PasswordIsTooShort") } });
        self.newPassword.extend({ required: { params: true, message: $clg("Membership.ClientError.PasswordIsEmpty") } })
                        .extend({ minLength: { params: 6, message: $clg("Membership.ClientError.PasswordIsTooShort") } });
        self.confirmNewPassword.extend({ sameAs: { params: self.newPassword, message: $clg("Membership.ClientError.ConfirmPasswordNotMatched") } });
    };
    $m.UserPasswordUpdate.prototype = function () {
        var updatePass = function () {
            var self = this, isValid;
            if (self.isClientValid() && self.holdChanges()) {
                self.commitChanges();
                self.beginChanges();
                var option = {
                    async: false,
                    url: "/api" + self.url + "/UpdatePassword",
                    type: "POST"
                };
                self.basicSelfModelAjax(option, true);
            }
        };
        return {
            updatePass: updatePass
        };
    }();
    $.extend($m.UserPasswordUpdate.prototype, $m.EditableViewModel.prototype);

    //    var aa = new $m.ViewModel({ a: "aaa", b: "bbb" });
    //    var bb = new $m.SavableViewModel({ a: "aaa", b: "bbb" });
    //    var cc = new $m.EditableViewModel({ a: "aaa", b: "bbb" });

    //    cc.clearServerNotifications();

    var xx = $m.UserSignIn.create();

    var myValue = ko.observable().extend({ required: true });


    //    var user = {
    //        FirstName: ko.observable('Some'),
    //        LastName: ko.observable('Person'),
    //        Address: {
    //            Country: ko.observable('USA'),
    //            City: ko.observable('Washington')
    //        }
    //    };
    //    ko.editable(user);

    //    user.beginEdit();
    //    user.FirstName('MyName');
    //    var t = user.hasChanges();          // returns `true`
    //    user.commit();
    //    t = user.hasChanges();          // returns `false`
    //    user.Address.Country('Ukraine');
    //    t = user.hasChanges();          // returns `true`
    //    user.rollback();
    //    t = user.Address.Country();     // returns 'USA'

    //    var fun = function () { };

    //    var f = new fun();

    //    var x = f.prototype;

})($cm);

