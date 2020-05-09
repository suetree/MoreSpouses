using HarmonyLib;
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


        public override void OnGameLoaded(Game game, object initializerObject)
        {
            CampaignGameStarter gameInitializer = (CampaignGameStarter)initializerObject;
            gameInitializer.LoadGameTexts(string.Format("{0}/Modules/{1}/ModuleData/sue_chat_prisoner.xml", BasePath.Name, "SueMoreSpouses"));
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
         
          //  game.GameTextManager.LoadGameTexts(string.Format("{0}/Modules/{1}/ModuleData/sue_chat_prisoner.xml", BasePath.Name, "SueMoreSpouses"));
            ((CampaignGameStarter)gameStarterObject).AddBehavior(new SpouseFromPrisonerBehavior());
        }
    }
}