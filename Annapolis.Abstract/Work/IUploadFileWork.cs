﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Abstract.Work
{
    public interface IUploadFileWork : IAnnapolisBaseCrudWork<UploadFile>
    {
        UploadFile Create(string filePath);
    }
}
