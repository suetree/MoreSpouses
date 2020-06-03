using HarmonyLib;
using SueMoreSpouses.operation;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;

namespace SueMoreSpouses.patch
{
    class AgingCampaignBehaviorPatch
    {

        [HarmonyPatch(typeof(AgingCampaignBehavior), "DailyTick")]
        public class AgingCampaignBehaviorDailyTickPatch
        {
           
            static void Prefix()
            {

                if (Hero.MainHero.Children.Count != 0)
                {
                    Hero.MainHero.Children.ForEach((Hero child) => ChildrenGrowthOperation.FastGrowth(child));
                }
            }

        }
    }
}
