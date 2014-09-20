using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Annapolis.Entity;
using Annapolis.Shared.Model;
using Microsoft.Practices.Unity;
using Annapolis.Abstract.Work;

namespace Annapolis.Work
{
    public abstract class AnnapolisBaseOwnerCrudWork<T> : AnnapolisBaseCrudWork<T> where T : BaseOwnerEntity, new()
    {
        [Dependency]
        public IMemberUserWork MemberUserWork { get; set; }

        [Dependency]
        public IPermissionWork PermissionWork { get; set; }

        [Dependency]
        public ISecurityWorker Security { get; set; }

        protected bool IsCurrentAdminUser()
        {
            var currentUser = Security.CurrentUser;
            if (currentUser == null) return false;
            if (currentUser.IsAdmin) return true;
            return false;
        }


        public override T Create()
        {
       
            T item = base.Create();
            if (Security.CurrentUser != null) 
            {
                item.UserId = Security.CurrentUser.UserId;
                item.UserName = Security.CurrentUser.UserName;
            }
           
            return item;
        }

        public override IQueryable<T> All
        {
            get
            {
                return Repository.All.Include(x => x.User);
            }
        }

        protected override OperationStatus Add(T item)
        {
            item.User = MemberUserWork.Get(item.UserId);
            return base.Add(item);
        }
    }
}
