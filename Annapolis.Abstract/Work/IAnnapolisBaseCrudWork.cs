using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Common;
using Annapolis.Shared.Model;
using System.Data.Entity.Infrastructure;
using PagedList;

namespace Annapolis.Abstract.Work
{
    public interface IAnnapolisBaseCrudWork<T> : IAnnapolisBaseWork where T : class
    {
        #region Query

        IQueryable<T> Query(Expression<Func<T, bool>> predicate, string orderBy = null, string[] includeProperties = null);

        IQueryable<T> Query(string predicate, string orderBy = null, string[] includeProperties = null);

        #endregion

        #region Page List

        IPagedList<T> Paging(int pageNumber, int pageSize, string orderBy, Expression<Func<T, bool>> predicate, string[] includeProperties = null);

        IPagedList<T> Paging(int pageNumber, int pageSize, string orderBy, string predicate = null, string[] includeProperties = null);

        #endregion

        #region Basic

        IQueryable<T> All { get; }

        T Single(Expression<Func<T, bool>> expression, string[] includeProperties = null);
        bool Contains(T item);
        bool Contains(Expression<Func<T, bool>> predicate);

        #endregion

        #region CRUD

        T Get(Guid id, string[] includeProperties = null);
        T Create();
        OperationStatus Save(T item, bool checkPermission = true);
        OperationStatus Delete(T item);
        OperationStatus Delete(Guid id);

        OperationStatus SaveMark(T item, bool checkPermission = true);
        OperationStatus DeleteMark(T item);
        OperationStatus DeleteMark(Guid id);

        #endregion

        #region Permission

        ///// <summary>
        ///// Check Item approved, such as if the item is locked, etc, for the user who also created the item
        ///// </summary>
      

        OperationStatus HasPermission(EntityPermission permission, T item = null, Guid? threadId = null);

        #endregion

        void ExecuteProcedure(String procedureCommand, params DbParameter[] sqlParams);

        IEnumerable<TEelement> SqlQuery<TEelement>(string sql, params object[] parameters);
    }

}
