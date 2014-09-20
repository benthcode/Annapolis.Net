using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Abstract.UnitOfWork
{
    public interface IUnitOfWork : IAnnapolisBase, IDisposable
    {
        void Commit();
        void Rollback();
        void SaveChanges();
    }
}
