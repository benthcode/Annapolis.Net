using Annapolis.Abstract.Work;
using Annapolis.Entity;
using Annapolis.Manager;
using Annapolis.Web.Client;
using Annapolis.WebSite.ViewModels;

namespace Annapolis.WebSite.Drivers.Base
{

    public abstract class OwnerableDriver<VM, CM> :  SavableDriver<VM, CM>
        where VM: OwnerViewModel, new()
        where CM : OwnerClientModel, new()
    {

        public override CM ToClient(VM vModel, CM c = null, string[] excludeProperties = null, bool serverStatus = true)
        {
            CM clientModel = base.ToClient(vModel, c, excludeProperties, serverStatus);

            clientModel.UserName = vModel.UserName;

            return clientModel;
        }

    }


    public abstract class OwnerableEntityDriver<E, CM> : SavableEntityDriver<E, CM>
        where E : BaseOwnerEntity
        where CM : OwnerClientModel, new()
    {
        public OwnerableEntityDriver(IAnnapolisBaseCrudWork<E> work)
            : base(work)
        { }

        public override CM CreateClient(string[] includeProperties = null)
        {
            var clientModel = base.CreateClient(includeProperties);
            if (SecurityManager.CurrentUser != null)
            {
                clientModel.UserName = SecurityManager.CurrentUser.UserName;
            }
            return clientModel;
        }

        public override CM ToClient(E entity, CM c = null, string[] excludeProperties = null, bool serverStatus = true)
        {
            if (entity == null) return null;
            CM clientModel = base.ToClient(entity, c, excludeProperties, serverStatus);

            if (clientModel.IsNew())
            {
                if (SecurityManager.CurrentUser != null)
                {
                    clientModel.UserName = SecurityManager.CurrentUser.UserName;
                }
            }
            else
            {
                clientModel.UserName = entity.User.UserName;
            }
            return clientModel;
        }

        public override E FromClient(E m, CM c, string[] includeProperties = null)
        {
            if (m == null || c == null) return null;
            return m;
        }
    }
}