using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SueMoreSpouses.view.setting
{
    class SpouseSettingBuilder
    {
        public  List<SpouseSettingGroup> Group { get; set; }

        public  SpouseSettingGroup BuildGroup(String name)
        {
            if (null == Group)
            {
                Group = new List<SpouseSettingGroup>();
            }
            SpouseSettingGroup settingGroup = new SpouseSettingGroup(name);
            this.Group.Add(settingGroup);
            return settingGroup;
        }

    }
}
