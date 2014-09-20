using Annapolis.Entity;
using Annapolis.WebSite.ClientModels;
using Annapolis.WebSite.Drivers.Base;

namespace Annapolis.WebSite.Drivers.Abstract
{
    public interface ITagDriver : IEntityConvertableDriver<ContentTag, TagClient>,
        ISavableDriver<ContentTag, TagClient>
    {
        TagListClient GetTagsByCategory(string categoryName, bool onlyHotTag = false, bool includeAll = false, bool includeOther = false);
    }
}