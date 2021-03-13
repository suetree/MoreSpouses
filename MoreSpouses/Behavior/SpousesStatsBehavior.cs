using SueMoreSpouses.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem;

namespace SueMoreSpouses.Behavior
{
    class SpousesStatsBehavior : CampaignBehaviorBase
    {

        [SaveableField(1)]
        List<SpousesHeroStatistic> _spousesStats;

        [SaveableField(2)]
        List<SpousesBattleRecord> _spousesBattleRecords;
        

        SpouseStatsBusiness spouseStatsBusiness;

        public override void RegisterEvents()
        {
            
            // CampaignEvents.OnMissionStartedEvent.AddNonSerializedListener(this, new Action<IMission>(this.CombatBattle));
           // CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, new Action<MapEvent>(this.MapBattle));
        }

        public override void SyncData(IDataStore dataStore)
        {
            try
            {
                dataStore.SyncData("SpousesStats", ref _spousesStats);
                dataStore.SyncData("SpousesBattleRecords", ref _spousesBattleRecords);
            }
            catch (Exception)
            {
            }
            finally
            {
            }
        }

     
        public void MapBattle(MapEvent mapEvent)
        {
            if (Hero.MainHero != null && mapEvent.IsPlayerMapEvent 
                && mapEvent.IsPlayerSimulation )
            {
               // GetSpouseStatsBusiness().EndCountHeroBattleData();
            }
        }

        public void CombatBattle(IMission imission)
        {

            if(null == this._spousesStats)
            {
                this._spousesStats = new List<SpousesHeroStatistic>();
            }

            Mission mission = imission as Mission;
            if (null != mission && (mission.CombatType == Mission.MissionCombatType.Combat
                || mission.CombatType == Mission.MissionCombatType.ArenaCombat)
               )
            {
                Mission.Current.AddMissionBehaviour(new SpousesStatsMissionLogic(GetSpouseStatsBusiness()));
            }
        }



        public List<SpousesHeroStatistic> SpousesStats()
        {
            if(null == this._spousesStats)
            {
                this._spousesStats = new List<SpousesHeroStatistic>();
            }
            return this._spousesStats;
        }

        public void ClanAllData()
        {
            SpousesBattleRecords().Clear();
            SpousesStats().Clear();
        }

        public List<SpousesBattleRecord> SpousesBattleRecords()
        {
            if (null == this._spousesBattleRecords)
            {
                this._spousesBattleRecords = new List<SpousesBattleRecord>();
            }
            return this._spousesBattleRecords;
        }

        public SpouseStatsBusiness GetSpouseStatsBusiness()
        {
            if (null == spouseStatsBusiness)
            {
                spouseStatsBusiness = new SpouseStatsBusiness(SpousesStats(), SpousesBattleRecords());
            }

            return spouseStatsBusiness;
        }

    }
}
