using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Abstract.UnitOfWork
{
    public interface IUnitOfWorkManager : IAnnapolisBase, IDisposable
    {
        IUnitOfWork NewUnitOfWork();       
    }
}
