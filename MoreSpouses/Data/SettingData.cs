using SueMoreSpouses.setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SueMoreSpouses.Data
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

        public List<ValueNamePair> HeroSelectScope { get; set; }

        public ValueNamePair ChildrenFastGrowUpScope { get; set; }

        public String ChildrenNamePrefix { get; set; }
        public String ChildrenNameSuffix { get; set; }

        public bool NPCCharaObjectSkillAuto { get; set; }
        public int NPCCharaObjectFromTier { get; set; }

        public SettingData()
        {
            this.ChildrenFastGrowtStopGrowUpAge = 18;
            this.ChildrenFastGrowthCycleInDays = 36;
            this.ExspouseGetPregnancyDailyChance = 0.5f;
            this.ExspouseGetPregnancyDurationInDays = 30;

            this.NPCCharaObjectFromTier = 2;


            InitData();
           
        }

        public void InitData()
        {
            List<ValueNamePair> defaulteScope = new List<ValueNamePair>();
            defaulteScope.Add(new ValueNamePair(0, "{=hero_scope_player_related}Player rerelated"));
            defaulteScope.Add(new ValueNamePair(1, "{=hero_scope_clan_related}Clan related"));
            defaulteScope.Add(new ValueNamePair(2, "{=hero_scope_kindow_related}Kindom related"));
            defaulteScope.Add(new ValueNamePair(3, "{=hero_scope_world_related}World related"));
            this.HeroSelectScope = defaulteScope;
            if (ChildrenFastGrowUpScope == null)
            {
                this.ChildrenFastGrowUpScope = defaulteScope.First();
            }
        }
    }

  

}
