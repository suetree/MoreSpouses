using SueMoreSpouses.Data.sp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SueMoreSpouses.view.sp
{
    class BattleHistorySPPartyVM: ViewModel
    {

        private SpousesBattleRecordParty _party;

        private MBBindingList<BattleHistorySPCharacterVM> _members;

        private BattleHistorySPScoreVM _score;

        [DataSourceProperty]
        public BattleHistorySPScoreVM Score
        {
            get
            {
                return this._score;
            }
        }

        [DataSourceProperty]
        public MBBindingList<BattleHistorySPCharacterVM> Members
        {
            get
            {
                return this._members;
            }
        }



        public BattleHistorySPPartyVM (SpousesBattleRecordParty party)
        {
            this._party = party;
            this._score = new BattleHistorySPScoreVM();
            this.Score.UpdateScores(party.Name, party.Remain, party.KillCount, party.Wounded, party.RunAway, party.Killed, 0);
            this._members = new MBBindingList<BattleHistorySPCharacterVM>();
            if (null != this._party.Characters && this._party.Characters.Count > 0)
            {
                this._party.Characters.Sort((x, y) => {
                    return  - 1 * x.KillCount.CompareTo(y.KillCount);
                });
                this._party.Characters.ForEach(obj =>
                { 
                    if (null != obj)
                    {
                        this._members.Add(new BattleHistorySPCharacterVM(obj));
                    }
                }
                );
            }
        }
    }
}
