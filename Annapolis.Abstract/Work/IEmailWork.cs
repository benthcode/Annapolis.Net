using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;
using Annapolis.Shared.Model;

namespace Annapolis.Abstract.Work
{
    public interface IEmailWork : IAnnapolisBaseWork
    {
        void SendMail(Email email);
        void SendMail(List<Email> email);
    }
}
