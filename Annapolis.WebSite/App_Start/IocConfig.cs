using Annapolis.IoC;
using Annapolis.WebSite.Drivers;
using Annapolis.WebSite.Drivers.Abstract;
using Microsoft.Practices.Unity;

namespace Annapolis.WebSite.App
{
    public class IocConfig 
    {
        public static void Register(IUnityContainer container)
        {
            container.BindInRequestScope<IAccountDriver, AccountDriver>();
            container.BindInRequestScope<ITopicDriver, TopicDriver>();
            container.BindInRequestScope<ICommentDriver, CommentDriver>();
            container.BindInRequestScope<ITagDriver, TagDriver>();
            container.BindInRequestScope<IThreadDriver, ThreadDriver>();
        }
    }
}
