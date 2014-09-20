using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Abstract.Work;
using Annapolis.Entity;
using Annapolis.Abstract.Repository;
using Annapolis.Shared.Model;
using Annapolis.Abstract;
using System.Data.Entity;
using Annapolis.Shared.Extension;
using Annapolis.Shared.Utility;

namespace Annapolis.Work
{
    public class CommentWork : AnnapolisBaseOwnerCrudWork<ContentComment>, ICommentWork
    {
        private readonly IUploadFileWork _uploadWork;
        private readonly IUploadFileCategoryWork _uploadCategoryWork;
        private readonly IBannedWordWork _bannedWordWork;
        private readonly ISettingWork _settingWork;
        private readonly IThreadWork _threadWork;
        private readonly IRepository<ContentTopic> _topicRepository;
        

        private readonly Setting _defaultSetting;

        static CommentWork()
        {
            PermissionNameDictionary.Add(EntityPermission.Read, PermissionConstants.Comment_Read);
            PermissionNameDictionary.Add(EntityPermission.Add, PermissionConstants.Comment_Add);
            PermissionNameDictionary.Add(EntityPermission.Update, PermissionConstants.Comment_Update);
            PermissionNameDictionary.Add(EntityPermission.Delete, PermissionConstants.Comment_Delete);
            PermissionNameDictionary.Add(EntityPermission.Vote, PermissionConstants.Comment_Vote);

            PermissionNameDictionary.Add(EntityPermission.UploadDocumentRead, PermissionConstants.Comment_Upload_Document_Read);
            PermissionNameDictionary.Add(EntityPermission.UploadDocument, PermissionConstants.Comment_Upload_Document);
            PermissionNameDictionary.Add(EntityPermission.UploadImage, PermissionConstants.Comment_Upload_Image);
        
        }

        public CommentWork(IUploadFileWork uploadWork, IUploadFileCategoryWork uploadCategoryWork,
                        IBannedWordWork bannedWordWork, SettingWork settingWork, ThreadWork threadWork, IRepository<ContentTopic> topicRepository)
        {
            _topicRepository = topicRepository;
            _uploadWork = uploadWork;
            _uploadCategoryWork = uploadCategoryWork;
            _bannedWordWork = bannedWordWork;
            _settingWork = settingWork;
            _threadWork = threadWork;

            _defaultSetting = _settingWork.GetDefaultSetting();
        }

        public override ContentComment Create()
        {
            var comment = base.Create();
            comment.With(x =>
            {
                x.Content = string.Empty;
                x.IsLocked = false;
                x.IsSolution = false;
                x.IsSpam = false;
                x.IsHidden = false;
                x.CreateTime = DateTime.UtcNow;
                x.LastUpdateTime = DateTime.UtcNow;
            });

            return comment;
        }

        protected override void OnSaving(ContentComment item, bool checkPermission)
        {
            item.LastUpdateTime = DateTime.UtcNow;
            item.OriginalContent = item.Content;
            item.Content = _bannedWordWork.ReplaceBannedWord(item.OriginalContent, _defaultSetting.BannedWordReplaceHolder[0]);

            if (item.IsNew())
            {
                item.Topic = _topicRepository.All.Include(x => x.Thread).Where(x => x.Id == item.TopicId).SingleOrDefault();
            }
        }

        protected override OperationStatus AddMark(ContentComment item)
        {
            item.CreateTime = DateTime.UtcNow;
            return base.AddMark(item);
        }

        protected override IQueryable<ContentComment> AddAlwaysPredication(IQueryable<ContentComment> source)
        {
            return source.Where(x => !x.IsAttachedToTopic);
        }

        public void ParseContentFile(ContentComment comment, string uploadFileCategory)
        {
            List<string> targetFiles = StringUtility.ParseLinkFiles(comment.Content);
            List<UploadFile> existingFiles = comment.Files.ToList();
            for (int i = existingFiles.Count - 1; i >= 0; i--)
            {
                if (!targetFiles.Contains(existingFiles[i].FilePath))
                {
                    comment.Files.Remove(existingFiles[i]);
                }
            }

            var toAddFiles = targetFiles.Where(x => !existingFiles.Select(f => f.FilePath).Contains(x)).ToList();
            foreach (var fileName in toAddFiles)
            {
                UploadFile file = _uploadWork.Create(fileName);
                file.CategoryId = _uploadCategoryWork.GetFileCategoryByName(uploadFileCategory).Id;
                comment.Files.Add(file);
            }
        }

        public override OperationStatus HasPermission(EntityPermission permission, ContentComment item = null, Guid? threadId = null)
        {
            if (!Security.IsCurrentUserValid()) return OperationStatus.NoPermission;
            if (IsCurrentAdminUser()) return OperationStatus.Granted;

            if (item != null && permission.IsDataChangePermission())
            {
                var currentThread = _threadWork.GetThread(item.Topic.ThreadId);
                if (item.IsHidden || item.Topic.IsHidden || currentThread.IsHidden || currentThread.IsAbstract) { return OperationStatus.CommentHasHidden; }
                if (item.IsLocked || item.Topic.IsLocked || currentThread.IsLocked) { return OperationStatus.CommentHasLocked;}
                if (Security.CurrentUser.UserId != item.UserId) { return OperationStatus.NoPermission; }
            }

            if (PermissionNameDictionary.ContainsKey(permission))
            {
                Guid? tId = null;
                if (item != null && item.Topic != null) { tId = item.Topic.ThreadId; }
                else if (threadId.HasValue) { tId = threadId; }

                if (tId == null)
                {
                    return PermissionWork.IsPermissionGranted(Security.CurrentUser.RoleId, PermissionNameDictionary[permission]) ?
                        OperationStatus.Granted : OperationStatus.NoPermission;
                }
                else
                {
                    return PermissionWork.IsPermissionGranted(Security.CurrentUser.RoleId, PermissionNameDictionary[permission], tId) ?
                        OperationStatus.Granted : OperationStatus.NoPermission;
                }
            }

            return OperationStatus.NoPermission;
        }

    }
}
