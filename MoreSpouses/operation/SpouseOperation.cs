using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SueMoreSpouses.Operation
{
    class SpouseOperation
    {

        public static  void GetPregnancyForHero(Hero father, Hero mother)
        {
            MakePregnantAction.Apply(mother);
            PregnancyCampaignBehavior pregnancyCampaignBehavior = Campaign.Current.GetCampaignBehavior<PregnancyCampaignBehavior>();
            CampaignTime campaignTime = CampaignTime.DaysFromNow(Campaign.Current.Models.PregnancyModel.PregnancyDurationInDays);
            if (MoreSpouseSetting.Instance.SettingData.ExspouseGetPregnancyEnable)
            {
                campaignTime = CampaignTime.DaysFromNow(MoreSpouseSetting.Instance.SettingData.ExspouseGetPregnancyDurationInDays);
            }
            Type type = pregnancyCampaignBehavior.GetType();
            FieldInfo _heroPregnancies = type.GetField("_heroPregnancies", BindingFlags.NonPublic | BindingFlags.Instance);
            IList list = (IList)_heroPregnancies.GetValue(pregnancyCampaignBehavior);
            object[] parameters = new object[3] { mother, father, campaignTime };
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
        }

        public static void SetPrimarySpouse(Hero hero)
        {
            if ( Hero.MainHero.Spouse != hero )
            {
                Hero.MainHero.Spouse = hero;
                hero.Spouse = Hero.MainHero;
                RemoveRepeatExspouses(Hero.MainHero, Hero.MainHero.Spouse);
                RemoveRepeatExspouses(hero, hero.Spouse);
            }
          
        }

        public static void Divorce(Hero hero)
        {
            if (null != hero)
            {
                if (Hero.MainHero.Spouse == hero)
                {
                    Hero.MainHero.Spouse = null;
                    hero.Spouse = null;
                }

                if (Hero.MainHero.ExSpouses.Contains(hero))
                {
                    RemoveRepeatExspouses(Hero.MainHero, hero);
                    RemoveRepeatExspouses(hero, Hero.MainHero);
                }

                hero.CompanionOf = Clan.PlayerClan;


            }
        }


        public static void RemoveRepeatExspouses(Hero hero, Hero target)
        {
            if (Hero.MainHero.ExSpouses.Count > 0)
            {
                FieldInfo fieldInfo = hero.GetType().GetField("_exSpouses", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                FieldInfo fieldInfo2 = hero.GetType().GetField("ExSpouses", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (null == fieldInfo || null == fieldInfo2) return;
                List<Hero> heroes = (List<Hero>)fieldInfo.GetValue(hero);
                MBReadOnlyList<Hero> heroes2 = (MBReadOnlyList<Hero>)fieldInfo2.GetValue(hero);
                //heroes.D
                heroes = heroes.Distinct(new DistinctSpouse<Hero>()).ToList();
                if (heroes.Contains(target))
                {
                    heroes.Remove(target);
                }
                fieldInfo.SetValue(hero, heroes);
                heroes2 = new MBReadOnlyList<Hero>(heroes);
                fieldInfo2.SetValue(hero, heroes2);
            }
        }
    }
}
