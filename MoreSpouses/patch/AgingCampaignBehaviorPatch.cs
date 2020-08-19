using HarmonyLib;
using SueMoreSpouses.operation;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace SueMoreSpouses.patch
{
    class AgingCampaignBehaviorPatch
    {

        [HarmonyPatch(typeof(AgingCampaignBehavior), "DailyTick")]
        public class AgingCampaignBehaviorDailyTickPatch
        {
           
            static void Prefix()
            {

				// 支持玩家兄弟的成长
				if (null != Hero.MainHero.Father && Hero.MainHero.Father.Children.Count > 0)
				{
					Hero.MainHero.Father.Children.ForEach((Hero child) => ChildrenGrowthOperation.FastGrowth(child));
				}

				if (Hero.MainHero.Children.Count != 0)
                {
                   
                    Hero.MainHero.Children.ForEach((Hero child) => ChildrenGrowthOperation.FastGrowth(child));
                }
            }

        }

        [HarmonyPatch(typeof(HeroCreationCampaignBehavior), "DeriveSkillsFromTraits")]
        public class OnHeroComesOfAgePatch
        {
            static void Postfix(Hero hero, CharacterObject templateCharacter = null)
            {
                if (MoreSpouseSetting.Instance.SettingData.ChildrenSkillFixEnable)
                {
                    int zeroCount = 0;
                    foreach (SkillObject skillIT in DefaultSkills.GetAllSkills())
                    {
                        if (hero.GetSkillValue(skillIT) <= 10)
                        {
                            zeroCount++;
                        }
                    }
                    if (zeroCount >= (DefaultSkills.GetAllSkills().Count() - 3))
                    {
                        GrowUpForFixSkill(hero);
                    }
                }

            }

            private static void GrowUpForFixSkill(Hero hero)
            {

                // InformationManager.DisplayMessage(new InformationMessage("MoreSpouse GrowUpForFixSkill " + hero.Name));
                if (hero == Hero.MainHero)
                {
                    return;
                }

                hero.ClearSkills();
                
                // hero.HeroDeveloper.ClearHeroLevel();
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

                float skillTimes = (new Random().Next(2) == 1) ? 1.1f : 1f;

                if (Hero.MainHero.Children.Contains(hero) || Hero.MainHero.Father.Children.Contains(hero))
                {
                   
                    int randomTims = new Random().Next(5);
                    if (randomTims == 1)
                    {
                        skillTimes = skillTimes * 1.5f;
                        InformationManager.AddQuickInformation(new TextObject($"Your children {hero.Name} get more power"),
                        0, null, "event:/ui/notification/quest_finished");
                    }
                   
                }
              

                foreach (SkillObject skillIT in DefaultSkills.GetAllSkills())
                {
                    int sikillValue = (int)(InheritFather.GetSkillValue(skillIT) * fatherInheritDivider + InheritMother.GetSkillValue(skillIT) * motherInheritDivider);
                    hero.HeroDeveloper.ChangeSkillLevel(skillIT, (int)(sikillValue * skillTimes), false);
                    hero.HeroDeveloper.TakeAllPerks(skillIT);
                }
                hero.Level = 0;
                hero.HeroDeveloper.UnspentFocusPoints = 20;
                hero.HeroDeveloper.UnspentAttributePoints = 20;

            }



        }
    }
}
