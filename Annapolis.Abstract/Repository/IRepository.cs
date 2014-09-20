using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;
using System.Linq.Expressions;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Annapolis.Abstract.Repository
{
    public interface IRepository<T> : IAnnapolisBase where T : BaseEntity
    {

      

        T Single(Expression<Func<T, bool>> expression);

        bool Contains(T item);

        bool Contains(Expression<Func<T, bool>> predicate);

        IQueryable<T> All { get; }

        T Get(Guid id);

        T Add(T item);

        T Update(T item);

        T Delete(T item);

        T Delete(Guid id);

        void Delete(Expression<Func<T, bool>> predicate);

        void Detach(T item);

        void Detach(Expression<Func<T, bool>> predicate);

        void Attach(T item);
       
        int SaveChanges();

        void ExecuteProcedure(String procedureCommand, params DbParameter[] sqlParams);

        IEnumerable<TEelement> SqlQuery<TEelement>(string sql, params object[] parameters);

        IAnnapolisDbContext DbContext { get; set; }
    }
}
