using Helpers;
using MCM.Abstractions.Settings.Base.Global;
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
            if (!AttributeGlobalSettings<MoreSpouseSetting>.Instance.ChildrenFastGrowUpEnable) return;
            if (null == child) return;
            if (child.Age < AttributeGlobalSettings<MoreSpouseSetting>.Instance.ChildrenStopGrowUpAge)
            {
                int lastAge = (int)child.Age;
                int circle = AttributeGlobalSettings<MoreSpouseSetting>.Instance.ChildrenGrowthCycleInDays;
                int days = (int)child.BirthDay.ElapsedDaysUntilNow;
                if (days % circle == 0)
                {
                    child.BirthDay = CampaignTime.Days(child.BirthDay.GetDayOfYear) +  CampaignTime.Years(child.BirthDay.GetYear - 1);
                }

                if ((lastAge + 1) == Campaign.Current.Models.AgeModel.HeroComesOfAge)
                {
                    TextObject textObject = GameTexts.FindText("sue_more_spouses_children_grow_up_to_hero_age", null);
                    StringHelpers.SetCharacterProperties("SUE_HERO", child.CharacterObject, null, textObject);
                    InformationManager.AddQuickInformation(textObject, 0, null, "event:/ui/notification/quest_finished");
                    // GrowUpForSkill(child);
                }
            }

        }

        private  static void  GrowUpForSkill(Hero hero)
        {
            hero.ClearSkills();
            hero.HeroDeveloper.ClearHeroLevel();
            float fatherInheritDivider = 0;
            float motherInheritDivider = 0;
            if (hero.IsFemale == true)
            {
                fatherInheritDivider = 0.4f;
                motherInheritDivider = 0.6f;
            }
            else
            {
                fatherInheritDivider = 0.6f;
                motherInheritDivider = 0.4f;
            }

            Hero InheritFather = hero.Father != null ? hero.Father : hero;
            Hero InheritMother = hero.Mother != null ? hero.Mother : hero;

           int  skillTimes = (new Random().Next(2) == 1) ? 2 : 1;

            int randomSkillValue = new Random().Next(100) + new Random().Next(5) * 10;

            foreach (SkillObject skillIT in DefaultSkills.GetAllSkills())
            {
                int sikillValue = (int)(InheritFather.GetSkillValue(skillIT) * fatherInheritDivider +
                InheritMother.GetSkillValue(skillIT) * motherInheritDivider);
                hero.HeroDeveloper.ChangeSkillLevel(skillIT, sikillValue * skillTimes + randomSkillValue, false);
                hero.HeroDeveloper.TakeAllPerks(skillIT);
            }
            hero.Level = 0;
            hero.HeroDeveloper.UnspentFocusPoints = 10;
            hero.HeroDeveloper.UnspentAttributePoints = 10;

            if (skillTimes > 1)
            {
                InformationManager.AddQuickInformation(new TextObject($"你的孩子{hero.Name}从父母那里继承了部分能力, 在许多方面都突出常人"),
              0, null, "event:/ui/notification/quest_finished");
            }
        }


    }
}
