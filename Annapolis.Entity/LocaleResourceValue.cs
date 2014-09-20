using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Entity
{
    public class LocaleResourceValue : BaseEntity
    {
        
        public string ResourceValue { get; set; }
        public string Note { get; set; }

        public Guid LanguageId { get; set; }
        public virtual LocaleLanguage Language { get; set; }

        public Guid ResourceKeyId { get; set; }
        public virtual LocaleResourceKey ResourceKey { get; set; }

    }
}
