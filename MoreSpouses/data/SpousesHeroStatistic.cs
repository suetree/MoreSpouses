using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace SueMoreSpouses.data
{
    [SaveableClass(2009021)]
    class SpousesHeroStatistic
    {
        [SaveableProperty(1)]
        internal Hero StatsHero { get; set; }

        [SaveableProperty(2)]
        internal int TotalKillCount { get; set; }

        [SaveableProperty(3)]
        internal int MVPCount { get; set; }

        [SaveableProperty(4)]
        internal int ZeroCount { get; set; }

        [SaveableProperty(5)]
        internal int FightCount { get; set; }

        public SpousesHeroStatistic(Hero hero)
        {
            StatsHero = hero;
        }
    }
}
