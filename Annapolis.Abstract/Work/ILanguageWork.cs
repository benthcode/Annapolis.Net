using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Abstract.Work
{
    public interface ILanguageWork : IAnnapolisBaseCacheCrudWork<LocaleLanguage>
    {
        List<LocaleLanguage> AllLanguageCacheItems { get; }

        List<LocaleResourceKey> AllResourceKeyCacheItems { get; }

        List<LocaleResourceValue> AllResourceValueCacheItems { get; }

        LocaleResourceValue GetResourceValue(LocaleLanguage language, LocaleResourceKey resourceKey);
        LocaleResourceValue GetResourceValue(Guid languageId, Guid resourceKeyId);
    }
}
