using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Annapolis.Entity
{
    public class LocaleLanguage : BaseEntity
    {
        public string Name { get; set; }
        public string Culture { get; set; }

        [DisplayName("Language Flag")]
        public string LanguageImageFileName { get; set; }
        public bool RightToLeft { get; set; }

        public virtual ICollection<Setting> Settings { get; set; }

        public virtual ICollection<LocaleResourceValue> LocaleValues { get; set; }
    }
}
