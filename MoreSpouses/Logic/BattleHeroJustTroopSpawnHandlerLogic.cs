using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SueMoreSpouses.logic
{
    class BattleHeroJustTroopSpawnHandlerLogic : MissionLogic
	{
		private MissionAgentSpawnLogic _missionAgentSpawnLogic;

		private MapEvent _mapEvent;

		public override void OnBehaviourInitialize()
		{
			base.OnBehaviourInitialize();
			this._missionAgentSpawnLogic = base.Mission.GetMissionBehaviour<MissionAgentSpawnLogic>();
			this._mapEvent = MapEvent.PlayerMapEvent;
		}

		public override void AfterStart()
		{
			Scene scene = base.Mission.Scene;
			List<GameEntity> list = base.Mission.Scene.FindEntitiesWithTag("sp_special_item").ToList<GameEntity>();

			int num = MBMath.Floor((float)this._mapEvent.GetNumberOfInvolvedMen(BattleSideEnum.Defender));
			int num2 = MBMath.Floor((float)this._mapEvent.GetNumberOfInvolvedMen(BattleSideEnum.Attacker));
			int defenderInitialSpawn = num;
			int attackerInitialSpawn = num2;
			this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Defender, false);
			this._missionAgentSpawnLogic.SetSpawnHorses(BattleSideEnum.Attacker, false);
			this._missionAgentSpawnLogic.InitWithSinglePhase(num, num2, defenderInitialSpawn, attackerInitialSpawn, true, true, 1f);
		}

		public override void OnAgentHit(Agent affectedAgent, Agent affectorAgent, int damage, in MissionWeapon affectorWeapon)
		{
			Scene scene = base.Mission.Scene;
			WorldFrame worldFrame =  affectedAgent.GetWorldFrame();
			WorldPosition worldPosition = affectedAgent.GetWorldPosition();
		}
	}
}
