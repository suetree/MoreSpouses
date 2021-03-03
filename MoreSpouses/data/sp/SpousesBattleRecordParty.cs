using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.SaveSystem;

namespace SueMoreSpouses.Data.sp
{
    [SaveableClass(2009024)]
    class SpousesBattleRecordParty
    {

        [SaveableProperty(1)]
        internal List<SpousesBattleRecordCharacter> Characters { get; set; }

        [SaveableProperty(2)]
        internal String Name { get; set; }

        [SaveableProperty(3)]
        internal String  UniqueId { get; set; }

        [SaveableProperty(4)]
        internal int KillCount { get; set; }

        [SaveableProperty(5)]
        internal int Remain { get; set; }

        [SaveableProperty(6)]
        internal int Killed { get; set; }

        [SaveableProperty(7)]
        internal int Wounded { get; set; }

        [SaveableProperty(8)]
        internal int RunAway { get; set; }

        public SpousesBattleRecordParty(String uniqueId)
        {
            this.UniqueId = uniqueId;
            this.Characters = new List<SpousesBattleRecordCharacter>();
        }

        public SpousesBattleRecordCharacter GetBattleRecordCharacter(BasicCharacterObject character)
        {
            foreach (SpousesBattleRecordCharacter data in Characters)
            {
                if (data.Character == character)
                {
                    return data;
                }
            }
            return null;
        }

        public void AddSpousesBattleRecordCharacter(SpousesBattleRecordCharacter data)
        {
            Characters.Add(data);
        }
    }
}
