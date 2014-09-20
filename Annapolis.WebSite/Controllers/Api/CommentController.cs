using System.Web.Http;
using System.Web.Mvc;
using Annapolis.Manager;
using Annapolis.Shared.Model;
using Annapolis.WebSite.Application.Base;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Abstract;
using Annapolis.Web.Client;
using Annapolis.WebSite.Application.Attribute;

namespace Annapolis.WebSite.Controllers.Api
{
   
    public class CommentController : BaseApiController
    {
        private readonly ICommentDriver _commentDriver;

        public CommentController()
        {
            _commentDriver = DependencyResolver.Current.GetService<ICommentDriver>();
        }

        private void SaveComment(CommentClient comment)
        {
            var status = _commentDriver.Save(comment);
            if (status == OperationStatus.Success)
            {
                comment.AddSuccessNotification(MessageManager.GetMessage(status));
            }
            else
            {
                comment.AddErrorNotification(MessageManager.GetMessage(status));
            }
        }

        [System.Web.Http.HttpPost]
        [AnnaApiAuthorize]
        public ClientModel Create([FromBody]CommentClient comment)
        {
            SaveComment(comment);
            comment.ServerActionKey = "Save";
            comment.ServerStatus = true;
            return comment;
        }

        [System.Web.Http.HttpPut]
        [AnnaApiAuthorize]
        public ClientModel Update(string key, [FromBody]CommentClient comment)
        {
            SaveComment(comment);
            comment.ServerActionKey = "Save";
            comment.ServerStatus = true;
            return comment;
        }

        [System.Web.Http.HttpPost]
        [AnnaApiAuthorize]
        public ClientModel CheckUploadPermissionForNewComment([FromBody]CommentClient comment)
        {
            comment.ClearNotifications();
            comment.CanUploadDocument = _commentDriver.HasPermission(EntityPermission.UploadDocument, threadId: comment.ThreadId) == OperationStatus.Granted;
            comment.CanUploadImage = _commentDriver.HasPermission(EntityPermission.UploadImage, threadId: comment.ThreadId) == OperationStatus.Granted;
            comment.ServerActionKey = "CheckUploadPermission";
            comment.ServerStatus = true;
            return comment;
        }

        [System.Web.Http.HttpDelete]
        [AnnaApiAuthorize]
        public void Delete(string key)
        {
        }
    }
}
