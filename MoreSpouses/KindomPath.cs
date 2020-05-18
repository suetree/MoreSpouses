

using HarmonyLib;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;

namespace SueMoreSpouses
{
    [HarmonyPatch(typeof(ChangeKingdomAction), "ApplyInternal")]
    class KindomPath
    {
        static bool Prefix(Clan clan, Kingdom kingdom, Object detail, int awardMultiplier = 0, bool byRebellion = false, bool showNotification = true)
        {
            if (Clan.PlayerClan == clan)
            {
                if (null != Hero.MainHero.MapFaction && Hero.MainHero.MapFaction is Kingdom && Hero.MainHero.IsFactionLeader)
                {
                    InformationManager.DisplayMessage(new InformationMessage(" Not allowed MainPlay  to leave the MainPlay country "));
                    return false;
                }
              
            }
            return true;
        } 
    }

   /* [HarmonyPatch(typeof(LeaveKingdomAsClanBarterable), "Apply")]
    class LeaveKingdomAsClanBarterablePath
    {
        static bool Prefix(LeaveKingdomAsClanBarterable __instance)
        {
            //List<Clan>.Enumerator enumerator = Clan.All.GetEnumerator();
            if (__instance.OriginalOwner == Hero.MainHero)
            {
                Hero hero = Hero.MainHero;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(JoinKingdomAsClanBarterable), "Apply")]
    class JoinKingdomAsClanBarterablePath
    {
        static bool Prefix(LeaveKingdomAsClanBarterable __instance)
        {
          
            if(Hero.MainHero == __instance.OriginalOwner )
            {
                Hero hero = Hero.MainHero;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MarriageBarterable), "Apply")]
    class MarriageBarterablePath
    {
        static bool Prefix(MarriageBarterable __instance)
        {

            if (Hero.MainHero == __instance.OriginalOwner)
            {
                Hero hero = Hero.MainHero;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MercenaryJoinKingdomBarterable), "Apply")]
    class MercenaryJoinKingdomBarterablePath
    {
        static bool Prefix(MercenaryJoinKingdomBarterable __instance)
        {

            if (Hero.MainHero == __instance.OriginalOwner)
            {
                Hero hero = Hero.MainHero;
            }
            return true;
        }
    }*/
}
