using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Annapolis.Entity;
using Annapolis.Abstract.Work;
using Annapolis.Abstract.Repository;
using Annapolis.Shared.Model;


namespace Annapolis.Work
{
    public class LanguageWork : AnnapolisBaseCacheCrudWork<LocaleLanguage>, ILanguageWork
    {
        private readonly IRepository<LocaleResourceKey> _resourceKeyRepository;
        private readonly IRepository<LocaleResourceValue> _resourceValueRepository;

        public LanguageWork( IRepository<LocaleResourceKey> localeResourceKeyRepository,
            IRepository<LocaleResourceValue> localeResourceValueRepository)
        {
            _resourceKeyRepository = localeResourceKeyRepository;
            _resourceValueRepository = localeResourceValueRepository;
        }

        public List<LocaleLanguage> AllLanguageCacheItems
        {
            get 
            {
                if (!CacheManager.Contains("LanguageService_AllLanguageItems"))
                {
                    CacheManager.AddOrUpdate("LanguageService_AllLanguageItems", All.Include(x => x.LocaleValues).ToList());
                }
                return CacheManager.GetData<List<LocaleLanguage>>("LanguageService_AllLanguageItems");
            }
        }

        public List<LocaleResourceKey> AllResourceKeyCacheItems
        {
            get
            {
                if (!CacheManager.Contains("LanguageService_ResourceKeyItems"))
                {
                    CacheManager.AddOrUpdate("LanguageService_ResourceKeyItems",
                                _resourceKeyRepository.All.ToList());
                }
                return CacheManager.GetData<List<LocaleResourceKey>>("LanguageService_ResourceKeyItems");
            }
        }

        public List<LocaleResourceValue> AllResourceValueCacheItems
        {
            get
            {
                if (!CacheManager.Contains("LanguageService_ResourceValueItems"))
                {
                    CacheManager.AddOrUpdate("LanguageService_ResourceValueItems",
                                _resourceValueRepository.All.Include(x => x.Language).Include(x => x.ResourceKey).ToList());
                }
                return CacheManager.GetData<List<LocaleResourceValue>>("LanguageService_ResourceValueItems");
            }
        }

        public override OperationStatus Save(LocaleLanguage language, bool checkPermission = true)
        {
            foreach (var resourceKey in AllResourceKeyCacheItems)
            {
                var localResourceValue = CreateEmptyResourceValue(language.Id, resourceKey.Id);
                _resourceValueRepository.Add(localResourceValue);
            }
            return base.Save(language, true);
        }

        public override LocaleLanguage Create()
        {
            var language = base.Create();
            language.RightToLeft = false;
            return  language;
        }

        //languageId, resourceKey => ResourceValue
        private Dictionary<Guid, Dictionary<Guid, LocaleResourceValue>> ResourceValueDictionary
        {

            get
            {
                Dictionary<Guid, Dictionary<Guid, LocaleResourceValue>> dictionary = null;
                if (!CacheManager.Contains("LanguageService_GetResourceValue"))
                {
                    dictionary = new Dictionary<Guid, Dictionary<Guid, LocaleResourceValue>>();
                    foreach (var resourceValue in AllResourceValueCacheItems)
                    {
                        if (!dictionary.ContainsKey(resourceValue.LanguageId))
                        {
                            dictionary.Add(resourceValue.LanguageId, new Dictionary<Guid, LocaleResourceValue>());
                        }
                        if (!dictionary[resourceValue.LanguageId].ContainsKey(resourceValue.ResourceKeyId))
                        {
                            dictionary[resourceValue.LanguageId].Add(resourceValue.ResourceKeyId, resourceValue);
                        }
                        else
                        {
                            dictionary[resourceValue.LanguageId][resourceValue.ResourceKeyId] = resourceValue;
                        }
                    }
                    CacheManager.AddOrUpdate("LanguageService_GetResourceValue", dictionary);
                }
                dictionary = CacheManager.GetData<Dictionary<Guid, Dictionary<Guid, LocaleResourceValue>>>("LanguageService_GetResourceValue");
                return dictionary;
            }
        }

        private LocaleResourceValue CreateEmptyResourceValue(Guid languageId, Guid resourceKeyId)
        {
            return new LocaleResourceValue() { LanguageId = languageId, ResourceKeyId = resourceKeyId, ResourceValue=string.Empty };
        }

        public LocaleResourceValue GetResourceValue(LocaleLanguage language, LocaleResourceKey resourceKey)
        {
            return GetResourceValue(language.Id, resourceKey.Id);
        }
        
        public LocaleResourceValue GetResourceValue(Guid languageId, Guid resourceKeyId)
        {
           if(ResourceValueDictionary.ContainsKey(languageId)
               && (ResourceValueDictionary[languageId].ContainsKey(resourceKeyId)))
           {
               return ResourceValueDictionary[languageId][resourceKeyId];
           }
           return CreateEmptyResourceValue(languageId, resourceKeyId);
        }

    }
}
