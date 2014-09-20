using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Abstract.Work
{
    public interface IAnnapolisBaseCacheCrudWork<T> : IAnnapolisBaseCrudWork<T> where T : class
    {
        List<T> AllCacheItems { get; }
        Dictionary<Guid, T> AllDictionary { get; }
    }
}
