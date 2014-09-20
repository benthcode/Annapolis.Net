using System;

namespace Annapolis.Entity
{

    public abstract class BaseEntity
    {
        public Guid Id { get; set; }

        public string Key { get; set; }

        public override bool Equals(object obj)
        {
            if(obj == null || !(obj is BaseEntity)) return false;
            return Id.Equals(((BaseEntity)obj).Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public virtual bool IsNew()
        {
            return Id == Guid.Empty;
        }

        public virtual void GenerateId()
        {
            Id = Guid.NewGuid();
        }

        public virtual void ResetId()
        {
            Id = Guid.Empty;
        }
    }

}
