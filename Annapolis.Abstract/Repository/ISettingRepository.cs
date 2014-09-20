using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Annapolis.Entity;

namespace Annapolis.Abstract.Repository
{
    public interface ISettingRepository  : IAnnapolisBase, IRepository<Setting>
    {
        Setting GetDefaultSetting();
    }
}
