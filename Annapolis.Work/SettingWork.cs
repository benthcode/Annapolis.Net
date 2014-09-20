using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Annapolis.Entity;
using Annapolis.Abstract.Work;
using Annapolis.Abstract.Repository;
using System.Data.Entity;

namespace Annapolis.Work
{
    public class SettingWork : AnnapolisBaseCacheCrudWork<Setting>, ISettingWork
    {
      
        public Setting GetDefaultSetting()
        {
            if (!CacheManager.Contains("SettingService_GetDefaultSetting"))
            {
                var setting = AllCacheItems.Where(x => x.IsDefault).SingleOrDefault();
                CacheManager.AddOrUpdate("SettingService_GetDefaultSetting", setting);
            }
            return CacheManager.GetData<Setting>("SettingService_GetDefaultSetting");
        }

        public override IQueryable<Setting> All
        {
            get
            {
                return Repository.All
                        .Include(x => x.Language)
                        .Include(x => x.NewMemberStartRole)
                        .Include(x => x.SuperAdminUser);
            }
        }

   
        public override Setting Create()
        {
            var defaultSetting = GetDefaultSetting();

            var setting = base.Create();
            setting.LanguageId = defaultSetting.LanguageId;
            setting.SuperAdminUserId = defaultSetting.SuperAdminUserId;
            setting.NewMemberStartRoleId = defaultSetting.NewMemberStartRole.Id;

            return setting;
        }
       
    }
}
