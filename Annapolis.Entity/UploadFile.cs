using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Entity
{
    public class UploadFile : BaseOwnerEntity
    {
        public string FilePath { get; set; }
        public DateTime UploadDate { get; set; }

        public Guid? CategoryId { get; set; }
        public UploadFileCategory Category { get; set; }

        public Guid? AttachId { get; set; } 

        public string FriendFileName
        {
            get 
            {
                string friendFileName = FilePath;
                int firstIndex = friendFileName.IndexOf('_');
                if(firstIndex >= 0)
                {
                    friendFileName = friendFileName.Substring(firstIndex+1);    
                }
                return friendFileName; 
            }
        }

    }
}
