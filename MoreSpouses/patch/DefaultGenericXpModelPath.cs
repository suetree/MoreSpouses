using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;

namespace SueMoreSpouses.patch
{
    //[HarmonyPatch(typeof(DefaultGenericXpModel), "GetXpMultiplier")]
    class DefaultGenericXpModelPath
    {
        public static void Postfix(ref float __result, Hero hero)
        {
            if (null != hero.Clan  && hero.Clan == Hero.MainHero.Clan)
            {
                __result = 20;
            }
          
        }
    }
}
