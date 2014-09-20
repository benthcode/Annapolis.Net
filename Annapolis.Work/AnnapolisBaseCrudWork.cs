using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Abstract.Work;
using Annapolis.Entity;
using Annapolis.Abstract.Repository;
using Annapolis.Data.Repository;
using Annapolis.Abstract;
using Annapolis.Shared.Extension;
using Annapolis.Shared.Model;
using Microsoft.Practices.Unity;
using System.Linq.Expressions;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using LinqDynamicKit;
using Annapolis.Abstract.UnitOfWork;
using PagedList;


namespace Annapolis.Work
{
    public abstract class AnnapolisBaseCrudWork<T> :AnnapolisBaseWork, IAnnapolisBaseCrudWork<T> where T : BaseEntity, new()
    {
        [Dependency]
        public IRepository<T> Repository {get; set;}


        [Dependency]
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }

        protected static Dictionary<EntityPermission, string> PermissionNameDictionary;

        static AnnapolisBaseCrudWork()
        {
            PermissionNameDictionary = new Dictionary<EntityPermission, string>();
        }

        protected IQueryable<T> AddIncludeSelector(IQueryable<T> source, string[] includeSelectors)
        {
            IQueryable<T> query = source;
            if (includeSelectors != null && includeSelectors.Length > 0)
            {
                foreach (var selector in includeSelectors)
                {
                    query.Include(selector.Trim());
                }
            }
            return query.AsExpandable(); 
        }

        protected virtual IQueryable<T> AddAlwaysIncludeSelector(IQueryable<T> source)
        {
            return source;
        }

        protected virtual IQueryable<T> AddAlwaysPredication(IQueryable<T> source)
        {
            return source;
        }

        #region Query

        //http://www.albahari.com/nutshell/predicatebuilder.aspx


        public virtual IQueryable<T> Query(Expression<Func<T, bool>> predicate, string orderBy = null, string[] includeSelectors = null)
        {
            var query = AddIncludeSelector(All, includeSelectors);
            
            query = AddAlwaysPredication(query);

            if (predicate != null) { query = query.Where(predicate); }
            
            if (!string.IsNullOrEmpty(orderBy)) { query = query.OrderBy(orderBy); }
           
            return query;
        }


        public virtual IQueryable<T> Query(string predicate, string orderBy = null, string[] includeSelectors = null)
        {
            var query = AddIncludeSelector(All, includeSelectors);

            query = AddAlwaysPredication(query);

            if (predicate != null) { query = query.Where(predicate); }

            if (!string.IsNullOrEmpty(orderBy)) { query = query.OrderBy(orderBy); }

            return query;
        }

        #endregion

        #region Page List


        public virtual IPagedList<T> Paging(int pageNumber, int pageSize, string orderBy, Expression<Func<T, bool>> predicate, string[] includeSelectors = null)
        {
            var query = Query(predicate, orderBy, includeSelectors);
            return query.ToPagedList(pageNumber, pageSize);
        }

        public virtual IPagedList<T> Paging(int pageNumber, int pageSize, string orderBy, string predicate = null, string[] includeSelectors = null)
        {
            var query = Query(predicate, orderBy, includeSelectors);
            return query.ToPagedList(pageNumber, pageSize);
        }

        #endregion

        #region Basic 

        public virtual IQueryable<T> All
        {
            get
            {
                return Repository.All;
            }
        }

        public virtual T Single(Expression<Func<T, bool>> expression, string[] includeSelectors = null)
        {
            var query = AddIncludeSelector(All, includeSelectors);
            return query.Where(expression).SingleOrDefault();
        }

        public virtual bool Contains(T item)
        {
            return Repository.Contains(item);
        }

        public virtual bool Contains(Expression<Func<T, bool>> predicate)
        {
            return Repository.Contains(predicate);
        }

        #endregion

        #region CRUD

        public virtual T Create()
        {
            return new T();
        }

        public virtual T Get(Guid id, string[] includeSelectors = null)
        {
            var query = AddIncludeSelector(All, includeSelectors);
            return query.Where(x => x.Id == id).SingleOrDefault();
        }

        protected virtual void OnSaving(T item, bool checkPermission)
        {
        }

        protected virtual void OnSaved(T item, OperationStatus status, bool checkPermission)
        {
        }

        public virtual OperationStatus Save(T item, bool checkPermission = true)
        {
            if (item == null) { throw new NullReferenceException("Try to save null value"); }

            OperationStatus status = OperationStatus.None;
            try
            {
                OnSaving(item, checkPermission);
                if (item.IsNew())
                {
                    OperationStatus permissonStatus = OperationStatus.NoPermission;
                    if (checkPermission) { permissonStatus = HasPermission(EntityPermission.Add, item); }
                    if (!checkPermission || permissonStatus == OperationStatus.Granted)
                    {
                        status = Add(item);
                        if (status == OperationStatus.Success)
                        {
                            MarkPersistentDataChanged();
                        }
                    }
                    else
                    {
                        return permissonStatus;
                    }
                }
                else
                {
                    OperationStatus permissonStatus = OperationStatus.NoPermission;
                    if (checkPermission) { permissonStatus = HasPermission(EntityPermission.Update, item); }
                    if (!checkPermission || permissonStatus == OperationStatus.Granted)
                    {
                        status = Update(item);
                        if (status == OperationStatus.Success)
                        {
                            MarkPersistentDataChanged();
                        }
                    }
                    else
                    {
                        return permissonStatus;
                    }
                }
            }
            catch
            {
                status =  OperationStatus.GenericError;
            }
            finally
            {
                OnSaved(item, status, checkPermission);
            }

            return status;
           
        }

