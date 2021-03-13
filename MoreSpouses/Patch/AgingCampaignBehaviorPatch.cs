using HarmonyLib;
using SueMoreSpouses.Operation;
using SueMoreSpouses.setting;
using SueMoreSpouses.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.GauntletUI.Widgets.Inventory;

namespace SueMoreSpouses.Patch
{
    class AgingCampaignBehaviorPatch
    {

        [HarmonyPatch(typeof(AgingCampaignBehavior), "DailyTickHero")]
        public class AgingCampaignBehaviorDailyTickPatch
        {
           
            static void Prefix(Hero hero)
            {

                if (IsInScope(hero))
                {
                    if (hero.Age < MoreSpouseSetting.Instance.SettingData.ChildrenFastGrowtStopGrowUpAge
                        && MoreSpouseSetting.Instance.SettingData.ChildrenFastGrowthEnable)
                    {
                        ChildrenGrowthOperation.FastGrowth(hero);
                    }
                }

            }

            private static bool IsInScope(Hero hero)
            {
                bool can = false;
                if (MoreSpouseSetting.Instance.SettingData.ChildrenFastGrowthEnable && null != MoreSpouseSetting.Instance.SettingData.ChildrenFastGrowUpScope)
                {
                    ValueNamePair scope = MoreSpouseSetting.Instance.SettingData.ChildrenFastGrowUpScope;
                    switch (scope.Value)
                    {
                        case 0:
                            if (hero == Hero.MainHero || Hero.MainHero.Children.Contains(hero) || (null != Hero.MainHero.Father && Hero.MainHero.Father.Children.Contains(hero)))
                            {
                                can = true;
                            }
                            break;
                        case 1:
                            if (null != hero.Clan && hero.Clan == Hero.MainHero.Clan)
                            {
                                can = true;
                            }
                            break;
                        case 2:
                            Kingdom kindom1 = Hero.MainHero.MapFaction as Kingdom;
                            Kingdom kindom2 = hero.MapFaction as Kingdom;
                            if (null != kindom1 &&  null != kindom2 && kindom1 == kindom2)
                            {
                                can = true;
                            }
                            break;
                        case 3:
                            can = true;
                            break;
                    }

                }
                return can;
            }

        }

        //[HarmonyPatch(typeof(AgingCampaignBehavior), "DailyTickHero")]
        public class AgingCampaignBehaviorDailyTick2Patch
        {

            static void Postfix(ref AgingCampaignBehavior __instance, Hero current)
            {

                if (!current.IsTemplate && current.IsAlive)
                {
                    if ((int)current.BirthDay.ElapsedDaysUntilNow == (int)CampaignTime.Years((float)Campaign.Current.Models.AgeModel.HeroComesOfAge).ToDays)
                    {
                        if (current.HeroState != Hero.CharacterStates.Active)
                        {
                            CampaignEventDispatcher dispatcher = GameComponent.CampaignEventDispatcher();
                            if (null != dispatcher)
                            {
                                ReflectUtils.ReflectMethodAndInvoke("OnHeroComesOfAge", dispatcher, new Object[] { current });
                            }
                        }
                    }
                    else if ((int)current.BirthDay.ElapsedDaysUntilNow == (int)CampaignTime.Years((float)Campaign.Current.Models.AgeModel.BecomeTeenagerAge).ToDays)
                    {
                        // CampaignEventDispatcher.Instance.OnHeroReachesTeenAge(current);
                    }
                    else if ((int)current.BirthDay.ElapsedDaysUntilNow == (int)CampaignTime.Years((float)Campaign.Current.Models.AgeModel.BecomeChildAge).ToDays)
                    {
                        // CampaignEventDispatcher.Instance.OnHeroGrowsOutOfInfancy(current);
                    }
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

                float skillTimes = (new Random().Next(2) == 1) ? 1.5f : 1f;

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
