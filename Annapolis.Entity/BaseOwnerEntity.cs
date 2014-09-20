using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Annapolis.Entity
{
    public abstract class BaseOwnerEntity : BaseEntity
    {
        public Guid UserId { get; set; }

        private string _userName;
        public string UserName
        {
            get
            {
                if(_userName == null && User!=null) { _userName = User.UserName;}
                return _userName;
            }
            set
            {
                _userName = value;
            }
        }
        public virtual MemberUser User { get; set; }
    }

}
