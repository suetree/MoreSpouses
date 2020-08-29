using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
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
            Type type = __instance.GetType();
            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            if (null == type) return;
            if (hero.IsFemale && hero.Age > 18 && Hero.MainHero.ExSpouses.Contains(hero) && !hero.IsPregnant && hero != Hero.MainHero.Spouse)
            {
                    try
                    {
                        MethodInfo methodinfo = type.GetMethod("RefreshSpouseVisit", flags);
                        object[] parameters = new object[1] { hero };
                        if (null != methodinfo)
                        {
                            object i = methodinfo.Invoke(__instance, parameters);
                        }
                          
                    }
                    catch (TargetInvocationException e)
                    {
                        InformationManager.DisplayMessage(new InformationMessage($"MoreSpouses.DailyTickHero error:" + e.Message));
                    }
                
            }else  if (!hero.IsFemale && hero.IsPregnant && Hero.MainHero.ExSpouses.Contains(hero)) //男性怀孕逻辑处理
            {
             
                try
                {
                    MethodInfo[] info = type.GetMethods(flags);
                    object[] parameters = new object[1] { hero };
                    foreach (MethodInfo func in info)
                    {
                        if (func.Name.Equals("CheckOffspringsToDeliver", StringComparison.Ordinal))
                        {
                            ParameterInfo[] parmInfo = func.GetParameters();
                        
                            if (parmInfo.Length == 1)
                            {
                                Type type1 = parmInfo[0].ParameterType;
                                Type type2 = hero.GetType();
                                if (type1.Equals(type2))
                                {
                                    func.Invoke(__instance, parameters);
                                }
                               
                            }

                        }
                    }
                }
                catch (TargetInvocationException e)
                {
                    InformationManager.DisplayMessage(new InformationMessage($"MoreSpouses.DailyTickHero error:" + e.Message));
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
            if (mother.IsFemale && mother.Age > 18 && (Hero.MainHero.ExSpouses.Contains(mother) || mother == Hero.MainHero.Spouse))
            {
                CampaignTime campaignTime = CampaignTime.DaysFromNow(Campaign.Current.Models.PregnancyModel.PregnancyDurationInDays);
                if (MoreSpouseSetting.Instance.SettingData.ExspouseGetPregnancyEnable)
                {
                    campaignTime = CampaignTime.DaysFromNow(MoreSpouseSetting.Instance.SettingData.ExspouseGetPregnancyDurationInDays);
                }
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
            if (MoreSpouseSetting.Instance.SettingData.ExspouseGetPregnancyEnable)
            {
                if (hero.Clan == Clan.PlayerClan && hero != Hero.MainHero && (Hero.MainHero.ExSpouses.Contains(hero) || hero == Hero.MainHero.Spouse))
                {
                    float num = MoreSpouseSetting.Instance.SettingData.ExspouseGetPregnancyDailyChance;
                    __result = num;

                }
            }
        }
    }





    /*   [HarmonyPatch(typeof(TaleWorlds.CampaignSystem.SandBox.GameComponents.DefaultPregnancyModel),
            "PregnancyDurationInDays", MethodType.Getter)]
       public class PregnancyDurationInDaysPatch
       {
           static void Postfix(ref float __result)
           {
               __result = GlobalSettings<MoreSpouseSetting>.Instance.PregnancyDurationInDays;  //孕期天数
           }
       }*/

/*    [HarmonyPatch(typeof(AgingCampaignBehavior),"OnHeroGrowsOutOfInfancy")]
    public class OnHeroGrowsOutOfInfancyPatch
    {
        static void Postfix(Hero child)
        {
            int becomeInfantAge = Campaign.Current.Models.AgeModel.BecomeInfantAge;
            List<CharacterObject> list = (from t in CharacterObject.ChildTemplates
                                          where t.Culture == child.Culture && t.Age <= (float)becomeInfantAge && t.IsFemale == child.IsFemale && t.Occupation == Occupation.Lord
                                          select t).ToList<CharacterObject>();
            if (!list.IsEmpty<CharacterObject>())
            {
                CharacterObject randomElement = list.GetRandomElement<CharacterObject>();
                EquipmentHelper.AssignHeroEquipmentFromEquipment(child, randomElement.CivilianEquipments.GetRandomElement<Equipment>());
                EquipmentHelper.AssignHeroEquipmentFromEquipment(child, randomElement.BattleEquipments.GetRandomElement<Equipment>());
            }
        }
    }*/

 

   

}
