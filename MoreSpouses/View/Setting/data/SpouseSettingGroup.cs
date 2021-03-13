using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SueMoreSpouses.view.setting
{
    class SpouseSettingGroup
    {
        public String DisplayName { get; set; }

        public List<SpouseSettingsProperty> SettingsProperties { get; set; }

        public SpouseSettingGroup(string displayName)
        {
            DisplayName = displayName;
        }

        public SpouseSettingGroup AddSettingsProperty(SpouseSettingsProperty property)
        {
            if (null != property)
            {
                if (null == SettingsProperties)
                {
                    SettingsProperties = new List<SpouseSettingsProperty>();
                }

                if (!SettingsProperties.Contains(property))
                {
                    SettingsProperties.Add(property);
                }
            }
            return this;
        }



    }
}
