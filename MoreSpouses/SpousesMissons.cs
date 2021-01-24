using SandBox;
using SueMoreSpouses.logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;

namespace SueMoreSpouses
{
    class SpousesMissons
    {
		public static Mission OpenDateMission(string scene, List<Hero> heros)
		{
			return MissionState.OpenNew("TownCenter", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, "", false), (Mission mission) => new MissionBehaviour[]
			{
				new MissionOptionsComponent(),
				new CampaignMissionComponent(),
				new MissionBasicTeamLogic(),
				new BattleSpawnLogic("battle_set"),
			    new SpousesDateSpawnLogic(heros),
				new BattleAgentLogic(),
				new AgentBattleAILogic(),
				new MissionFacialAnimationHandler(),

				new MissionHardBorderPlacer(),
		        new MissionBoundaryPlacer(),
		        new MissionBoundaryCrossingHandler(),

				new AgentMoraleInteractionLogic(),
	        	new HighlightsController(),
		        new BattleHighlightsController()
			}, true, true);
		}

	}
}
