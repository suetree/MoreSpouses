using System;
using System.Collections;
using System.IO;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;

namespace SueMoreSpouses
{
    [HarmonyPatch(typeof(PregnancyCampaignBehavior),
      "DailyTickHero")]
    public class DailyTickHeroPath
    {
        static void Postfix(PregnancyCampaignBehavior __instance, Hero hero)
        {
            if (hero.IsFemale && hero.Age > 18 && Hero.MainHero.ExSpouses.Contains(hero) && !hero.IsPregnant && hero != Hero.MainHero.Spouse)
            {
                Type type = __instance.GetType();
                BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
                if (null != type)
                {
                    MethodInfo methodinfo = type.GetMethod("RefreshSpouseVisit", flags);
                    object[] parameters = new object[1] { hero };
                    try
                    {
                        object i = methodinfo.Invoke(__instance, parameters);
                    }
                    catch (TargetInvocationException e)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"MoreSpouses.DailyTickHero error:" + e.Message));
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(PregnancyCampaignBehavior),  "CheckAreNearby")]
    public class CheckAreNearbyPath
    {

        static void Prefix(Hero hero, out Hero spouse)
        {
            if (hero.IsFemale && hero.Age > 18 && Hero.MainHero.ExSpouses.Contains(hero) && !hero.IsPregnant && hero != Hero.MainHero.Spouse)
            {
                spouse = Hero.MainHero;
            }
            else
            {
                spouse = hero.Spouse;
            }
        }

    }



    [HarmonyPatch(typeof(PregnancyCampaignBehavior), "ChildConceived")]
    public class ChildConceivedPath
    {

        static bool Prefix(PregnancyCampaignBehavior __instance, Hero mother)
        {
            if (mother.IsFemale && mother.Age > 18 && Hero.MainHero.ExSpouses.Contains(mother) && mother != Hero.MainHero.Spouse)
            {
                CampaignTime campaignTime = CampaignTime.DaysFromNow(Campaign.Current.Models.PregnancyModel.PregnancyDurationInDays);
                Type type = __instance.GetType();
                FieldInfo _heroPregnancies = type.GetField("_heroPregnancies", BindingFlags.NonPublic | BindingFlags.Instance);
                IList list = (IList)_heroPregnancies.GetValue(__instance);
                object[] parameters = new object[3] { mother, Hero.MainHero, campaignTime };
                try
                {
                    Type innerType = type.GetNestedType("Pregnancy", BindingFlags.NonPublic | BindingFlags.Instance);
                    object obj = Activator.CreateInstance(innerType, BindingFlags.Public | BindingFlags.Instance, null, parameters, null);
                    list.Add(obj);
                }
                catch (IOException e)
                {
                    InformationManager.DisplayMessage(new InformationMessage("MoreSpouses.ChildConceived error:" + e.Message));
                }
                return false; 
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(TaleWorlds.CampaignSystem.SandBox.GameComponents.DefaultPregnancyModel), "GetDailyChanceOfPregnancyForHero")]
    public class GetDailyChanceOfPregnancyForHero
    {
        static void Postfix(ref float __result, Hero hero)
        {
            if (hero.Clan == Clan.PlayerClan && hero != Hero.MainHero && Hero.MainHero.ExSpouses.Contains(hero) &&  hero != Hero.MainHero.Spouse)
            {
                float num = 0f;
                if ( hero.IsFertile && hero.Age >= 18f)
                {
                    ExplainedNumber explainedNumber = new ExplainedNumber(1f, null);
                    Helpers.PerkHelper.AddPerkBonusForCharacter(DefaultPerks.Medicine.PerfectHealth, hero.Clan.Leader.CharacterObject, ref explainedNumber);
                    num = (6.5f - (hero.Age - 18f) * 0.23f) * 0.02f * explainedNumber.ResultNumber;
                }
                if (hero.Children.Count == 0)
                {
                    num *= 3f;
                }
                else if (hero.Children.Count == 1)
                {
                    num *= 2f;
                }
                __result = num + 0.2f;
                
            }
        }
    }


}
