using Annapolis.Shared.Model;
using Annapolis.Web.Client;

namespace Annapolis.WebSite.Drivers.Base
{
    public interface ISavableDriver<M, CM>
        where M : class
        where CM : ClientModel
    {
        OperationStatus Save(CM c, string[] includeProperties = null, string[] excludeProperties = null);

        OperationStatus Save(M m);
    }
}
