using Newtonsoft.Json;
namespace Annapolis.Web.Client
{
    public interface IClientModel
    {
        string ToJson(JsonSerializerSettings setting = null);
        string TargetJsonModelNameSpace { get; }
        string TargetJsonModelName { get; }

        string UniqueId { get; set; }

        string ServerActionKey { get; set; }
    }
}
