using SueMoreSpouses.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;

namespace SueMoreSpouses
{
    class SpousesStatsMissionLogic : MissionLogic
    {

        SpouseStatsBusiness _spouseStatsBusiness;

        public SpousesStatsMissionLogic(SpouseStatsBusiness spouseStatsBusiness) : base()
        {
            this._spouseStatsBusiness = spouseStatsBusiness;
        }

        public override void ShowBattleResults()
        {

            foreach (Agent ag in Mission.Current.PlayerTeam.ActiveAgents)
            {
                CharacterObject character = ag.Character as CharacterObject;
                if(null != character )
                {
                  //  this._spouseStatsBusiness.CountHeroBattleDataForStats(character, ag.KillCount);
                    
                   // this._spouseStatsBusiness.CountHeroBattleDataForRecordsSide(0, character, ag.rem);

                }
             
            }

            foreach (Agent ag in Mission.Current.PlayerEnemyTeam.ActiveAgents)
            {
            }

                //this._spouseStatsBusiness.EndCountHeroBattleData();


        }
    }
}
