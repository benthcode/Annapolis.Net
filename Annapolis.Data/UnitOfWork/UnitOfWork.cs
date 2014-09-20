using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Abstract.UnitOfWork;
using System.Data;
using System.Data.Objects;
using System.Data.Entity.Infrastructure;

namespace Annapolis.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
      
        private readonly AnnapolisDbContext _context;
        private readonly IDbTransaction _transaction;
        private readonly ObjectContext _objectContext;

        /// <summary>
        /// Constructor
        /// </summary>
        public UnitOfWork(AnnapolisDbContext context)
        {
            _context = context;

            _objectContext = ((IObjectContextAdapter)_context).ObjectContext;

            if (_objectContext.Connection.State != ConnectionState.Open)
            {
                _objectContext.Connection.Open();
                _transaction = _objectContext.Connection.BeginTransaction();
            }
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void Commit()
        {
            _context.SaveChanges();
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();

            foreach (var entry in _context.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Unchanged;
                        break;
                }
            }
        }

        public void Dispose()
        {
            if (_objectContext.Connection.State != ConnectionState.Closed)
            {
                _objectContext.Connection.Close();
            }
        }
    }
}
