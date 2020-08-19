using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SueMoreSpouses.data
{
    class SettingData
    {

        public bool ExspouseGetPregnancyEnable { get; set; }
        public float ExspouseGetPregnancyDailyChance { get; set; }
        public float ExspouseGetPregnancyDurationInDays { get; set; }

        public bool ChildrenFastGrowthEnable { get; set; }
        public int ChildrenFastGrowthCycleInDays { get; set; }
        public int ChildrenFastGrowtStopGrowUpAge { get; set; }

        public bool ChildrenSkillFixEnable { get; set; }

        public SettingData()
        {
            this.ChildrenFastGrowtStopGrowUpAge = 18;
            this.ChildrenFastGrowthCycleInDays = 36;
            this.ExspouseGetPregnancyDailyChance = 0.5f;
            this.ExspouseGetPregnancyDurationInDays = 30;

        }
    }

  

}
