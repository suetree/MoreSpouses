using SueMoreSpouses.data.sp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.SaveSystem;

namespace SueMoreSpouses.data
{
    [SaveableClass(2009022)]
    class SpousesBattleRecord
    {
     
        [SaveableProperty(1)]
        internal String Name { get; set; }

        [SaveableProperty(2)]
        internal SpousesBattleRecordSide AttackerSide { get; set; }

        [SaveableProperty(3)]
        internal SpousesBattleRecordSide DefenderSide { get; set; }

        [SaveableProperty(4)]
        internal CampaignTime CreatedTime { get; set; }

        [SaveableProperty(5)]
        internal int BattleResultIndex { get; set; }
       
        [SaveableProperty(6)]
        internal SpousesBattleRecordReward RecordReward { get; set; }

         public SpousesBattleRecord()
        {
            this.CreatedTime = CampaignTime.Now;
            this.Name = this.CreatedTime.ToString();

        }
    }
}
