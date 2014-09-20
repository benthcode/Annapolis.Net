using Annapolis.Abstract.Work;
using Annapolis.Entity;
using Annapolis.Shared.Model;
using Annapolis.Web.Client;
using Annapolis.WebSite.ViewModels;

namespace Annapolis.WebSite.Drivers.Base
{
    public abstract class SavableDriver<VM, CM> : ConvertableDriver<VM, CM>, ISavableDriver<VM, CM>
        where VM : ViewModel, new()
        where CM : ClientModel, new()
    {

        #region Savable

        public virtual OperationStatus Save(CM c, string[] includeProperties = null, string[] excludeProperties = null)
        {
            try
            {
                if (c == null) return OperationStatus.DataFormatError;
                VM vModel = Create(includeProperties);
                vModel = FromClient(vModel, c);
                OperationStatus status = Save(vModel);
                if (status == OperationStatus.Success)
                {
                    ToClient(vModel, c, excludeProperties);
                }
                else
                {
                    c.ServerStatus = false;
                }
                return status;
            }
            catch
            {
                c.ServerStatus = false;
                return OperationStatus.GenericError;
            }
        }

        public abstract OperationStatus Save(VM vModel);

        #endregion

    }


    public abstract class SavableEntityDriver<E, CM> : ConvertableEntityDriver<E, CM>, ISavableDriver<E, CM>
        where E : BaseEntity
        where CM : IdenticalClientModel, new()
    {
        public SavableEntityDriver(IAnnapolisBaseCrudWork<E> work):base(work)
        { }

        #region Savable

        public virtual OperationStatus Save(CM c, string[] includeProperties = null, string[] excludeProperties = null)
        {
            try
            {
                if (c == null) return OperationStatus.DataFormatError;
                E entity = null;
                if (c.IsNew())
                {
                    entity = Create(includeProperties);
                }
                else
                {
                    entity = Get(c.Id, includeProperties);
                }
                entity = FromClient(entity, c, includeProperties);
                OperationStatus status = Save(entity);
                if (status == OperationStatus.Success)
                {
                    ToClient(entity, c, excludeProperties);
                }
                else
                {
                    c.ServerStatus = false;
                }
                return status;
            }
            catch
            {
                c.ServerStatus = false;
                return OperationStatus.GenericError;
            }
        }

        public virtual OperationStatus Save(E entity)
        {
            return CrudWork.Save(entity);
        }

        #endregion
    }

}