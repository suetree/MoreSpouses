using Helpers;
using SandBox;
using SandBox.Source.Missions;
using SandBox.Source.Missions.Handlers;
using SueMoreSpouses.logic;
using SueMoreSpouses.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Source.Missions;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers;
using TaleWorlds.MountAndBlade.Source.Missions.Handlers.Logic;

namespace SueMoreSpouses
{
    class GameComponent
    {

        public  static CampaignEventDispatcher CampaignEventDispatcher()
        {
            CampaignEventDispatcher dispatcher = null; ;
            PropertyInfo propertyInfo = Campaign.Current.GetType().GetProperty("CampaignEventDispatcher", BindingFlags.Instance | BindingFlags.NonPublic);
            if (null != propertyInfo)
            {
                dispatcher = (CampaignEventDispatcher)propertyInfo.GetValue(Campaign.Current);

            }

            return dispatcher;
        }


        public static void SendEvent(String eventMethod, object[] objectParams)
        {
            CampaignEventDispatcher dispatcher = GameComponent.CampaignEventDispatcher();
            if (null != dispatcher)
            {
                ReflectUtils.ReflectMethodAndInvoke("OnHeroComesOfAge", dispatcher, objectParams);
            }
        }

        public static void StartBattle( PartyBase defenderParty)
        {
            StartBattleAction.Apply(MobileParty.MainParty.Party, defenderParty);
            PlayerEncounter.RestartPlayerEncounter(MobileParty.MainParty.Party, defenderParty, true);
            PlayerEncounter.Update();
            CampaignMission.OpenBattleMission(PlayerEncounter.GetBattleSceneForMapPosition(MobileParty.MainParty.Position2D));
        }


		public static Mission OpenCastleCourtyardMission(string scene, string sceneLevels, Location location, CharacterObject talkToChar)
		{
			return MissionState.OpenNew("TownCenter", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, sceneLevels, false), delegate (Mission mission)
			{
				List<MissionBehaviour> list = new List<MissionBehaviour>();
				list.Add(new MissionOptionsComponent());
				list.Add(new CampaignMissionComponent());

				list.Add(new MissionBasicTeamLogic());
				list.Add(new MissionSettlementPrepareLogic());
				list.Add(new TownCenterMissionController());
				list.Add(new MissionAgentLookHandler());
				list.Add(new SandBoxMissionHandler());
				list.Add(new BasicLeaveMissionLogic());
				list.Add(new LeaveMissionLogic());
				list.Add(new BattleAgentLogic());
			   /*	Settlement currentTown = SandBoxMissions.GetCurrentTown();
				if (currentTown != null)
				{
					list.Add(new WorkshopMissionHandler(currentTown));
				}*/
				list.Add(new MissionAgentPanicHandler());
				list.Add(new AgentTownAILogic());
				list.Add(new MissionConversationHandler(talkToChar));
				list.Add(new MissionAgentHandler(location, null));
				list.Add(new HeroSkillHandler());
				list.Add(new MissionFightHandler());
				list.Add(new MissionFacialAnimationHandler());
				list.Add(new MissionDebugHandler());
				list.Add(new MissionHardBorderPlacer());
				list.Add(new MissionBoundaryPlacer());
				list.Add(new MissionBoundaryCrossingHandler());
				list.Add(new VisualTrackerMissionBehavior());
				return list.ToArray();
			}, true, true);
		}

