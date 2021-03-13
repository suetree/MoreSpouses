using SueMoreSpouses.Data.sp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace SueMoreSpouses.Data
{
    [SaveableClass(2009023)]
    class SpousesBattleRecordSide
    {

        [SaveableProperty(1)]
        internal List<SpousesBattleRecordParty> Parties { get; set; }

        [SaveableProperty(2)]
        internal String Name { get; set; }

        [SaveableProperty(3)]
        internal Banner Banner { get; set; }

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

        public SpousesBattleRecordSide(String name, Banner banner)
        {
            this.Name = name;
            this.Banner = banner;
            this.Parties = new List<SpousesBattleRecordParty>();
        }

       
        public SpousesBattleRecordParty GetPartyByUniqueId(String uniqueId)
        {
            SpousesBattleRecordParty result = null;
            if (null != uniqueId)
            {
                foreach (SpousesBattleRecordParty party in Parties)
                {
                    if (uniqueId == party.UniqueId)
                    {
                        result = party;
                        break;        
                    }
                }
            }
            return result;
        }




    }
}
