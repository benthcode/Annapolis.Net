using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Data.Mapping
{
    public class BaseOwnerEntityTypeConfiguration<TEntity> : BaseEntityTypeConfiguration<TEntity>
        where TEntity : BaseOwnerEntity
    {
        public BaseOwnerEntityTypeConfiguration()
        {
            Ignore(x => x.UserName);
        }
    }
}