		public static Mission OpenBattleJustHero(string scene, string upgradeLevel)
		{

		   MissionInitializerRecord record = new MissionInitializerRecord(scene)
			{
				PlayingInCampaignMode = Campaign.Current.GameMode == CampaignGameMode.Campaign,
				AtmosphereOnCampaign = (Campaign.Current.GameMode == CampaignGameMode.Campaign) ? Campaign.Current.Models.MapWeatherModel.GetAtmosphereModel(CampaignTime.Now, MobileParty.MainParty.GetLogicalPosition()) : null,
				SceneLevels = upgradeLevel
		   };

			MissionAgentSpawnLogic lc2 = new MissionAgentSpawnLogic(new IMissionTroopSupplier[]
			{
				 new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, BattleSideEnum.Defender, null),
				 new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, BattleSideEnum.Attacker, null)
			 }, PartyBase.MainParty.Side);

			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			bool isPlayerInArmy = MobileParty.MainParty.Army != null;
			List<string> heroesOnPlayerSideByPriority = HeroHelper.OrderHeroesOnPlayerSideByPriority();
			return MissionState.OpenNew("Battle", record, delegate (Mission mission)
			{
				List<MissionBehaviour> list = new List<MissionBehaviour>();
				list.Add(new MissionOptionsComponent());
				list.Add(new CampaignMissionComponent());
				list.Add(new BattleEndLogic());
				list.Add(new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(BattleSideEnum.Defender), MobileParty.MainParty.MapEvent.GetLeaderParty(BattleSideEnum.Attacker), Mission.MissionTeamAITypeEnum.FieldBattle, isPlayerSergeant));
				list.Add(new BattleMissionStarterLogic());
				//list.Add(new MissionDefaultCaptainAssignmentLogic());
				list.Add(new BattleSpawnLogic("battle_set"));
				list.Add(new MissionAgentPanicHandler());
				list.Add(new AgentBattleAILogic());
				list.Add(new BattleObserverMissionLogic());
				list.Add(lc2);
				list.Add(new MissionFightHandler());
				list.Add(new AgentFadeOutLogic());
				list.Add(new BattleAgentLogic());
				list.Add(new AgentVictoryLogic());
				list.Add(new MissionDebugHandler());
				list.Add(new MissionHardBorderPlacer());
				list.Add(new MissionBoundaryPlacer());
				list.Add(new MissionBoundaryCrossingHandler());
				list.Add(new BattleMissionAgentInteractionLogic());
				list.Add(new HighlightsController());
				list.Add(new BattleHighlightsController());

				list.Add(new BattleHeroJustTroopSpawnHandlerLogic());
				list.Add(new FieldBattleController());
				//list.Add(new AgentMoraleInteractionLogic());
				list.Add(new  AssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, heroesOnPlayerSideByPriority, FormationClass.NumberOfRegularFormations));

				Hero leader = MapEvent.PlayerMapEvent.AttackerSide.LeaderParty.LeaderHero;
				string arg_18B_0 = (leader != null) ? leader.Name.ToString() : null;
				Hero leaderHero = MapEvent.PlayerMapEvent.DefenderSide.LeaderParty.LeaderHero;
				//list.Add(new CreateBodyguardMissionBehavior(arg_18B_0, (leaderHero != null) ? leaderHero.Name.ToString() : null, null, null, true));
				return list.ToArray();
			}, true, true);
		}


		public static Mission OpenBattleNormal(string scene, string sceneLevels)
		{

			MissionAgentSpawnLogic lc = new MissionAgentSpawnLogic(new IMissionTroopSupplier[]
			{
				 new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, BattleSideEnum.Defender, null),
				new PartyGroupTroopSupplier(MapEvent.PlayerMapEvent, BattleSideEnum.Attacker, null)
			 }, PartyBase.MainParty.Side);


			bool isPlayerSergeant = MobileParty.MainParty.MapEvent.IsPlayerSergeant();
			bool isPlayerInArmy = MobileParty.MainParty.Army != null;
			List<string> heroesOnPlayerSideByPriority = HeroHelper.OrderHeroesOnPlayerSideByPriority();
			return MissionState.OpenNew("Battle", SandBoxMissions.CreateSandBoxMissionInitializerRecord(scene, sceneLevels, false), delegate (Mission mission)
			{
				MissionBehaviour[] expr_07 = new MissionBehaviour[26];
				expr_07[0] = new MissionOptionsComponent();
				expr_07[1] = new CampaignMissionComponent();
				expr_07[2] = new BattleEndLogic();
				expr_07[3] = new MissionCombatantsLogic(MobileParty.MainParty.MapEvent.InvolvedParties, PartyBase.MainParty, MobileParty.MainParty.MapEvent.GetLeaderParty(BattleSideEnum.Defender), MobileParty.MainParty.MapEvent.GetLeaderParty(BattleSideEnum.Attacker), Mission.MissionTeamAITypeEnum.FieldBattle, isPlayerSergeant);
				expr_07[4] = new MissionDefaultCaptainAssignmentLogic();
				expr_07[5] = new BattleMissionStarterLogic();
				expr_07[6] = new BattleSpawnLogic("battle_set");
				expr_07[7] = new AgentBattleAILogic();
				expr_07[8] = lc;
				expr_07[9] = new BaseMissionTroopSpawnHandler();
				expr_07[10] = new AgentFadeOutLogic();
				expr_07[11] = new BattleObserverMissionLogic();
				expr_07[12] = new BattleAgentLogic();
				expr_07[13] = new AgentVictoryLogic();
				expr_07[14] = new MissionDebugHandler();
				expr_07[15] = new MissionAgentPanicHandler();
				expr_07[16] = new MissionHardBorderPlacer();
				expr_07[17] = new MissionBoundaryPlacer();
				expr_07[18] = new MissionBoundaryCrossingHandler();
				expr_07[19] = new BattleMissionAgentInteractionLogic();
				expr_07[20] = new FieldBattleController();
				expr_07[21] = new AgentMoraleInteractionLogic();
				expr_07[22] = new HighlightsController();
				expr_07[23] = new BattleHighlightsController();
				expr_07[24] = new AssignPlayerRoleInTeamMissionController(!isPlayerSergeant, isPlayerSergeant, isPlayerInArmy, heroesOnPlayerSideByPriority, FormationClass.NumberOfRegularFormations);
				int arg_190_1 = 25;
				Hero expr_152 = MapEvent.PlayerMapEvent.AttackerSide.LeaderParty.LeaderHero;
				string arg_18B_0 = (expr_152 != null) ? expr_152.Name.ToString() : null;
				Hero expr_177 = MapEvent.PlayerMapEvent.DefenderSide.LeaderParty.LeaderHero;
				expr_07[arg_190_1] = new CreateBodyguardMissionBehavior(arg_18B_0, (expr_177 != null) ? expr_177.Name.ToString() : null, null, null, true);
				return expr_07;
			}, true, true);
		}

		// Helpers.MobilePartyHelper
		public static FlattenedTroopRoster GetStrongestAndPriorTroops(MobileParty mobileParty, int maxTroopCount, List<Hero> includeList)
		{
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			FlattenedTroopRoster memberRoster = mobileParty.MemberRoster.ToFlattenedRoster();
			memberRoster.RemoveIf((x) => x.IsWounded);
			List<CharacterObject> list = memberRoster.Select((x) => x.Troop).OrderByDescending((x) => x.Level).ToList<CharacterObject>();
			if (null != includeList && includeList.Count > 0)
			{
				foreach (Hero hero  in includeList)
				{
					if (list.Any((x) => x == hero.CharacterObject))
					{
						list.Remove(hero.CharacterObject);
						troopRoster.AddToCounts(hero.CharacterObject, 1, false, 0, 0, true, -1);
						maxTroopCount--;
					}
				}
			}
			List<CharacterObject> heroList = list.Where((x) => x.IsHero).ToList<CharacterObject>();
			int num = Math.Min(heroList.Count<CharacterObject>(), maxTroopCount);
			for (int i = 0; i < num; i++)
			{
				troopRoster.AddToCounts(heroList[i], 1, false, 0, 0, true, -1);
				list.Remove(heroList[i]);
			}
			return troopRoster.ToFlattenedRoster();
		}
	}


}
