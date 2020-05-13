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

namespace MoreSpouses
{

    class SpouseFromPrisonerBehavior : CampaignBehaviorBase
    {

        int acceptFlag;

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));

        }

        public override void SyncData(IDataStore dataStore)
        {

        }

        private void OnSessionLaunched(CampaignGameStarter campaignGameStarter)
        {


            //修正过去BUG 导致 英雄状态还是囚犯状态
            campaignGameStarter.AddPlayerLine("conversation_prisoner_chat_start", "start", "close_window", GameTexts.FindText("sue_more_spouses_female_spouses_status_prisoner_become_active", null).ToString(), new ConversationSentence.OnConditionDelegate(isSpouseAndPrisoner), new ConversationSentence.OnConsequenceDelegate(changeSpousePrisonerStatusToActive), 1000, null, null);
            //campaignGameStarter.AddDialogLine("conversation_prisoner_chat_fix", "female_prisoner_fix_status", "", GameTexts.FindText("sue_more_spouses_thanks", null).ToString(), new ConversationSentence.OnConditionDelegate(isSpouseAndPrisoner), new ConversationSentence.OnConsequenceDelegate(changeSpousePrisonerStatusToActive), 1000, null);


            campaignGameStarter.AddPlayerLine("conversation_prisoner_chat_player", "prisoner_recruit_start_player", "female_prisoner_choice", GameTexts.FindText("sue_more_spouses_female_prisoner_become_spouse_ask", null).ToString(), new ConversationSentence.OnConditionDelegate(isSuitableFemale), new ConversationSentence.OnConsequenceDelegate(ConversationResult), 1000, null, null);
            campaignGameStarter.AddPlayerLine("conversation_prisoner_chat_player", "hero_main_options", "female_prisoner_choice", GameTexts.FindText("sue_more_spouses_female_prisoner_become_spouse_ask", null).ToString(), new ConversationSentence.OnConditionDelegate(isSuitableFemale), new ConversationSentence.OnConsequenceDelegate(ConversationResult), 1000, null, null);
            campaignGameStarter.AddDialogLine("female_prisoner_choice_result", "female_prisoner_choice", "female_prisoner_choice_accept", GameTexts.FindText("sue_more_spouses_female_prisoner_become_spouse_result_accept", null).ToString(), new ConversationSentence.OnConditionDelegate(() => this.acceptFlag == 1), null, 100, null);
            campaignGameStarter.AddDialogLine("female_prisoner_choice_result", "female_prisoner_choice", "female_prisoner_choice_refuse", GameTexts.FindText("sue_more_spouses_female_prisoner_become_spouse_result_refuse", null).ToString(), new ConversationSentence.OnConditionDelegate(() => this.acceptFlag != 1), null, 100, null);
        }


        public void changeSpousePrisonerStatusToActive()
        {
            Hero target = Hero.OneToOneConversationHero;
            if (null != target)
            {
                target.ChangeState(Hero.CharacterStates.Active);
            }

        }

        public bool isSpouseAndPrisoner()
        {
            Hero target = Hero.OneToOneConversationHero;
            return null != target && target.IsPrisoner && (target == Hero.MainHero.Spouse || Hero.MainHero.ExSpouses.Contains(target));
        }


        public bool isSuitableFemale()
        {
            Hero target = Hero.OneToOneConversationHero;
            return null != target &&
                target.IsFemale && target.Age < 35 && target.Spouse == null
                && (MobileParty.MainParty.PrisonRoster.Contains(target.CharacterObject) ||
                target.IsPlayerCompanion
                );
        }

        public void ConversationResult()
        {

            ConversationRandom();
            if (acceptFlag == 1)
            {
                Hero hero = Hero.OneToOneConversationHero;

                if (Hero.MainHero.Spouse == hero || Hero.MainHero.ExSpouses.Contains(hero))
                {
                    return;
                }

                if (hero.IsPrisoner)
                {
                    MobileParty.MainParty.PrisonRoster.RemoveIf((cobj) => (cobj.Character.IsHero && cobj.Character.HeroObject == hero));
                    hero.ChangeState(Hero.CharacterStates.Active);
                    MobileParty.MainParty.AddElementToMemberRoster(hero.CharacterObject, 1);

                }

                if (hero.IsPlayerCompanion && null != hero.CharacterObject && null != hero.CharacterObject.GetType())
                {

                    SetOccupationToLord(hero);
                    hero.IsNoble = true;
                    //去掉它的伙伴属性
                    hero.CompanionOf = null;
                }


                MarriageAction.Apply(Hero.MainHero, hero);

                MBReadOnlyList<Hero> exSpouses = Hero.MainHero.ExSpouses;
                removeRepeatExspouses();
                TextObject textObject = GameTexts.FindText("sue_more_spouses_marry_target", null);
                StringHelpers.SetCharacterProperties("SUE_HERO", hero.CharacterObject, null, textObject, false);
                InformationManager.AddQuickInformation(textObject, 0, null, "event:/ui/notification/quest_finished");
            }
        }

        public void ConversationRandom()
        {
            acceptFlag = new Random().Next(2);
            //InformationManager.DisplayMessage(new InformationMessage("acceptFlag="+ acceptFlag));
        }

        private static void SetOccupationToLord(Hero hero)
        {
            if (hero.CharacterObject.Occupation == Occupation.Lord) return;

            FieldInfo fieldInfo = hero.CharacterObject.GetType().GetField("_originCharacter", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            CharacterObject originalCharacterObject = (CharacterObject)fieldInfo.GetValue(hero.CharacterObject);
            PropertyInfo propertyInfo = typeof(CharacterObject).GetProperty("Occupation");
            if (null != propertyInfo && null != propertyInfo.DeclaringType)
            {
                propertyInfo = propertyInfo.DeclaringType.GetProperty("Occupation");
                if (null != propertyInfo)
                {
                    propertyInfo.SetValue(hero.CharacterObject, Occupation.Lord, null);
                }
            }
            fieldInfo.SetValue(hero.CharacterObject, CharacterObject.PlayerCharacter);
            //main_hero
        }

        private void removeRepeatExspouses()
        {
            if(Hero.MainHero.ExSpouses.Count > 2)
            {
                FieldInfo fieldInfo = Hero.MainHero.GetType().GetField("_exSpouses", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                FieldInfo fieldInfo2 = Hero.MainHero.GetType().GetField("ExSpouses", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);

                if (null == fieldInfo || null == fieldInfo2) return;

                List<Hero> heroes = (List<Hero>)fieldInfo.GetValue(Hero.MainHero);
                MBReadOnlyList<Hero> heroes2 = (MBReadOnlyList<Hero>)fieldInfo2.GetValue(Hero.MainHero);
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
