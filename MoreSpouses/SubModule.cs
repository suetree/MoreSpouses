﻿using HarmonyLib;
using MoreSpouses;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace SueMoreSpouses
{
    public class SubModule : MBSubModuleBase
    {

        protected override void OnSubModuleLoad()
        {
            Harmony harmony = new Harmony("mod.sue.morespouses");
            harmony.PatchAll();
        }
        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            if (gameStarterObject.GetType() == typeof(CampaignGameStarter))
            {
                CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;
                gameInitializer.LoadGameTexts(string.Format("{0}/Modules/{1}/ModuleData/sue_chat_prisoner.xml", BasePath.Name, "SueMoreSpouses"));
                gameInitializer.AddBehavior(new SpouseFromPrisonerBehavior());
            }
              
        }
    }
}