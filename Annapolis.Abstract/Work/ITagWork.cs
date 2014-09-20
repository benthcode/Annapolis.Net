using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Abstract.Work
{
    public interface ITagWork : IAnnapolisBaseCacheCrudWork<ContentTag>
    {
        List<ContentTag> GetSubTags(Guid tagId, int topLevel = 0, int bottomLevel = int.MaxValue);
        List<ContentTag> GetTagsByCategory(ContentTagCategory tagCategory, int depth = int.MaxValue);
    }
}
