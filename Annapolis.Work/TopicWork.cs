using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;
using Annapolis.Abstract.Work;
using Annapolis.Shared.Model;
using Annapolis.Abstract.Repository;
using System.Data.Entity;
using Annapolis.Shared.Extension;

namespace Annapolis.Work
{
    public class TopicWork : AnnapolisBaseOwnerCrudWork<ContentTopic>, ITopicWork
    {
        private readonly ICommentWork _commentWork;
        private readonly IBannedWordWork _bannedWordWork;
        private readonly ISettingWork _settingWork;
        private readonly IThreadWork _threadWork;
        private readonly Setting _defaultSetting;

        static TopicWork()
        {

            PermissionNameDictionary.Add(EntityPermission.Read, PermissionConstants.Topic_Read);
            PermissionNameDictionary.Add(EntityPermission.Add, PermissionConstants.Topic_Add);
            PermissionNameDictionary.Add(EntityPermission.Update, PermissionConstants.Topic_Update);
            PermissionNameDictionary.Add(EntityPermission.Delete, PermissionConstants.Topic_Delete);
            PermissionNameDictionary.Add(EntityPermission.Vote, PermissionConstants.Topic_Vote);

            PermissionNameDictionary.Add(EntityPermission.UploadDocumentRead, PermissionConstants.Topic_Upload_Document_Read);
            PermissionNameDictionary.Add(EntityPermission.UploadDocument, PermissionConstants.Topic_Upload_Document);
            PermissionNameDictionary.Add(EntityPermission.UploadImage, PermissionConstants.Topic_Upload_Image);

        }

        public TopicWork(ICommentWork commentWork, IBannedWordWork bannedWordWork, IThreadWork threadWork ,SettingWork settingWork)
        {
            _commentWork = commentWork;
            _bannedWordWork = bannedWordWork;
            _threadWork = threadWork;
            _settingWork = settingWork;

            _defaultSetting = _settingWork.GetDefaultSetting();
        }

        public override IQueryable<ContentTopic> All
        {
            get
            {
                return base.All.Include(x => x.FirstComment);
            }
        }

        public override ContentTopic Create()
        {
            var topic = base.Create();
            topic.With(x => {
                x.IsLocked = false;
                x.IsHidden = false;
                x.IsSticky = false;
                x.Title = string.Empty;
                x.Thumbnail = _defaultSetting.DefaultTopicThumbnailFile;
            });

            var comment = _commentWork.Create();
            topic.FirstComment = comment;
           
            return topic;
        }

        protected override void OnSaving(ContentTopic item, bool checkPermission)
        {
            item.LastUpdateTime = DateTime.UtcNow;
            item.OriginalTitle = item.Title;

            char replaceHolder = _defaultSetting.BannedWordReplaceHolder[0];
            item.Title = _bannedWordWork.ReplaceBannedWord(item.OriginalTitle, replaceHolder);
            item.OriginalSubTitle = item.SubTitle;
            item.SubTitle = _bannedWordWork.ReplaceBannedWord(item.OriginalSubTitle, replaceHolder);
            if (string.IsNullOrWhiteSpace(item.Thumbnail))
            {
                item.Thumbnail = _defaultSetting.DefaultTopicThumbnailFile;
            }
        }

        protected override OperationStatus Add(ContentTopic item)
        {
            try
            {
                item.CreateTime = DateTime.UtcNow;
                item.UserId = Security.CurrentUser.UserId;

                var firstComment = item.FirstComment;
                item.LastComment = null;
                item.FirstComment = null;

                //Save Topic
                using (var worker = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        var topicSaveStatus = AddMark(item);
                        if (topicSaveStatus == OperationStatus.Success)
                        {
                            worker.Commit();
                        }
                        else
                        {
                            throw new Exception("Topic Save Error");
                        }
                    }
                    catch 
                    {
                        item.ResetId();
                        throw new Exception("Topic Save Error");
                    }
                }

                //Save Post
                item.FirstComment = firstComment;
                firstComment.TopicId = item.Id;
                var commentSaveStatus = _commentWork.Save(firstComment);

                if (commentSaveStatus != OperationStatus.Success)
                {
                    throw new Exception("Post Save Error");
                }

                return  OperationStatus.Success;

            }
            catch
            {
                if (!item.IsNew())
                {
                    Delete(item);
                }
                if (item.FirstComment != null && !item.FirstComment.IsNew())
                {
                    _commentWork.Delete(item.FirstComment);
                }
                return OperationStatus.GenericError;
            }

        }

        public override OperationStatus HasPermission(EntityPermission permission,  ContentTopic item = null, Guid? threadId = null)
        {

            if (!Security.IsCurrentUserValid()) return OperationStatus.NoPermission;
            if (IsCurrentAdminUser()) return OperationStatus.Granted;

            if (item != null && permission.IsDataChangePermission())
            {
                var currentThread = _threadWork.GetThread(item.ThreadId);
                if (item.IsHidden || currentThread.IsHidden) { return OperationStatus.TopicHasHidden; }
                if (item.IsLocked || currentThread.IsLocked) { return OperationStatus.TopicHasLocked; }
                if (Security.CurrentUser.UserId != item.UserId) { return OperationStatus.NoPermission; }
            }

            if (PermissionNameDictionary.ContainsKey(permission))
            {
                if (item == null)
                {
                    return PermissionWork.IsPermissionGranted(Security.CurrentUser.RoleId, PermissionNameDictionary[permission]) ?
                        OperationStatus.Granted : OperationStatus.NoPermission;
                }
                else
                {
                    return PermissionWork.IsPermissionGranted(Security.CurrentUser.RoleId, PermissionNameDictionary[permission], item.ThreadId) ?
                        OperationStatus.Granted : OperationStatus.NoPermission;
                }
            }

            return OperationStatus.NoPermission;
        }
        
    }
}
