using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.SaveSystem;

namespace SueMoreSpouses.data.sp
{
    [SaveableClass(2009026)]
    class SpousesBattleRecordReward
    {
      
        [SaveableProperty(1)]
        internal float RenownChange { get; set; }

        [SaveableProperty(2)]
        internal float InfluenceChange { get; set; }

        [SaveableProperty(3)]
        internal float MoraleChange { get; set; }

        [SaveableProperty(4)]
        internal float GoldChange { get; set; }

        [SaveableProperty(5)]
        internal float PlayerEarnedLootPercentage { get; set; }

        public SpousesBattleRecordReward(float renownChange, float influenceChange, float moraleChange, float goldChange, float playerEarnedLootPercentage)
        {
            RenownChange = renownChange;
            InfluenceChange = influenceChange;
            MoraleChange = moraleChange;
            GoldChange = goldChange;
            PlayerEarnedLootPercentage = playerEarnedLootPercentage;
        }

    }
}
