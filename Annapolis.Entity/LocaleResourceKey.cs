using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Entity
{
    public class LocaleResourceKey : BaseEntity
    {
        public string ResourceKey { get; set; }
        public string Note { get; set; }
        public DateTime DateAdded { get; set; }

        public virtual ICollection<LocaleResourceValue> ResourceValues { get; set; }
    }
}
