using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Abstract.Work
{
    public interface ICacheWork : IAnnapolisBaseWork
    {
        bool Contains(string key);
        bool ExistsObject(string key);
        void AddOrUpdate(string key, object data);
        T GetData<T>(string key) where T : class;
        void Remove(string key);
        void Flush();

    }
}
