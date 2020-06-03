using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using Helpers;
using System.Reflection;
using System.Collections.Generic;
using TaleWorlds.Library;
using System.Linq;


namespace SueMoreSpouses
{
    class HeroRlationOperation
    {
        public static void ChangeCompanionToSpouse(Hero hero)
        {
            if (null == hero || !hero.IsPlayerCompanion) return;
            if (Hero.MainHero.Spouse == hero || Hero.MainHero.ExSpouses.Contains(hero)) return;

          
            //去掉它的伙伴属性
            hero.CompanionOf = null;
            OccuptionChange.ChangeOccupationToLord(hero.CharacterObject);
            //_nobles 添加到贵族列表
            MarryHero(hero);

            hero.IsNoble = true;
            FieldInfo fieldInfo = Clan.PlayerClan.GetType().GetField("_nobles", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (null != fieldInfo)
            {
                Object obj = fieldInfo.GetValue(Clan.PlayerClan);
                if (null != obj)
                {
                    List<Hero> list = (List<Hero>)obj;
                    if (!list.Contains(hero))
                    {
                        list.Add(hero);
                    }

                }
            }

        }


        public static void ChangePrisonerLordToSpouse(Hero hero)
        {
            if (null == hero && hero.CharacterObject.Occupation != Occupation.Lord) return;
            DealLordForClan(hero);
           // List<Kingdom>.Enumerator enumerator = Kingdom.All.GetEnumerator();
           /// List<Hero> heroes = Hero.FindAll((obj) => obj.IsFemale).ToList();
            ChangePrisonerToParty(hero);
            MarryHero(hero);
        }

        public static void ChangePrisonerWandererToSpouse(Hero hero)
        {
            if (null == hero && hero.CharacterObject.Occupation != Occupation.Wanderer) return;

            ChangePrisonerToParty(hero);
            OccuptionChange.ChangeOccupationToLord(hero.CharacterObject);
            MarryHero(hero);
        }

        public static void ChangePrisonerLordToFamily(Hero hero)
        {
            if (null == hero && hero.CharacterObject.Occupation != Occupation.Lord) return;
            DealLordForClan(hero);
             //OccuptionChange.ChangeToWanderer(hero.CharacterObject);
            ChangePrisonerToParty(hero);
          
        }

        private static void DealLordForClan(Hero hero)
        {
            Clan clan = hero.Clan;
            if (hero.Clan.Leader == hero)
            {
                List<Hero> others = clan.Heroes.Where((obj) => (obj != hero && obj.IsAlive)).ToList();
                if (others.Count() > 0)
                {
                    Hero target = others.GetRandomElement();
                    ChangeClanLeaderAction.ApplyWithSelectedNewLeader(clan, target);

                    TextObject textObject = GameTexts.FindText("sue_more_spouses_clan_leave", null);
                    StringHelpers.SetCharacterProperties("SUE_HERO", hero.CharacterObject, null, textObject);
                    InformationManager.AddQuickInformation(textObject, 0, null, "event:/ui/notification/quest_finished");

                    textObject = GameTexts.FindText("sue_more_spouses_clan_change", null);
                    StringHelpers.SetCharacterProperties("SUE_HERO", target.CharacterObject, null, textObject);
                    InformationManager.AddQuickInformation(textObject, 0, null, "event:/ui/notification/quest_finished");
                }
                else
                {
                    dealKindomLeader(clan, hero);
                    List<Settlement> settlements = clan.Settlements.ToList();
                    settlements.ForEach((settlement) => ChangeOwnerOfSettlementAction.ApplyByDestroyClan(settlement, Hero.MainHero));
                    DestroyClanAction.Apply(clan);
                }
              //  InformationManager.DisplayMessage(new InformationMessage("hero.Clan.Leader  Change"));
            }
            List<Hero> aliveOthers = clan.Heroes.Where((obj) => (obj != hero && obj.IsAlive)).ToList();
           // InformationManager.DisplayMessage(new InformationMessage("orginal clan.name" + clan.Name  + " count of  the heros that alive is  " + aliveOthers.Count));
        
           //  AddCompanionAction.Apply(Clan.PlayerClan, hero);
            hero.Clan = Clan.PlayerClan;
            //aliveOthers = clan.Heroes.Where((obj) => (obj != hero && obj.IsAlive)).ToList();
           //  InformationManager.DisplayMessage(new InformationMessage("change clan.name" + clan.Name + "  count of  the heros that alive is  " + aliveOthers.Count));


          
        }

        private static void dealKindomLeader(Clan clan, Hero hero)
        {
            if (clan.Kingdom.Leader == hero)
            {
                //InformationManager.DisplayMessage(new InformationMessage("clan.Kingdom.Leader  Change"));
                List<Clan> oteherClans = clan.Kingdom.Clans.Where((obj) => obj != clan && !obj.IsEliminated).ToList();
                if (oteherClans.Count > 0)
                {
                    IEnumerable<Clan> sortedStudents = from item in oteherClans
                                                       orderby item.Renown descending
                                                       select item;
                    Clan targetClan = sortedStudents.First();
                    clan.Kingdom.RulingClan = targetClan;


                    TextObject textObject = GameTexts.FindText("sue_more_spouses_kindom_leader_change", null);
                    StringHelpers.SetCharacterProperties("SUE_HERO", targetClan.Leader.CharacterObject, null, textObject);
                    InformationManager.AddQuickInformation(textObject, 0, null, "event:/ui/notification/quest_finished");
                }
                else
                {
                    //InformationManager.DisplayMessage(new InformationMessage("clan.Kingdom  destory"));
                    DestroyKingdomAction.Apply(clan.Kingdom);
                }

            }
        }

        private static void ChangePrisonerToParty(Hero hero)
        {
            if (null == hero || !hero.IsPrisoner || !MobileParty.MainParty.PrisonRoster.Contains(hero.CharacterObject)) return;
         
            MobileParty.MainParty.PrisonRoster.RemoveIf((cobj) => (cobj.Character.IsHero && cobj.Character.HeroObject == hero));
            hero.ChangeState(Hero.CharacterStates.Active);
            MobileParty.MainParty.AddElementToMemberRoster(hero.CharacterObject, 1);
        }

        private static void MarryHero(Hero hero)
        {
            if (Hero.MainHero.Spouse == hero || Hero.MainHero.ExSpouses.Contains(hero)) return;

            MarriageAction.Apply(Hero.MainHero, hero);
            RemoveRepeatExspouses();
            TextObject textObject = GameTexts.FindText("sue_more_spouses_marry_target", null);
            StringHelpers.SetCharacterProperties("SUE_HERO", hero.CharacterObject, null, textObject);
            InformationManager.AddQuickInformation(textObject, 0, null, "event:/ui/notification/quest_finished");
        }


        private static void RemoveRepeatExspouses()
        {
            if (Hero.MainHero.ExSpouses.Count > 2)
            {
                FieldInfo fieldInfo = Hero.MainHero.GetType().GetField("_exSpouses", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                FieldInfo fieldInfo2 = Hero.MainHero.GetType().GetField("ExSpouses", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (null == fieldInfo || null == fieldInfo2) return;
                List<Hero> heroes = (List<Hero>)fieldInfo.GetValue(Hero.MainHero);
                MBReadOnlyList<Hero> heroes2 = (MBReadOnlyList<Hero>)fieldInfo2.GetValue(Hero.MainHero);
                //heroes.D
                heroes = heroes.Distinct(new DistinctTest<Hero>()).ToList();
                if (heroes.Contains(Hero.MainHero.Spouse))
                {
                    heroes.Remove(Hero.MainHero.Spouse);
                }
                fieldInfo.SetValue(Hero.MainHero, heroes);
                heroes2 = new MBReadOnlyList<Hero>(heroes);
                fieldInfo2.SetValue(Hero.MainHero, heroes2);
            }
        }

        class DistinctTest<TModel> : IEqualityComparer<TModel>
        {
            public bool Equals(TModel x, TModel y)
            {
                Hero t = x as Hero;
                Hero tt = y as Hero;
                if (t != null && tt != null) return t.StringId == tt.StringId;
                return false;
            }

            public int GetHashCode(TModel obj)
            {
                return obj.ToString().GetHashCode();
            }
        }

    }
}
