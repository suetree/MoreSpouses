using HarmonyLib;
using Newtonsoft.Json.Schema;
using SandBox.ViewModelCollection;
using SueMoreSpouses.behavior;
using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection;

namespace SueMoreSpouses.patch
{
    [HarmonyPatch(typeof(SPScoreboardVM), "TroopNumberChanged")]
    class BattleSimulationTroopNumberChangedPath
    {
        public static void Postfix(SPScoreboardVM __instance, BattleSideEnum side, IBattleCombatant battleCombatant, BasicCharacterObject character, int number = 0, int numberDead = 0, int numberWounded = 0, int numberRouted = 0, int numberKilled = 0, int numberReadyToUpgrade = 0)
        {
            Campaign.Current.GetCampaignBehavior<SpousesStatsBehavior>()
                           .GetSpouseStatsBusiness().RecordBattleData(side, battleCombatant, character, number, numberDead, numberKilled, numberRouted, numberWounded);

        }
    }

    [HarmonyPatch(typeof(SPScoreboardVM), "Initialize")]
    class BattleSimulationInitializePath
    {

        public static void Prefix(SPScoreboardVM __instance, IMissionScreen missionScreen, Mission mission, Action releaseSimulationSources, Action<bool> onToggle)
        {
            Campaign.Current.GetCampaignBehavior<SpousesStatsBehavior>().GetSpouseStatsBusiness().Initialize(); ;
        }
    }

    [HarmonyPatch(typeof(SPScoreboardVM), "OnExitBattle")]
    class BattleSimulationOnExitBattlePath
    {

        public static void Postfix(SPScoreboardVM __instance)
        {
            StatExplainer renownExplainer = null;
            StatExplainer influencExplainer = null;
            StatExplainer moraleExplainer = null;
            float renownChange = 0;
            float influenceChange = 0;
            float moraleChange = 0;
            float goldChange = 0;
            float playerEarnedLootPercentage = 0;
            if (PlayerEncounter.IsActive)
            {
                PlayerEncounter.GetBattleRewards(out renownChange, out influenceChange, out moraleChange, out goldChange, out playerEarnedLootPercentage, ref renownExplainer, ref influencExplainer, ref moraleExplainer);
            }
            Campaign.Current.GetCampaignBehavior<SpousesStatsBehavior>().GetSpouseStatsBusiness()
                .EndCountHeroBattleData(__instance.BattleResultIndex, renownChange, influenceChange, moraleChange, goldChange, playerEarnedLootPercentage);
        }
    }

   

}
