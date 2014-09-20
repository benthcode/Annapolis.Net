using System;

namespace Annapolis.WebSite.ViewModels
{
    public abstract class  ViewModel
    {
       
    }

    public abstract class IdenticalViewModel : ViewModel
    {
        public abstract Guid Id { get; }

        public virtual bool IsNew()
        {
            return Id == Guid.Empty;
        }
    }

    public abstract class OwnerViewModel : IdenticalViewModel
    {
        public abstract Guid UserId { get; }
        public abstract string UserName { get; }

    }

}