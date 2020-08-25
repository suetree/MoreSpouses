using HarmonyLib;
using Helpers;
using MountAndBlade.CampaignBehaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace SueMoreSpouses.operation
{
    class ChildrenGrowthOperation
    {

        public static void FastGrowth(Hero child)
        {
            if (!MoreSpouseSetting.Instance.SettingData.ChildrenFastGrowthEnable) return;
            if (null == child) return;
            if (child.Age < MoreSpouseSetting.Instance.SettingData.ChildrenFastGrowtStopGrowUpAge)
            {
                int lastAge = (int)child.Age;
                int circle = MoreSpouseSetting.Instance.SettingData.ChildrenFastGrowthCycleInDays;
                int days = (int)child.BirthDay.ElapsedDaysUntilNow;
                if (days % circle == 0)
                {
                    child.BirthDay = CampaignTime.Years((float)CampaignTime.Now.ToYears - (float)(lastAge + 1));
                    InformationManager.DisplayMessage(new InformationMessage("hero "+ child.Name.ToString() + " fast grow up  age = " + (int)child.Age));
                }

                int comeOfAgeDays = (int)CampaignTime.Years((float)Campaign.Current.Models.AgeModel.HeroComesOfAge).ToDays;
                days = (int)child.BirthDay.ElapsedDaysUntilNow;
                if (days == comeOfAgeDays)
                {
                    TextObject textObject = GameTexts.FindText("suems_children_grow_up_to_hero_age", null);
                    StringHelpers.SetCharacterProperties("SUE_HERO", child.CharacterObject, null, textObject);
                    InformationManager.AddQuickInformation(textObject, 0, null, "event:/ui/notification/quest_finished");
                    //GrowUpForSkill(child);

                   /* if (child.IsAlive && !child.IsOccupiedByAnEvent())
                    {
                       
                    }
                    else if (!child.IsTemplate && child.IsAlive)
                    {
                        if ((int)child.BirthDay.ElapsedDaysUntilNow == (int)CampaignTime.Years((float)Campaign.Current.Models.AgeModel.HeroComesOfAge).ToDays)
                        {
                            if (child.HeroState != Hero.CharacterStates.Active)
                            {
                                InformationManager.DisplayMessage(new InformationMessage("test  " + child.Name));
                                // CampaignEventDispatcher.Instance.OnHeroComesOfAge(current);
                            }
                        }
                        else if ((int)child.BirthDay.ElapsedDaysUntilNow == (int)CampaignTime.Years((float)Campaign.Current.Models.AgeModel.BecomeTeenagerAge).ToDays)
                        {
                           //CampaignEventDispatcher.Instance.OnHeroReachesTeenAge(current);
                        }
                        else if ((int)child.BirthDay.ElapsedDaysUntilNow == (int)CampaignTime.Years((float)Campaign.Current.Models.AgeModel.BecomeChildAge).ToDays)
                        {
                          //  CampaignEventDispatcher.Instance.OnHeroGrowsOutOfInfancy(current);
                        }
                    }*/
                }
            }

        }

        [HarmonyPatch(typeof(Hero), "IsOccupiedByAnEvent")]
        public class HeroIsOccupiedByAnEventPatch
        {
            static bool Prefix(ref Hero __instance, ref bool __result)
            {
                if (null != Hero.MainHero.Children && Hero.MainHero.Children.Contains(__instance))
                {
                    if ((int)__instance.Age <= Campaign.Current.Models.AgeModel.HeroComesOfAge)
                    {
                        __result = true;
                        return false;
                    }

                }
                return true;
            }
        }

    }
}
