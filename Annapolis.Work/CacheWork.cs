using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Abstract.Work;
using System.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;

namespace Annapolis.Work
{

    public class CacheWork :AnnapolisBaseWork, ICacheWork
    {
        

        private static Dictionary<string, object> _cacheManager;

        static CacheWork()
        {
            try
            {
              
                _cacheManager = new Dictionary<string, object>();
                
            }
            catch (Exception ex)
            {
                throw new Exception("Error fetch CacheManager. " + ex.Message);
            }
        }

        public CacheWork()
        {
            
        }

        public bool Contains(string key)
        {
            return _cacheManager.ContainsKey(key);
        }

        public bool ExistsObject(string key)
        {
            return _cacheManager.ContainsKey(key) && (_cacheManager[key] != null);
        }

        public void AddOrUpdate(string key, object data)
        {
            if (_cacheManager.ContainsKey(key))
            {
                _cacheManager.Remove(key);
            }
            _cacheManager.Add(key, data);
        }

        public T GetData<T>(string key) where T : class
        {
            if (!_cacheManager.ContainsKey(key))
            {
                return null;
            }
            else
            {
                return _cacheManager[key] as T;
            }
        }

        public void Remove(string key)
        {
            _cacheManager.Remove(key);
        }

        public void Flush()
        {
            _cacheManager.Clear();
        }


    }
}
