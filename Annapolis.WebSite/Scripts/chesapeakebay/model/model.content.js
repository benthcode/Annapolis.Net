//depend on circle.model
(function ($m, app) {

    //var testObj = $.parseJSON('{"identifier":null,"userName":null,"registerEmail":null,"contactEmail":null,"password":null,"isAuthenticated":false,"id":"00000000-0000-0000-0000-000000000000","serverErrors":{"aaaa":{"isFaulty":true,"isOff":false,"message":"eeeeeee"}}}');
    $m.Tag = function (jsonObj, params) {
        if (!jsonObj) return null;
        var self = this;
        $m.ViewModel.call(self, jsonObj, params);
        self.$type = "Tag";
    };
    $.extend($m.Tag.prototype, $m.ViewModel.prototype);

    $m.TagOption = function (jsonObj, params) {
        if (!jsonObj) return null;
        var self = this;
        $m.ViewModel.call(self, jsonObj, params);
        self.$type = "TagOption";
    };
    $.extend($m.TagOption.prototype, $m.ViewModel.prototype);
    $m.TagOption.create = function (obj) {
        var defaultObj = { serverStatus: null };
        return new $m.TagOption($.extend(defaultObj, obj));
    };

    $m.Thread = function (jsonObj, params) {
        if (!jsonObj) return null;
        var self = this;
        $m.ViewModel.call(self, jsonObj, params);
        self.$type = "Thread";
    };
    $.extend($m.Thread.prototype, $m.ViewModel.prototype);

    $m.Vote = function (jsonObj, params) {
        if (!jsonObj) return null;
        var self = this;
        $m.ViewModel.call(self, jsonObj, params);
        self.url = "/vote";
        self.$type = "Vote";
    };
    $m.Vote.prototype = function () {
        var _makeVote = function (url) {
            var self = this;
            if (self.userName() === $ca.user.userName()) {
                self.notify({ message: $clg("Vote.VoteCannotForYourSelf") });
            } else {
                self.basicSelfModelAjax({ url: url }, true);
            }
        }

        var voteUp = function () {
            this._makeVote("/api/vote/voteup");
        };

        var voteDown = function () {
            this._makeVote("/api/vote/votedown");
        };

        return {
            _makeVote: _makeVote,
            voteUp: voteUp,
            voteDown: voteDown
        };
    }();
    $.extend($m.Vote.prototype, $m.ViewModel.prototype);

    $m.Topic = function (jsonObj, params) {
        if (!jsonObj) return null;
        var mOption = {
            koMapping:
            {
                'comments':
                  {
                      create: function (options) {
                          return new $m.Comment(options.data);
                      }
                  },
                'tagOptions': {
                    create: function (options) {
                        return new $m.ModelList(options.data);
                    }
                },
                'vote': {
                    create: function (options) {
                        return new $m.Vote(options.data);
                    }
                }
            }
        },
        self = this;
        $.extend(true, mOption, params);


        $m.OwnerableViewModel.call(self, jsonObj, mOption);

        self.url = "/topic";
        self.$type = "Topic";
        self.title.extend({ required: { params: true, message: $clg("Topic.TitleIsRequired") } });
        self.content.extend({ required: { params: true, message: $clg("Topic.ContentIsRequired") } });

    };
    $m.Topic.prototype = function () {
        var changeTagOption = function (tagCategory, newValue) {
            var self = this, found = false;
            ko.utils.arrayForEach(self.tagOptions.models(), function (item) {
                if (item.categoryName() === tagCategory) {
                    item.idStrs(newValue);
                    found = true;
                    return false;
                }
            });
            if (found === false) {
                self.tagOptions.models.push($m.TagOption.create({ categoryName: tagCategory, idStrs: newValue }));
            }
        };

        var changeThread = function (newValue) {
            var self = this;
            self.threadId(newValue);
        };

        return {
            changeTagOption: changeTagOption,
            changeThread: changeThread
        };
    }();
    $.extend($m.Topic.prototype, $m.OwnerableViewModel.prototype);

    $m.Comment = function (jsonObj, params) {
        if (!jsonObj) return null;
        var mOption = {
            koMapping:
                    {
                        'vote': {
                            create: function (options) {
                                return new $m.Vote(options.data);
                            }
                        }
                    }
        },
             self = this;
        $.extend(true, mOption, params);
        $m.OwnerableViewModel.call(this, jsonObj, mOption);

        self.url = "/comment";
        self.$type = "Comment";
        self.content.extend({ required: { params: true, message: $clg("Comment.ContentIsRequired") } });
    };
    $m.Comment.prototype = function () {
        var checkUploadPermission = function (url, callbackFn) {
            var self = this;
            self.basicSelfModelAjax({ url: url }, true, callbackFn);
        };

        return {
            checkUploadPermission: checkUploadPermission
        };
    }();
    $.extend($m.Comment.prototype, $m.OwnerableViewModel.prototype);
    $m.Comment.create = function (obj) {
        var defaultObj = { content: '', serverStatus: null };
        return new $m.Comment($.extend(true, defaultObj, obj));
    };

    $m.TopicWithPageComments = function (jsonObj) {
        if (!jsonObj) return null;
        var mOption = {
            koMapping: {
                'topic': {
                    create:
                      function (options) {
                          return new $m.Topic(options.data);
                      }
                },
                'pageComments': {
                    create:
                                        function (options) {
                                            return new $m.PageList(options.data);
                                        }
                }
            }
        },
        self = this;

        $m.ViewModel.call(self, jsonObj, mOption);
        self.$type = "TopicWithPageComments";
        self.url = "/api/topic/getCommentsOnPage";

        amplify.subscribe($ca.notification.pageNumberChanged + self.pageComments.uniqueId, function (event) {
            var url = self.url + '/' + self.topic.id() + "?page=" + event.pageNumber;
            self.basicTargetModelAjax(url, null, self.pageComments, { type: 'GET' });
        });
    };
    $m.TopicWithPageComments.prototype = function () {

        var gotoPage = function (pageNumber) {
            var self = this, url = self.url + '/' + self.topic.id() + "?page=" + pageNumber;
            self.basicTargetModelAjax(url, null, self.pageComments, { type: 'GET' });
        };

        return {
            gotoPage: gotoPage
        };
    }();
    $.extend($m.TopicWithPageComments.prototype, $m.ViewModel.prototype);

    $m.TagList = function (jsonObj) {
        if (!jsonObj) return null;
        $m.SelectableList.call(this, jsonObj);
        this.$type = "TagList";
    };
    $.extend($m.TagList.prototype, $m.SelectableList.prototype);

    $m.ThreadList = function (jsonObj) {
        if (!jsonObj) return null;
        $m.SelectableList.call(this, jsonObj);
        this.$type = "ThreadList";
    };
    $.extend($m.ThreadList.prototype, $m.SelectableList.prototype);

    $m.PageTopicFilter = function (jsonObj) {
        if (!jsonObj) return null;
        var mOption = {
            koMapping: {
                'TagOptions': {
                    create: function (options) {
                        return new $m.ModelList(options.data);
                    }
                }
            }
        },
        self = this;

        $m.ViewModel.call(self, jsonObj, mOption);
    };
    $.extend($m.PageTopicFilter.prototype, $m.ViewModel.prototype);

    $m.PageTopic = function (jsonObj) {
        if (!jsonObj) return null;
        var mOption = {
            koMapping: {
                'page': {
                    create: function (options) {
                        return new $m.PageList(options.data);
                    }
                },
                'filter': {
                    create: function (options) {
                        return new $m.PageTopicFilter(options.data);
                    }
                }
            }
        },
        self = this;

        $m.ViewModel.call(self, jsonObj, mOption);
        self.$type = "PageTopic";

        amplify.subscribe($ca.notification.pageNumberChanged + self.page.uniqueId, function (event) {
            self.filter.pageNumber(event.pageNumber);
            self.requestNewPage();
        });

    };
    $m.PageTopic.prototype = function () {
        var changeTagFilter = function (tagCategory, newValue) {
            var self = this, found = false;
            ko.utils.arrayForEach(self.filter.tagOptions.models(), function (item) {
                if (item.categoryName() === tagCategory) {
                    item.idStrs(newValue);
                    found = true;
                    return false;
                }
            });
            if (found === false) {
                self.filter.tagOptions.models.push($m.TagOption.create({ categoryName: tagCategory, idStrs: newValue }));
            }
            self.filter.pageNumber($ca.page.firstPageNumber);
            self.requestNewPage();
        };

        var requestNewPage = function() {
            var self = this;
            var url = "/api/topic/topicsbyfilter";
            self.basicTargetModelAjax(url, self.filter, self.page);
        };

        return {
            changeTagFilter: changeTagFilter,
            requestNewPage: requestNewPage
        };
    }();
    $.extend($m.PageTopic.prototype, $m.ViewModel.prototype);

})($cm, $ca);

