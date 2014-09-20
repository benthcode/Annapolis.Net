using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Entity
{
    public class UploadFileCategory : BaseEntity
    {
        public string Name { get; set; } 

        public ICollection<UploadFile> Files { get; set; }
    }
}
