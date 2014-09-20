$(document).ready(function () {


    window.fn = {};


    //Topic/Show:  Call after new post saved
    fn.afterNewCommentSaved = function (comment) {
        commentsPerTopic.gotoPage(0);
        commentsPerTopic.pageComments.unshift(comment);
        var newCommentHolder = $("#divNewCommentHolder-j");
        KindEditor.remove(newCommentHolder.find("textarea"));
        newCommentHolder.remove();
    };

    //Edit for Comment
    (function () {

        var lastIndex = -1,
            commentEditHolder = $("#comment-edit-holder-j"),
            commentEditTextarea = commentEditHolder.find("textarea"),
            commentViewHolder = null,
            commentViewContent = null,
            commentModel = null;

        var closeCommentEditor = function () {
            commentEditHolder.hide();
            KindEditor.remove(commentEditTextarea);
            commentViewHolder.show();
            lastIndex = -1;
            commentModel = null;
        }

        commentEditHolder.hide();

        fn.commentEditButtonClick = function (index, option) {

            if (lastIndex === index) return;

            if (lastIndex !== -1) {
                commentViewHolder.show();
                KindEditor.remove(commentEditTextarea);
            } else {
                commentEditHolder.show();
            }

            commentModel = this;
            commentViewHolder = $("#comment-view-holder_" + index);
            commentViewContent = commentViewHolder.find(".comment-view-content-j");

            commentEditHolder.insertBefore(commentViewHolder);
            commentEditTextarea.val(commentViewContent.html());
            commentViewHolder.hide();

            var toolItems = ['source', '|', 'undo', 'redo', '|', 'cut', 'copy', 'paste', 'plainpaste', 'wordpaste', '|',
                              'justifyleft', 'justifycenter', 'justifyright', 'justifyfull',
                              'insertorderedlist', 'insertunorderedlist', 'indent', 'outdent', 'subscript',
                              'superscript', 'clearhtml', 'quickformat', 'selectall', '|', 'fullscreen', '/',
                              'formatblock', 'fontname', 'fontsize', '|', 'forecolor', 'hilitecolor', 'bold',
                              'italic', 'underline', 'strikethrough', 'lineheight', 'removeformat', '|', 'image'],
                uploadDoc = option.canUploadDocument,
                uploadImg = option.canUploadImage,
                uploadUrl = option.uploadUrl;

            if (uploadDoc) { toolItems.push('insertfile'); }
            $.merge(toolItems, ['table', 'hr', 'emoticons', 'pagebreak', 'anchor', 'link', 'unlink', '|', 'about']);
            KindEditor.create(commentEditTextarea, {
                width: 200,
                height: 200,
                items: toolItems,
                allowImageUpload: uploadImg,
                uploadJson: uploadUrl
            });
            lastIndex = index;
        }

        $(document).on("click", "#comment-edit-holder-j > .comment-edit-save-j", function () {
            if (lastIndex !== -1 && commentModel !== null) {
                KindEditor.sync(commentEditTextarea);
                commentModel.content(commentEditTextarea.val());
                commentModel.save(closeCommentEditor);
            }
        });

        $(document).on("click", "#comment-edit-holder-j > .comment-edit-cancel-j", function () {
            if (lastIndex !== -1) {
                closeCommentEditor();
            }
        });

    })();

    //Tag Options Changes for thread not used now
    fn.tagOptionChangesOnThread = function (url, topic) {
        var content = topic.content();
        topic.content('');
        var pData = JSON.stringify(topic.toJson());
        $.ajax({
            url: url,
            dataType: "html",
            data: { jsonModel: pData },
            type: "POST",
            success: function (data, status) {
                $("#tagOptionHolder-j").html(data);
            },
            error: function (xhr, status, errorThrown) {
            },
            complete: function () {
                topic.content(content);
            }
        });
    };


});

