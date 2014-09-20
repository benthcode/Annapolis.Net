using System;
using System.Collections.Generic;
using Annapolis.Abstract.Work;
using Annapolis.Entity;
using Annapolis.WebSite.ViewModels;
using Annapolis.Web.Client;

namespace Annapolis.WebSite.Drivers.Base
{

    public abstract class ConvertableDriver<VM, CM> : BaseDriver, IConvertableDriver<VM,CM>
        where VM : ViewModel, new()
        where CM : ClientModel, new()
    {

        public abstract VM Create(string[] includeProperties = null);

        public virtual CM CreateClient(string[] includeProperties = null)
        {
            return new CM();
        }

        public abstract VM FromClient(VM m, CM c, string[] includeProperties = null);

        public virtual CM ToClient(VM m, CM c = null, string[] includeProperties = null, bool serverStatus = true)
        {
            if (c == null) return new CM();
            return c;
        }

        protected virtual ModelListClient<CM> ToClientCollection(ICollection<VM> collection)
        {
            if (collection == null) return null;
            ModelListClient<CM> list = new ModelListClient<CM>();
            foreach (VM m in collection)
            {
                list.Add(ToClient(m));
            }
            return list;
        }
    }

    public abstract class ConvertableEntityDriver<E, CM> : EntityDriver, IEntityConvertableDriver<E, CM>
        where E : BaseEntity
        where CM : IdenticalClientModel, new()
    {
        protected IAnnapolisBaseCrudWork<E> CrudWork;

        public ConvertableEntityDriver(IAnnapolisBaseCrudWork<E> work)
        {
            CrudWork = work;
        }

        public virtual E Create(string[] includeProperties = null)
        {
            return CrudWork.Create();
        }

        public virtual E Get(Guid id, string[] includeProperties = null)
        {
            return CrudWork.Get(id, includeProperties);
        }

        public virtual CM CreateClient(string[] includeProperties = null)
        {
            return new CM();
        }

        public abstract E FromClient(E m, CM c, string[] includeProperties = null);

        public virtual CM ToClient(E m, CM c = null, string[] excludeProperties = null, bool serverStatus = true)
        {
            if (c == null) { 
                c = new CM();
                c.Id = m.Id;
            }
            c.ServerStatus = serverStatus;

            return c;
        }

        protected virtual IList<CM> ToClientCollection(IList<E> collection)
        {
            if (collection == null) return null;
            List<CM> list = new List<CM>();
            foreach (E entity in collection)
            {
                list.Add(ToClient(entity));
            }
            return list;
        }
    }
}