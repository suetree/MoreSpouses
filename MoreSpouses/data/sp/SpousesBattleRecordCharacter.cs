using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace SueMoreSpouses.data
{
    [SaveableClass(2009025)]
    class SpousesBattleRecordCharacter
    {
        [SaveableProperty(1)]
        internal BasicCharacterObject Character { get; set; }

        [SaveableProperty(2)]
        internal int KillCount { get; set; }

        [SaveableProperty(3)]
        internal int Remain { get; set; }

        [SaveableProperty(4)]
        internal int Killed { get; set; }

        [SaveableProperty(5)]
        internal int Wounded { get; set; }

        [SaveableProperty(6)]
        internal int RunAway { get; set; }

        public SpousesBattleRecordCharacter(BasicCharacterObject character)
        {
            Character = character;
        }

       
    }
}
