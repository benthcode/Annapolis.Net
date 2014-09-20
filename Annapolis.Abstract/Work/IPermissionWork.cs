using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Abstract.Work
{
    public interface IPermissionWork : IAnnapolisBaseCacheCrudWork<PermissionOnThread>
    {
         //List<Permission> AllPermissionItems { get;}

         //PermissionOnThread GetPermission(MemberRole role, ContentThread thread, Permission permission);
         //PermissionOnThread GetPermission(Guid roleId, Guid categoryId, Guid permissionId);

        bool IsPermissionGranted(MemberRole role, Permission permission, ContentThread thread = null);
        bool IsPermissionGranted(Guid roleId, string permissionName, Guid? contentThreadId = null );

    }
}
