using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Annapolis.Abstract
{
    public interface IAnnapolisDbContext : IDisposable, IObjectContextAdapter
    {
        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbEntityEntry Entry(object entity);
        Database Database { get; }
        int SaveChanges();
    }
}
