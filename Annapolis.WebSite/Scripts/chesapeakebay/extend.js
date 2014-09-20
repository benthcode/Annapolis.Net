(function (cpkb) {

    JSON.stringify = JSON.stringify || function (obj) {
        var t = typeof (obj);
        if (t != "object" || obj === null) {
            // simple data type
            if (t == "string") obj = '"' + obj + '"';
            return String(obj);
        }
        else {
            // recurse array or object
            var n, v, json = [], arr = (obj && obj.constructor == Array);
            for (n in obj) {
                v = obj[n]; t = typeof (v);
                if (t == "string") v = '"' + v + '"';
                else if (t == "object" && v !== null) v = JSON.stringify(v);
                json.push((arr ? "" : '"' + n + '":') + String(v));
            }
            return (arr ? "[" : "{") + String(json) + (arr ? "]" : "}");
        }
    };


    /*********************** knock out **********************/

    ko.bindingHandlers.authClick = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            $(element).click(function () {
                var value = valueAccessor();
                if ($ca.isCurrentUserValid() === true) {
                    value.call(viewModel);
                } else {
                    //wrap value as a function to pass to publish, if calling function after auth
                    amplify.publish($ca.notification.userAuthenticationRequired);
                }
            });
        }
    };

    //svBack svSuccUrl, svErrUrl, svDuration, svCallBack; svActionKey
    ko.bindingHandlers.svBack = {

        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = valueAccessor();
            if (value === undefined || value === null) return;
            //value(null);
        },

        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var value = valueAccessor(), sStatus = null, redirectUrl = null, callback = null, svActionKey, serverActionKey;
            if (value === undefined || value === null) return;
            sStatus = ko.unwrap(value);
            if (sStatus === undefined || sStatus === null) return;
            svActionKey = allBindings.get('svActionKey');
            
            if(svActionKey !== undefined && svActionKey !== null && svActionKey !== '')
            {
                serverActionKey = viewModel.serverActionKey();
                if (svActionKey !== serverActionKey) {
                    //value(null);
                    return;
                }
            }


            if (sStatus === true) {
                redirectUrl = allBindings.get('svSuccUrl');
                callback = allBindings.get("svSuccCallBack");
            }
            else if (sStatus === false) {
                redirectUrl = allBindings.get('svErrUrl');
                callback = allBindings.get("svErrCallBack");
            }
            if (callback) {
                callback(viewModel);
            }
            if (redirectUrl) {
                var duartion = allBindings.get('svDuration');
                $cs.redirect(redirectUrl, duartion);
            }

            //value(null);
        }
    };

    ko.bindingHandlers.buttonSetValue = {
        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            if (ko.unwrap(valueAccessor()) === bindingContext.$parent.selectedValue()) { $(element).addClass('active'); }
            $(element).click(function () {
                var rootElementId = allBindings.get("rootElementId"), value = valueAccessor(), selectValue = ko.unwrap(value);
                $('#'+rootElementId).find("*").removeClass('active');
                bindingContext.$parent.selectedValue(selectValue);
                $(element).addClass('active');
            });
        }
    };

    ko.bindingHandlers.kindEditorValue = {

        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

            var toolItems = ['source', '|', 'undo', 'redo', '|', 'cut', 'copy', 'paste', 'plainpaste', 'wordpaste', '|',
                              'justifyleft', 'justifycenter', 'justifyright', 'justifyfull',
                              'insertorderedlist', 'insertunorderedlist', 'indent', 'outdent', 'subscript',
                              'superscript', 'clearhtml', 'quickformat', 'selectall', '|', 'fullscreen', '/',
                              'formatblock', 'fontname', 'fontsize', '|', 'forecolor', 'hilitecolor', 'bold',
                              'italic', 'underline', 'strikethrough', 'lineheight', 'removeformat', '|', 'image'],
                attr = allBindings.get("attr"),
                uploadDoc = attr.canUploadDocument,
                uploadImg = attr.canUploadImage,
                uploadUrl = attr.uploadUrl;
            attr.width = $(element).parent().width();
            if (uploadDoc) { toolItems.push('insertfile'); }
            $.merge(toolItems, ['table', 'hr', 'emoticons', 'pagebreak', 'anchor', 'link', 'unlink', '|', 'about']);
            KindEditor.ready(function (K) {
                var editor = K.create(element, $.extend({
                    langType: "en",
                    height: 300,
                    resizeType: 1,
                    items: toolItems,
                    allowImageUpload: uploadImg,
                    uploadJson: uploadUrl, //'/UploadFile/Upload',   //'KindEditor/asp.net/upload_json.ashx',
                    afterFocus: function () {
                        if ($ca.isCurrentUserValid() !== true) {
                            amplify.publish($ca.notification.userAuthenticationRequired);
                        }
                    },
                    afterChange: function () {
                        //K.sync('textarea[name="xxxx"]');
                        var observable = valueAccessor();
                        observable(this.html());
                    },
                    afterCreate: function () {
                        if ($ca.isCurrentUserValid() !== true) {
                            this.readonly(true);
                        }
                    }
                }, attr));
                $(window).on('resize', function () {
                    if (editor) {
                        var width = $(element).parent().width();
                        editor.resize(width, null);
                    }
                });

            });
        },

        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

            var valueUnwrapped = ko.unwrap(valueAccessor());
            $(element).val(valueUnwrapped);
        }
    };

    ko.bindingHandlers.kindThumbnail = {

        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var image = $('<img />'),
                address = $('<input type="text" class="span8" readonly />'),
                button = $('<button class="btn btn-info" style="margin-bottom:10px" >' + $clg("Editor.ChooseImage") + '</button>'),
                uploadUrl = allBindings.get("uploadUrl"),
                thumbnail;
            $(element).append(address).append(button).append(image);
            thumbnail = valueAccessor();
            image.attr("src", ko.unwrap(thumbnail));
            address.val(ko.unwrap(thumbnail));

            KindEditor.ready(function (K) {
                var editor = K.editor({
                    allowFileManager: false,
                    langType: "en",
                    uploadJson: uploadUrl
                });

                button.click(function () {
                    editor.loadPlugin('image', function () {
                        editor.plugin.imageDialog({
                            showRemote: false,
                            imageUrl: address.val(),
                            clickFn: function (url, title, width, height, border, align) {
                                image.attr("src", url);
                                address.val(url);
                                thumbnail(url);
                                editor.hideDialog();
                            }
                        });
                    });
                });
            });
        }
    };

    //pagination
    ko.bindingHandlers.pagination = {

        init: function (element, valueAccessor, allBindings, viewModel, bindingContext) {
            var attr = allBindings.get('attr'),
               url = valueAccessor() || '';
            if (viewModel.totalCount() <= 0 || viewModel.actualSizeOnPage() <= 0) return;
            $(element).pagination({
                items: ko.unwrap(viewModel.totalCount),
                itemsOnPage: ko.unwrap(viewModel.pageSize),
                currentPage: ko.unwrap(viewModel.pageNumber),
                cssStyle: 'light-theme', //compact-theme, dark-theme
                onPageClick: function (pageNumber, event) {
                    viewModel.onPageNumberChangedd(url, pageNumber);
                }
            });
        },

        update: function (element, valueAccessor, allBindings, viewModel, bindingContext) {

            var attr = allBindings.get('attr');
            url = valueAccessor();
            $(element).pagination('destroy');
            if (viewModel.totalCount() <= 0 || viewModel.actualSizeOnPage() <= 0) return;

            $(element).pagination({
                items: ko.unwrap(viewModel.totalCount),
                itemsOnPage: ko.unwrap(viewModel.pageSize),
                currentPage: ko.unwrap(viewModel.pageNumber),
                cssStyle: 'light-theme',
                onPageClick: function (pageNumber, event) {
                    viewModel.onPageNumberChanged(url, pageNumber);
                }
            });
        }

    };

    ko.bindingHandlers.uniqueFor = {
        init: function (element, valueAccessor, allBindingsAccessor) {
            //var value = valueAccessor(), forValue = ko.unwrap(value);
            //element.setAttribute("for", forValue);
        }
    };

    ko.unapplyBindings = function (node, remove) {
        // unbind events
        node.find("*").each(function () {
            $(this).unbind();
        });
        // Remove KO subscriptions and references
        if (remove) {
            ko.removeNode(node[0]);
        } else {
            ko.cleanNode(node[0]);
        }
    };

    /*************** ko Validation**********************/
    ko.validation.init({ insertMessages: false });

    ko.validation.rules["sameAs"] = {
        validator: function (val, otherObservable) {
            return val === otherObservable();
        },
        message: $clg("Error.Validation.NotSame")
    };

    ko.validation.registerExtenders();

    /**************** ko Editable ********************/

    ko.isEditable = function (model) {
        if (!model) return false;
        if ($.isFunction(model.beginEdit)) {
            return true;
        }
        return false;
    };


    /***************** moment ************************/
    zone = {};
    zone.toTimeDiff = function (val) {
        var localTime = moment.utc(val).local(),
            diff = moment().diff(localTime),
            d = moment.duration(diff).humanize() + ' '+ $clg("App.Time.Ago");
        return d;
    }

})(cpkb);