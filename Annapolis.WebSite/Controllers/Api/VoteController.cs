using System.Web.Http;
using System.Web.Mvc;
using Annapolis.Abstract.Work;
using Annapolis.Manager;
using Annapolis.Shared.Model;
using Annapolis.WebSite.Application.Base;
using Annapolis.WebSite.ClientModels;
using Annapolis.Web.Client;
using Annapolis.WebSite.Application.Attribute;

namespace Annapolis.WebSite.Controllers.Api
{
    public class VoteController : BaseApiController
    {

        private readonly IVoteWork _voteWork;

        public VoteController()
        {
            _voteWork = DependencyResolver.Current.GetService<IVoteWork>();
        }

        [System.Web.Http.HttpPost]
        [AnnaApiAuthorize]
        public ClientModel VoteUp([FromBody]VoteClient vote)
        {
            var status = _voteWork.VoteUp(vote.CommentId);
            if (status == OperationStatus.Success)
            {
                vote.VoteUpCount++;
                vote.AddSuccessNotification(MessageManager.GetMessage(status));
            }
            else
            {
                vote.AddErrorNotification(MessageManager.GetMessage(status));
            }
            
            return vote;
        }

        [System.Web.Http.HttpPost]
        [AnnaApiAuthorize]
        public ClientModel VoteDown([FromBody]VoteClient vote)
        {
            var status = _voteWork.VoteDown(vote.CommentId);
            if (status == OperationStatus.Success)
            {
                vote.VoteDownCount++;
                vote.AddSuccessNotification(MessageManager.GetMessage(status));
            }
            else
            {
                vote.AddErrorNotification(MessageManager.GetMessage(status));
            }
            return vote;
        }
    }
}
