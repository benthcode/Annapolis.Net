using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Abstract.UnitOfWork;
using Annapolis.Abstract;
using System.Data.Entity;

namespace Annapolis.Data.UnitOfWork
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
      
        private readonly AnnapolisDbContext _context;

        public UnitOfWorkManager(IAnnapolisDbContext context)
        {
            
            Database.SetInitializer<AnnapolisDbContext>(null);

            _context = context as AnnapolisDbContext;
        }

        public IUnitOfWork NewUnitOfWork()
        {
            return new UnitOfWork(_context);
        }

      
    }
}
