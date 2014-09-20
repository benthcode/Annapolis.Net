using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using Annapolis.Entity;
using Annapolis.Abstract.Work;
using Annapolis.Abstract.Repository;

namespace Annapolis.Work
{
    public class PermissionWork : AnnapolisBaseCacheCrudWork<PermissionOnThread>, IPermissionWork
    {

        private readonly IRepository<Permission> _permissionRepository;
        private readonly IThreadWork _threadWork;

        public PermissionWork(IRepository<Permission> permissionRepository, IThreadWork threadWork)
        {
            _permissionRepository = permissionRepository;
            _threadWork = threadWork;
        }

        private Dictionary<Guid, Dictionary<Guid, Dictionary<string, bool>>> PermissionTickets
        { 
            get
            {
                if (!CacheManager.Contains("PermissionService_PermissionTickets"))
                {
                    List<PermissionOnThread> permissionSet = All.Include(x => x.Role)
                                                .Include(x => x.Thread)
                                                .Include(x => x.Permission)
                                                .OrderBy(x => x.Thread.Id)
                                                .ThenBy(x => x.Role.Id)
                                                .ThenBy(x => x.Permission.Id)
                                                .ToList();

                    Dictionary<Guid, Dictionary<Guid, Dictionary<string, bool>>> dict = new Dictionary<Guid,Dictionary<Guid,Dictionary<string,bool>>>();
                    foreach (var permissionOnThread in permissionSet)
                    {
                        if (!dict.ContainsKey(permissionOnThread.ThreadId))
                        {
                            dict.Add(permissionOnThread.ThreadId, new Dictionary<Guid, Dictionary<string, bool>>());
                        }
                        var roleDict = dict[permissionOnThread.ThreadId];

                        if (!roleDict.ContainsKey(permissionOnThread.RoleId))
                        {
                            roleDict.Add(permissionOnThread.RoleId, new Dictionary<string, bool>());
                        }
                        var permissionDict = roleDict[permissionOnThread.RoleId];

                        permissionDict.Add(permissionOnThread.Permission.Name, permissionOnThread.IsGranted);

                    }

                    CacheManager.AddOrUpdate("PermissionService_PermissionTickets", dict);
                }
                return CacheManager.GetData<Dictionary<Guid, Dictionary<Guid, Dictionary<string, bool>>>>("PermissionService_PermissionTickets");
            }
        }


    
        public bool IsPermissionGranted(MemberRole role, Permission permission, ContentThread thread = null)
        {
            if (thread == null)
            {
                return IsPermissionGranted(role.Id, permission.Name);
            }
            else
            {
                return IsPermissionGranted(role.Id, permission.Name, thread.Id);
            }
        }

        public bool IsPermissionGranted(Guid roleId, string permissionName, Guid? contentThreadId = null)
        {
            Guid threadId;
            if (contentThreadId.HasValue)
            {
                threadId = contentThreadId.Value;
                while (!PermissionTickets.ContainsKey(threadId))
                {
                    ContentThread thread = _threadWork.GetThread(threadId);
                    if (thread == null || thread.ParentThreadId == null) break;
                    threadId = thread.ParentThreadId.Value;
                }
            }
            else
            {
                threadId = _threadWork.RootThread.Id;
            }

            if (PermissionTickets.ContainsKey(threadId))
            {
                var roleDict = PermissionTickets[threadId];
                if (roleDict.ContainsKey(roleId))
                {
                    var permissionDict = roleDict[roleId];
                    if (permissionDict.ContainsKey(permissionName))
                    {
                        return permissionDict[permissionName];
                    }
                }
            }

            return false;
        }
       
    }
}
