using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Entity
{
    public class ContentBannedWord : BaseEntity
    {
        public string Word { get; set; }
        public bool IsRequiredToCheck { get; set; }
    }
}
