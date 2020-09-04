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
using SueMoreSpouses.operation;
using SueMoreSpouses.utils;

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
            RefreshClanPanelList(hero);

        }
        public static void NPCToSouse(CharacterObject character, CampaignGameStarter campaignGameStarter)
        {
            Hero hero = DealNPC(character, campaignGameStarter);
            if(null != hero)
            {
                //去掉它的伙伴属性
                hero.CompanionOf = null;
                OccuptionChange.ChangeOccupationToLord(hero.CharacterObject);
                MarryHero(hero);
                hero.IsNoble = true;
                RefreshClanPanelList(hero);
            }
           
        }

        public static void NPCToCompanion(CharacterObject character, CampaignGameStarter campaignGameStarter)
        {
            // OccuptionChange.ChangeOccupationToLord(hero.CharacterObject);
             Hero hero = DealNPC(character, campaignGameStarter);
             OccuptionChange.ChangeOccupationToLord(hero.CharacterObject);
             hero.IsNoble = true;
            if (!MobileParty.MainParty.MemberRoster.Contains(hero.CharacterObject))
            {
                MobileParty.MainParty.MemberRoster.AddToCounts(hero.CharacterObject, 1);
            }
            AddCompanionAction.Apply(Clan.PlayerClan, hero);

        }

        private static Hero DealNPC(CharacterObject target, CampaignGameStarter campaignGameStarter)
        {
            Hero hero = null;
            if (null != target)
            {
                CharacterObject character = CharacterObject.OneToOneConversationCharacter;
                hero = HeroCreator.CreateSpecialHero(character, null, Clan.PlayerClan, Clan.PlayerClan);
              
                hero.ChangeState(Hero.CharacterStates.Active);
                hero.CacheLastSeenInformation(hero.HomeSettlement, true);
                hero.SyncLastSeenInformation();
                HeroUtils.InitHeroForNPC(hero);
              

                AddHeroToPartyAction.Apply(hero, MobileParty.MainParty, true);
                CampaignEventDispatcher.Instance.OnHeroCreated(hero, false);
                ConversationUtils.ChangeCurrentCharaObject(campaignGameStarter, hero);


            }
            return hero;
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
            //1.4.3 结婚后，第二个英雄状态会变成逃亡状态，并且会被所在部队移除, 所在部队还会解散。

            Hero mainSpouse = Hero.MainHero.Spouse;

            bool needAddPlayerTroop = true;
            if (Hero.MainHero.PartyBelongedTo.MemberRoster.Contains(hero.CharacterObject))
            {
                Hero.MainHero.PartyBelongedTo.MemberRoster.RemoveTroop(hero.CharacterObject, 1);
            }
            if (hero.PartyBelongedTo != null)
            {
                MobileParty partyBelongedTo = hero.PartyBelongedTo;
                partyBelongedTo.MemberRoster.RemoveTroop(hero.CharacterObject, 1);
                partyBelongedTo.RemoveParty();
                //partyBelongedTo.Party.Owner = partyBelongedTo.MemberRoster.First().Character;
            }



            if (null == hero.Clan)
            {
                hero.Clan = Hero.MainHero.Clan;
            }
            MarriageAction.Apply(Hero.MainHero, hero);
            hero.ChangeState(Hero.CharacterStates.Active);
            if (needAddPlayerTroop)
            {
                Hero.MainHero.PartyBelongedTo.MemberRoster.AddToCounts(hero.CharacterObject, 1);
            }
         

            SpouseOperation.RemoveRepeatExspouses(Hero.MainHero, Hero.MainHero.Spouse);

            TextObject textObject = GameTexts.FindText("sue_more_spouses_marry_target", null);
            StringHelpers.SetCharacterProperties("SUE_HERO", hero.CharacterObject, null, textObject);
            InformationManager.AddQuickInformation(textObject, 0, null, "event:/ui/notification/quest_finished");

            if (null  != mainSpouse)
            {
                SpouseOperation.SetPrimarySpouse(mainSpouse);
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



        public static  void DealApplyByFire(Clan clan, Hero hero)
        {
            if (null == hero.LastSeenPlace)
            {
                hero.CacheLastSeenInformation(hero.HomeSettlement, true);
                hero.SyncLastSeenInformation();
            }
            RemoveCompanionAction.ApplyByFire(Hero.MainHero.Clan, hero);
        }

        public static void RefreshClanPanelList(Hero hero)
        {
            //下面逻辑是1.4.2以前
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

            //1.4.3
            FieldInfo fieldInfo2 = Clan.PlayerClan.GetType().GetField("_lords", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (null != fieldInfo2)
            {
                Object obj = fieldInfo2.GetValue(Clan.PlayerClan);
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

    }
}
