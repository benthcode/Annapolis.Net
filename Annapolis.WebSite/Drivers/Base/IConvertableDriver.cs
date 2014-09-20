using System;
using Annapolis.Entity;
using Annapolis.Web.Client;

namespace Annapolis.WebSite.Drivers.Base
{
    public interface IConvertableDriver<M, CM> : IBaseDriver
        where M : class
        where CM : ClientModel
    {
        M Create(string[] includeProperties = null);

        CM CreateClient(string[] includeProperties = null);

        M FromClient(M m, CM c, string[] includeProperties = null);

        CM ToClient(M m, CM c = null, string[] excludeProperties = null, bool serverStatus = true);
    }

    public interface IEntityConvertableDriver<M, CM> : IConvertableDriver<M,CM>
        where M : BaseEntity
        where CM : IdenticalClientModel
    {
        M Get(Guid id, string[] includeProperties = null);
    }
}