        protected virtual OperationStatus Add(T item)
        {
            using (var worker = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    AddMark(item);
                    worker.Commit();
                    return OperationStatus.Success;
                }
                catch
                {
                    item.ResetId();
                    worker.Rollback();
                    return OperationStatus.GenericError;
                }
            }
        }

        protected virtual OperationStatus Update(T item)
        {
            using (var worker = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    UpdateMark(item);
                    worker.Commit();
                    return OperationStatus.Success;
                }
                catch
                {
                    worker.Rollback();
                    return OperationStatus.GenericError;
                }
            }
        }

        public virtual OperationStatus Delete(Guid id)
        {
            T item = Get(id);
            if (item != null)
            {
                return Delete(item);
            }
            else
            {
                return OperationStatus.EntityNotExists;
            }
        }

        public virtual OperationStatus Delete(T item)
        {
            var permissionStatus = HasPermission(EntityPermission.Delete, item);
            if (permissionStatus == OperationStatus.Granted)
            {
                using (var worker = UnitOfWorkManager.NewUnitOfWork())
                {
                    try
                    {
                        Repository.Delete(item);
                        MarkPersistentDataChanged();
                        worker.Commit();
                        return OperationStatus.Success;
                    }
                    catch
                    {
                        worker.Rollback();
                        return OperationStatus.GenericError;
                    }
                }
            }
            else
            {
                return permissionStatus;
            }
        }

        public OperationStatus SaveMark(T item, bool checkPermission = true)
        {
            if (item == null) { throw new NullReferenceException("Try to save null value"); }

            OperationStatus status = OperationStatus.None;
            try
            {
                OnSaving(item, checkPermission);
                if (item.IsNew())
                {
                    OperationStatus permissonStatus = OperationStatus.NoPermission;
                    if (checkPermission) { permissonStatus = HasPermission(EntityPermission.Add, item); }
                    if (!checkPermission || permissonStatus == OperationStatus.Granted)
                    {
                        status = AddMark(item);
                        if (status == OperationStatus.Success)
                        {
                            MarkPersistentDataChanged();
                        }
                    }
                    else
                    {
                        return permissonStatus;
                    }
                }
                else
                {
                    OperationStatus permissonStatus = OperationStatus.NoPermission;
                    if (checkPermission) { permissonStatus = HasPermission(EntityPermission.Update, item); }
                    if (!checkPermission || permissonStatus == OperationStatus.Granted)
                    {
                        status = UpdateMark(item);
                        if (status == OperationStatus.Success)
                        {
                            MarkPersistentDataChanged();
                        }
                    }
                    else
                    {
                        return permissonStatus;
                    }
                }
            }
            catch
            {
                status = OperationStatus.GenericError;
            }
            finally
            {
                OnSaved(item, status, checkPermission);
            }

            return status;

        }

        protected virtual OperationStatus AddMark(T item)
        {
            try
            {
                Repository.Add(item);
                return OperationStatus.Success;
            }
            catch
            {
                item.ResetId();
                return OperationStatus.GenericError;
            }
          
        }

        protected virtual OperationStatus UpdateMark(T item)
        {
            try
            {
                Repository.Update(item);
                return OperationStatus.Success;
            }
            catch
            {
                return OperationStatus.GenericError;
            }
        }

        public OperationStatus DeleteMark(Guid id)
        {
            T item = Get(id);
            if (item != null)
            {
                return DeleteMark(item);
            }
            else
            {
                return OperationStatus.EntityNotExists;
            }
        }

        public OperationStatus DeleteMark(T item)
        {
            var permissionStatus = HasPermission(EntityPermission.Delete, item);
            if (permissionStatus == OperationStatus.Granted)
            {
                try
                {
                    Repository.Delete(item);
                    return OperationStatus.Success;
                }
                catch
                {
                    return OperationStatus.GenericError;
                }                
            }
            else
            {
                return permissionStatus;
            }
        }

        protected virtual void MarkPersistentDataChanged() { }

        #endregion

        #region Permission

        public virtual OperationStatus HasPermission(EntityPermission permission, T item = null, Guid? threadId = null)
        {
            return OperationStatus.Granted;
        }

        #endregion

        public virtual void ExecuteProcedure(string procedureCommand, params DbParameter[] sqlParams)
        {
            Repository.ExecuteProcedure(procedureCommand, sqlParams);
        }

        public IEnumerable<TEelement> SqlQuery<TEelement>(string sql, params object[] parameters)
        {
            return Repository.SqlQuery<TEelement>(sql, parameters);
        }

    }
}
