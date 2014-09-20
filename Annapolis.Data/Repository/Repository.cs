using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Abstract.Repository;
using Annapolis.Entity;
using System.Linq.Expressions;
using System.Data;
using System.Data.Common;
using Annapolis.Abstract;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Collections;

namespace Annapolis.Data.Repository
{
    public class Repository<T> : IRepository<T> where T: BaseEntity
    {
       

        public IAnnapolisDbContext DbContext { get; set; }

        public Repository(IAnnapolisDbContext context)
        {
            DbContext = context;
        }

        public virtual T Single(Expression<Func<T, bool>> expression)
        {
            return All.FirstOrDefault(expression);
        }

        public virtual bool Contains(T item)
        {
            return All.Contains(item);
        }

        public virtual bool Contains(Expression<Func<T, bool>> predicate)
        {
            return All.Count<T>(predicate) > 0;
        }

        public virtual IQueryable<T> All
        {
            get { return DbContext.Set<T>().AsQueryable(); }
        }

        public virtual T Get(Guid id)
        {
            return Single(x => x.Id == id);
        }

        public virtual T Add(T item)
        {
            item.GenerateId();
            var newEntry = DbContext.Set<T>().Add(item);
            return newEntry;
        }

        public virtual T Update(T item)
        {
            DbContext.Entry(item).State = EntityState.Modified;
            return item;
        }

        public virtual T Delete(Guid id)
        {
            var item = All.Where(t => t.Id == id).SingleOrDefault();
            if (item != null)
            {
                item =  DbContext.Set<T>().Remove(item);
            }
            return item;
        }

        public virtual T Delete(T item)
        {
            T delteItem = DbContext.Set<T>().Remove(item);
            return delteItem;
        }

        public virtual void Delete(Expression<Func<T, bool>> predicate)
        {
            var objects = All.Where(predicate);
            foreach (var obj in objects)
            {
                DbContext.Set<T>().Remove(obj);
            }
        }

        public virtual void Detach(T item)
        {
            var entry = DbContext.Entry(item);
            DbContext.ObjectContext.Detach(item);
        }

        public virtual void Detach(Expression<Func<T, bool>> predicate)
        {
            var objects = All.Where(predicate);
            foreach (var obj in objects)
            {
                DbContext.ObjectContext.Detach(obj);
            }
        }

        public virtual void Attach(T item)
        {
            var entry = DbContext.Entry(item);
            DbContext.Set<T>().Attach(item);
        }

        public virtual int SaveChanges()
        {
            return DbContext.SaveChanges();
        }

        public virtual void ExecuteProcedure(String procedureCommand, params DbParameter[] sqlParams)
        {
            DbContext.Database.ExecuteSqlCommand(procedureCommand, sqlParams);
            
        }

        public virtual IEnumerable<TEelement> SqlQuery<TEelement>(string sql, params object[] parameters)
        {
            return DbContext.Database.SqlQuery<TEelement>(sql, parameters);
        }

    }
}
