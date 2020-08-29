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
                int days = (int)CampaignTime.Now.ToDays;
                if (days % circle == 0)
                {
                    child.BirthDay = CampaignTime.Years((float)CampaignTime.Now.ToYears - (float)(lastAge + 1));
                    if (Hero.MainHero.Children.Contains(child))
                    {
                        InformationManager.DisplayMessage(new InformationMessage(child.Name.ToString() + "  AGE =" + child.Age));
                    }
                   
                }

                int comeOfAgeDays = (int)CampaignTime.Years((float)Campaign.Current.Models.AgeModel.HeroComesOfAge).ToDays;
                days = (int)child.BirthDay.ElapsedDaysUntilNow;
                if (days == comeOfAgeDays)
                {
                    if (child == Hero.MainHero || Hero.MainHero.Children.Contains(child) || (null != Hero.MainHero.Father && Hero.MainHero.Father.Children.Contains(child)))
                    {
                        TextObject textObject = GameTexts.FindText("suems_children_grow_up_to_hero_age", null);
                        StringHelpers.SetCharacterProperties("SUE_HERO", child.CharacterObject, null, textObject);
                        InformationManager.AddQuickInformation(textObject, 0, null, "event:/ui/notification/quest_finished");
                    }
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
