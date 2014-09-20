using System;
using Annapolis.Entity;
using Annapolis.Shared.Model;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Base;
using Annapolis.Web.Client;

namespace Annapolis.WebSite.Drivers.Abstract
{
    public interface ICommentDriver : IEntityConvertableDriver<ContentComment, CommentClient>,
        ISavableDriver<ContentComment, CommentClient>
    {
        OperationStatus HasPermission(EntityPermission permission, ContentComment contentComment = null, Guid? threadId = null);

        PageListClient<CommentClient> PagingMyClientComments(int pageNumber, int pageSize, string sort);

    }
}