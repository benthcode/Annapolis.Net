//depend on amplify, json2, knockout, jquery 
(function (global) {

    global.cpkb.model = global.$cm = {};

    var $m = $cm, app = $ca,
        helper = {

            defaultApiRoute: "/api",

            modelToJson: function (modelObj) { return ko.mapping.toJS(modelObj); },

            createFromJson: function (modelObj, jsonObj) {
                if (jsonObj == null) return;
                var self = this;
                if (jQuery.isFunction(modelObj.creatingFromJson) === true) {
                    modelObj.creatingFromJson(jsonObj);
                }
                ko.mapping.fromJS(jsonObj, modelObj.koMapping, modelObj);
                if (jsonObj.serverStatus === undefined) { modelObj.serverStatus = ko.observable(null); }
                if (jQuery.isFunction(modelObj.createdFromJson) === true) {
                    modelObj.createdFromJson(jsonObj);
                }
            },

            updateFromJson: function (modelObj, jsonObj) {
                if (modelObj == null || jsonObj == null) return;
                var self = this;
                if (jQuery.isFunction(modelObj.updatingFromJson) === true) {
                    modelObj.updatingFromJson(jsonObj);
                }
                ko.mapping.fromJS(jsonObj, modelObj.koMapping, modelObj);
                if (jQuery.isFunction(modelObj.updatedFromJson) === true) {
                    modelObj.updatedFromJson(jsonObj);
                }
                modelObj.serverStatus(jsonObj.serverStatus);
            },

            basicSelfModelAjax: function (modelObj, option, isApiCall, successCallBack, errorCallBack) {
                this.basicTargetModelAjax(modelObj.url, modelObj, modelObj, option, isApiCall, successCallBack, errorCallBack);
            },

            basicTargetModelAjax: function (url, requestModel, targetModel, option, isApiCall, successCallBack, errorCallBack) {
                if (requestModel && typeof (requestModel) === 'object'
                        && typeof (requestModel.actionBeforeAjax) === 'function') {
                    requestModel.actionBeforeAjax();
                }
                var self = this,
                pData = null,
                ajaxOption = {
                    url: url,
                    contentType: 'application/json',
                    dataType: "json",
                    type: "POST",
                    success: function (data, status) {
                        if ($.type(data) === "string") {
                            data = JSON.parse(data);
                        }
                        if (targetModel) {
                            targetModel.updateFromJson(data);
                            if (targetModel.serverStatus() === true) {
                                if (typeof (successCallBack) === 'function') {
                                    successCallBack(requestModel, targetModel, data, status);
                                }
                            }
                            else if (targetModel.serverStatus() === false) {
                                if (typeof (errorCallBack) === 'function') {
                                    errorCallBack(requestModel, targetModel, status, errorThrown);
                                }
                            }
                        }
                    },
                    error: function (xhr, status, errorThrown) {
                        window.alert(errorThrown);
                    }
                },
                inProgressIndicator = false;
                try{
                    if (requestModel !== undefined && requestModel !== null) {
                        pData = JSON.stringify(self.modelToJson(requestModel));
                    }
                    if (isApiCall === false) {
                        if (pData !== null) {
                            ajaxOption.data = pData; //{ jsonModel: pData };
                        }
                    } else {
                        ajaxOption.headers = { "cpkbUserName": app.user.userName(), "cpkbToken": app.user.token() };
                        if (pData !== null) {
                            ajaxOption.data = pData;
                        }
                    };

                    $.extend(true, ajaxOption, option);
                    amplify.publish($ca.notification.progressOperationBeginEvent);
                    inProgressIndicator = true;
                    $.ajax(ajaxOption).always(function () {
                        amplify.publish($ca.notification.progressOperationEndEvent);
                        inProgressIndicator = false;
                    });
                } catch (err) {
                    if (inProgressIndicator) {
                        amplify.publish($ca.notification.progressOperationEndEvent);
                        inProgressIndicator = false;
                    }
                }
            }

        };


    //Class design goals:
    $m.Notification = function(notiObj) {
        if (!notiObj) return null;
        var self = this;
        self.message = ko.observable(notiObj.message);
        self.type = ko.observable(notiObj.type);
        self.timeOut = ko.observable(notiObj.timeOut);
        self.isModal = ko.observable(notiObj.isModal);
        self.isVisible = ko.observable(notiObj.isVisible);
    };

    /***************  ViewModel section **********/
    //Support auto creation and update from json data, and to json data.
    //Support knockout inheritance
    //Support server notification event publisher
    $m.ViewModel = function (jsonObj, params) {
        if (!jsonObj) return null;
        var self = this,
            defaultKoMapping = {
                'serverNotifications': {
                    create: function (options) {
                        return new $m.Notification(options.data);
                    }
                },
                'ignore': ["uniqueId", "serverStatus"]
            },
            mapping = (params && params.koMapping) ? params.koMapping : null;
        self.$type = "ViewModel";
        self.serverNotifications = ko.observableArray([]); //create observableArray, common array if comment this line;
        self.koMapping = {};
        $.extend(true, self.koMapping, defaultKoMapping, mapping);
        helper.createFromJson(self, jsonObj);
        self.uniqueId = jsonObj.uniqueId;
        self.serverStatus = ko.observable(null);
    };
    $m.ViewModel.prototype = function () {

        var clearServerNotifications = function () {
            if (ko.isObservable(this.serverNotifications)) {
                //this.serverNotifications.removeAll();
                while (this.serverNotifications() && this.serverNotifications().length > 0) {
                    this.serverNotifications.pop();
                }
            }
        };

        var updateFromJson = function(jsonObj, modelObj) {
            var updateModel = typeof(modelObj) === 'object' ? modelObj : this;
            helper.updateFromJson(updateModel, jsonObj, this.koMapping);
            if (ko.isEditable(updateModel)) {
                this.commitChanges();
                this.beginChanges();
            }
        };

        var toJson = function () { return helper.modelToJson(this); };

        //Event
        var creatingFromJson = function (jsonObj) { };
        var createdFromJson = function (jsonObj) { };

        var updatingFromJson = function (jsonObj) { };
        var updatedFromJson = function (jsonObj) {
            amplify.publish(app.notification.serverNotificationReceivedEvent, this);
        };

        var actionBeforeAjax = function () {
            if (typeof (this.clearServerNotifications()) === 'function') {
                this.clearServerNotifications();
            }
            if (ko.isObservable(this.serverStatus)) {
                this.serverStatus(null);
            }
        };

        var basicSelfModelAjax = function (option, isApiCall, successCallBack, errorCallBack) {
            helper.basicSelfModelAjax(this, option, isApiCall, successCallBack, errorCallBack);
        };

        var basicTargetModelAjax = function (url, requestModel, targetModel, option, isApiCall, successCallBack, errorCallBack) {
            helper.basicTargetModelAjax(url, requestModel, targetModel, option, isApiCall, successCallBack, errorCallBack);
        };

        var notify = function(notification) {
            var noti = {};
            if (typeof(notification) === "string") {
                noti.message = notification;
            } else {
                $.extend(true, noti, notification);
            }

            if (noti.type === undefined || noti.type === null) {
                noti.type = app.notification.type.information;
            }
            if (noti.timeOut === undefined || noti.timeOut === null) {
                noti.timeOut = 2000;
            }
            if (noti.isModal === undefined || noti.isModal === null) {
                noti.isModal = false;
            }
            if (noti.isVisible === undefined || noti.isVisible === null) {
                noti.isVisible = true;
            }

            amplify.publish(app.notification.clientNotificationReceiveEvent, new $m.Notification(noti));
        };

        return {

            clearServerNotifications: clearServerNotifications,
            updateFromJson: updateFromJson,
            toJson: toJson,

            //Event
            creatingFromJson: creatingFromJson,
            createdFromJson: createdFromJson,
            updatingFromJson: updatingFromJson,
            updatedFromJson: updatedFromJson,

            //ajax
            actionBeforeAjax: actionBeforeAjax,
            basicSelfModelAjax: basicSelfModelAjax,
            basicTargetModelAjax: basicTargetModelAjax,

            //message
            notify: notify
        };
    }();

    /***************  Validatable section **********/
    //Support knockout client validation
    $m.ValidatableViewModel = function(jsonObj, params) {
        if (!jsonObj) return null;
        $m.ViewModel.call(this, jsonObj, params);
        this.$type = "ValidatableViewModel";

        var self = this;
        self.isOperationValid = ko.computed(function() {
            var isValid = self.isClientValid();
            return isValid;
        });
    };
    $m.ValidatableViewModel.prototype = function () {

        var isClientValid = function () {
            var self = this;
            for (var prop in self) {
                if (self.hasOwnProperty(prop) && ko.isObservable(self[prop]) && ko.validation.utils.isValidatable(self[prop])) {
                    if (!self[prop].isValid()) {
                        return false;
                    }
                }
            }
            return true;
        };

        var clientErrors = function () {
            var self = this,
                errors = {};
            for (var prop in self) {
                if (self.hasOwnProperty(prop) && ko.isObservable(self[prop]) && ko.validation.utils.isValidatable(self[prop])) {
                    if (!self[prop].isValid()) errors[self.prop] = self.errors;
                }
            }
            return errors;
        };

        return {
            isClientValid: isClientValid,
            clientErrors: clientErrors
        };

    }();
    $.extend($m.ValidatableViewModel.prototype, $m.ViewModel.prototype);

    /***************  SavableViewModel section **********/
    //Support basci CRUD operation for ajax call to server
    //Support server errors auto handling
    $m.SavableViewModel = function (jsonObj, params) {
        if (!jsonObj) return null;
        $m.ValidatableViewModel.call(this, jsonObj, params);
        this.$type = "SavableViewModel";
    };
    $m.SavableViewModel.prototype = function () {

        var save = function (successCallBack, errorCallBack) {
            if ($cu.isValidGuid(this.id())) {
                this.update(successCallBack, errorCallBack);
            } else {
                this.add(successCallBack, errorCallBack);
            }
        };

        var remove = function (succFun, errFun) {
            helper.basicSelfModelAjax(this, { url: helper.defaultApiRoute + this.url, type: "DELETE" }, true, succFun, errFun);
        };

        var add = function (succFun, errFun) {
            helper.basicSelfModelAjax(this, { url: helper.defaultApiRoute + this.url + "/create", type: "POST" }, true, succFun, errFun);
        };

        var update = function (succFun, errFun) {
            helper.basicSelfModelAjax(this, { url: helper.defaultApiRoute + this.url + "/update/" + this.id(), type: "PUT" }, true, succFun, errFun);
        };

        return {
            save: save,
            remove: remove,
            add: add,
            update: update
        };

    }();
    $.extend($m.SavableViewModel.prototype, $m.ValidatableViewModel.prototype);

    /***************  EditableViewModel  **************/
    //Support editable operation for model, which means if there is some error, you cannot commit it again before fixing it.
    $m.EditableViewModel = function (jsonObj, params) {
        if (!jsonObj) return null;
        var self = this, extendProp;
        $m.SavableViewModel.call(self, jsonObj, params);
        self.$type = "EditableViewModel";
        if (self.id !== undefined && ko.isObservable(self.id)) { self.id.extend({ editable: false }); }
        self.serverStatus.extend({ editable: false });
        if (params && params.editableExtender) {
            for (extendProp in params.editableExtender) {
                self[extendProp].extend(params.editableExtender[extendProp]);
            }
        }

        ko.editable(self);

        self.holdChanges = ko.computed(function () {
            var isChanged = self.hasChanges();
            return isChanged;
        });

        self.isOperationValid = ko.computed(function () {
            var isValid = self.isClientValid() && self.holdChanges();
            return isValid;
        });

        self.beginEdit();
    };
    $m.EditableViewModel.prototype = function () {

        var beginChanges = function () {
            this.beginEdit();
        };

        var commitChanges = function () {
            this.commit();
        };

        var rollbackChanges = function () {
            this.rollback();
        };

        return {
            beginChanges: beginChanges,
            commitChanges: commitChanges,
            rollbackChanges: rollbackChanges
        };
    }();
    $.extend($m.EditableViewModel.prototype, $m.SavableViewModel.prototype);

    /**************  OwnerableViewModel  *************/
    //Support privilege check such as canAdd, canEdit and canDelete
    $m.OwnerableViewModel = function(jsonObj, params) {
        if (!jsonObj) return null;
        $m.EditableViewModel.call(this, jsonObj, params);
        this.$type = "OwnerableViewModel";
    };
    //m.OwnerableViewModel.prototype = function () { }();
    $.extend($m.OwnerableViewModel.prototype, $m.EditableViewModel.prototype);

    /**************  ModelList  *************/
    $m.ModelList = function (jsonObj, mOption) {
        if (!jsonObj) return null;
        var params = { koMapping: {} },
            targetNameSpace = jsonObj.targetModelNameSpace,
            targetModel = jsonObj.targetModel,
            self = this;

        params.koMapping['models'] = {
            create: function (options) {
                return new global[targetNameSpace][targetModel](options.data);
            }
        };
        $.extend(true, params, mOption);

        self.models = ko.observableArray([]);
        $m.ViewModel.call(self, jsonObj, params);
        self.$type = "ModelList";
    };
    $m.ModelList.prototype = function () {
        var unshift = function (item) {
            this.models.unshift(item);
        };

        return {
            unshift: unshift
        };
    }();
    $.extend($m.ModelList.prototype, $m.ViewModel.prototype);

    /**************  PageList  *************/
    $m.PageList = function (jsonObj, mOption) {
        if (!jsonObj) return null;
        $m.ModelList.call(this, jsonObj, mOption);
        this.$type = "PageList";
    };
    $m.PageList.prototype = function () {
        var onPageNumberChanged = function (url, pageNumber) {
            var self = this;
            if (pageNumber < 1) return;
            amplify.publish(app.notification.pageNumberChanged + self.uniqueId,
                                { url: url, pageNumber: pageNumber, uniqueId: self.uniqueId });
        };

        return {
            onPageNumberChanged: onPageNumberChanged
        };
    }();
    $.extend($m.PageList.prototype, $m.ModelList.prototype);

    /**************  SelectableList  *************/
    $m.SelectableList = function (jsonObj, mOption) {

        var self = this,
            params = { koMapping: { 'ignore': ["notifyValueChangedEvent"] } };
        if (jsonObj.multiSelect === true) {
            var selections = [];
            if (jsonObj.selectedValue) {
                selections = jsonObj.selectedValue.split(",");
            }
            jsonObj.selectedValue = selections;
            self.selectedValue = ko.observableArray([]);
        }
        $.extend(true, params, mOption);
        $m.ModelList.call(self, jsonObj, params);
        self.$type = "SelectableList";

        if (jsonObj.notifyValueChangedEvent === true) {
            self.selectedValue.subscribe(function () {
                var newSelectedValue = self.selectedValue();
                if ($.isArray(newSelectedValue)) { newSelectedValue = newSelectedValue.join(','); }
                amplify.publish($ca.notification.selectableListValueChanged + "_" + self.uniqueId, {
                    newValue: newSelectedValue,
                    target: self.target(), group: self.group(), uniqueId: self.uniqueId
                });
            });
        }
    };
    $.extend($m.SelectableList.prototype, $m.ModelList.prototype);

})(window);

