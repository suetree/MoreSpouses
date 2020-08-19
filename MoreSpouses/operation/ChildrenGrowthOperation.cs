using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
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
                }
            }

        }
    }
}
