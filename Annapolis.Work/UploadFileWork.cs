using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;
using Annapolis.Abstract.Work;

namespace Annapolis.Work
{
    public class UploadFileWork : AnnapolisBaseOwnerCrudWork<UploadFile>, IUploadFileWork
    {
        public override UploadFile Create()
        {
            var file = base.Create();
            file.UserId = Security.CurrentUser.UserId;
            file.UploadDate = DateTime.UtcNow;
            file.GenerateId();
            return file;
        }

        public UploadFile Create(string filePath)
        {
            var file = Create();
            file.FilePath = filePath;
            return file;
        }
    }
}
