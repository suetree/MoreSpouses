using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace SueMoreSpouses.view.setting
{
    class SpouseSettingsGroupVM : ViewModel
    {
  
        public SpouseSettingGroup SettingGroup { get; set; }

        MBBindingList<SpouseSettingsPropertyVM> _settingProperties;

        [DataSourceProperty]
        public MBBindingList<SpouseSettingsPropertyVM> SettingProperties
        {
            get
            {
                return this._settingProperties;
            }
        }

        public SpouseSettingsGroupVM(SpouseSettingGroup settingGroup)
        {
            SettingGroup = settingGroup;
            this._settingProperties = new MBBindingList<SpouseSettingsPropertyVM>();
            List<SpouseSettingsProperty> list = settingGroup.SettingsProperties;
            list.ForEach((obj) => {
                this._settingProperties.Add(new SpouseSettingsPropertyVM(obj));
            });
            this.RefreshValues();
        }
    }
}
