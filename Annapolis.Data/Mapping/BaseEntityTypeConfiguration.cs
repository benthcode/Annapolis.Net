using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using Annapolis.Entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Annapolis.Data.Mapping
{
    public class BaseEntityTypeConfiguration<TEntity> : 
        EntityTypeConfiguration<TEntity> where TEntity: BaseEntity
    {
        public BaseEntityTypeConfiguration()
        {
            HasKey(x => x.Id);
            Property(x => x.Key).HasMaxLength(64);
       
        }
    }
}
