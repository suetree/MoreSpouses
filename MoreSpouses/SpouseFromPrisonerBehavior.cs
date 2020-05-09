using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.Core;

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
           
            campaignGameStarter.AddPlayerLine("conversation_prisoner_chat_player", "prisoner_recruit_start_player", "female_prisoner_choice", GameTexts.FindText("sue_more_spouses_female_prisoner_become_spouse_ask", null).ToString(), new ConversationSentence.OnConditionDelegate(IsFemalePrisoner), new ConversationSentence.OnConsequenceDelegate(ConversationResult), 1000, null, null);
            campaignGameStarter.AddDialogLine("female_prisoner_choice_result", "female_prisoner_choice", "female_prisoner_choice_accept", GameTexts.FindText("sue_more_spouses_female_prisoner_become_spouse_result_accept", null).ToString(), new ConversationSentence.OnConditionDelegate(() => this.acceptFlag == 1), null, 100, null);
            campaignGameStarter.AddDialogLine("female_prisoner_choice_result", "female_prisoner_choice", "female_prisoner_choice_refuse", GameTexts.FindText("sue_more_spouses_female_prisoner_become_spouse_result_refuse", null).ToString(), new ConversationSentence.OnConditionDelegate(() => this.acceptFlag != 1), null, 100, null);
        }

        public bool IsFemalePrisoner()
        {
            CharacterObject taget = CharacterObject.OneToOneConversationCharacter;
            return MobileParty.MainParty.PrisonRoster.Contains(taget) 
                && taget.IsHero && taget.IsFemale && taget.Age < 35 && taget.HeroObject.Spouse == null;
        }

        public void ConversationResult()
        {
            acceptFlag = new Random().Next(2);
            if (acceptFlag == 1)
            {
                Hero hero = Hero.OneToOneConversationHero;
                MarriageAction.Apply(Hero.MainHero, hero);
                MobileParty.MainParty.AddElementToMemberRoster(hero.CharacterObject, 1);
                MobileParty.MainParty.PrisonRoster.RemoveIf((cobj) => (cobj.Character.IsHero && cobj.Character.HeroObject == hero));
                InformationManager.AddQuickInformation(
                     new TaleWorlds.Localization.TextObject($"{hero.Name} become your spouse"),  0, null, "event:/ui/notification/quest_finished");

            }


        }
    }
}
