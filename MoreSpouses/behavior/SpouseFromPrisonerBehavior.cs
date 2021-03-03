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
using SueMoreSpouses;
using SueMoreSpouses.Utils;
using SueMoreSpouses.Data;
using TaleWorlds.SaveSystem;

namespace SueMoreSpouses.Behavior
{

    class SpouseFromPrisonerBehavior : CampaignBehaviorBase
    {

        public override void RegisterEvents()
        {
            CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));

        }

        public override void SyncData(IDataStore dataStore)
        {
           
        }
        CampaignGameStarter currStarter;
        private void OnSessionLaunched(CampaignGameStarter starter)
        {

            currStarter = starter;
            InformationManager.DisplayMessage(new InformationMessage("MoreSpouses OnSessionLaunched"));

            //修正过去BUG 导致 英雄状态还是囚犯状态 
            starter.AddPlayerLine("conversation_prisoner_chat_start", "start", "close_window", LoactionText("sue_more_spouses_female_spouses_status_prisoner_become_active"), new ConversationSentence.OnConditionDelegate(isSpouseAndPrisoner), new ConversationSentence.OnConsequenceDelegate(changeSpousePrisonerStatusToActive), 100, null, null);
            //campaignGameStarter.AddDialogLine("conversation_prisoner_chat_fix", "female_prisoner_fix_status", "", GameTexts.FindText("sue_more_spouses_thanks", null).ToString(), new ConversationSentence.OnConditionDelegate(isSpouseAndPrisoner), new ConversationSentence.OnConsequenceDelegate(changeSpousePrisonerStatusToActive), 1000, null);

            //同伴开始
            starter.AddPlayerLine("conversation_prisoner_chat_player", "hero_main_options", "sue_more_spouses_companion_become_spouse", LoactionText("sue_more_spouses_companion_become_spouse"), Condition(IsPlayerCompanionAndCanBecomeSpouse), null, 100, null, null);
            starter.AddDialogLine("sue_more_spouses_companion_choice_result", "sue_more_spouses_companion_become_spouse", "sue_more_spouses_companion_become_spouse_accept", LoactionText("sue_more_spouses_companion_become_spouse_accept"), null, Result(() => {
                Hero target = Hero.OneToOneConversationHero;
                HeroRlationOperation.ChangeCompanionToSpouse(target);
            }), 100, null);


            //囚禁事件的
            starter.AddPlayerLine("sms_tavernmaid_chat_player", "cprostitute_talk_00_response", "sue_more_spouses_tavernmaid_start", LoactionText("sue_more_spouses_npc_becomes_spouse"), Condition(IsNormalNPC), null, 100, null, null);
            starter.AddPlayerLine("sms_tavernmaid_chat_player", "cprostitute_talk_02_response", "sue_more_spouses_tavernmaid_start", LoactionText("sue_more_spouses_npc_becomes_spouse"), Condition(IsNormalNPC), null, 100, null, null);
            starter.AddPlayerLine("sms_tavernmaid_chat_player", "cprostitute_talk_01_response", "sue_more_spouses_tavernmaid_start", LoactionText("sue_more_spouses_npc_becomes_spouse"), Condition(IsNormalNPC), null, 100, null, null);
            starter.AddPlayerLine("sms_tavernmaid_chat_player", "tcustomer_00", "sue_more_spouses_tavernmaid_start", LoactionText("sue_more_spouses_npc_becomes_spouse"), Condition(IsNormalNPC), null, 100, null, null);
            starter.AddPlayerLine("sms_tavernmaid_chat_player", "ccustomer_00", "sue_more_spouses_tavernmaid_start", LoactionText("sue_more_spouses_npc_becomes_spouse"), Condition(IsNormalNPC), null, 100, null, null);

            //城镇和农村 人员
            starter.AddPlayerLine("sms_tavernmaid_chat_player", "town_or_village_player", "sue_more_spouses_tavernmaid_start", LoactionText("sue_more_spouses_npc_becomes_spouse"), Condition(IsNormalNPC), null, 100, null, null);
            //酒馆女仆
            starter.AddPlayerLine("sms_tavernmaid_chat_player", "tavernmaid_talk", "sue_more_spouses_tavernmaid_start", LoactionText("sue_more_spouses_npc_becomes_spouse"), Condition(IsNormalNPC), null, 100, null, null);
            starter.AddDialogLine("sms_tavernmaid_ask_what", "sue_more_spouses_tavernmaid_start", "sms_tavernmaid_ask_what", LoactionText("sms_tavernmaid_ask_what"), null, null, 100, null);
            starter.AddPlayerLine("sms_tavernmaid_ask_what_result", "sms_tavernmaid_ask_what", "sms_tavernmaid_accept_result", LoactionText("sue_more_spouses_prisoner_punish_lord_become_spouse"), Condition(IsNormalNPC), Result(() => {
                CharacterObject character = CharacterObject.OneToOneConversationCharacter;
                if(null != character)
                {
                    HeroRlationOperation.NPCToSouse(character, currStarter);
                }
            }), 100, null, null);
            starter.AddPlayerLine("sms_tavernmaid_ask_what_result", "sms_tavernmaid_ask_what", "sms_tavernmaid_accept_result", LoactionText("sue_more_spouses_prisoner_punish_lord_become_wanderer_companion"), Condition(IsNormalNPC), Result(() => {
                CharacterObject character = CharacterObject.OneToOneConversationCharacter;
                if (null != character)
                {
                    // ConversationManager manager =  ConversationUtils.GetConversationManager(currStarter);
                    HeroRlationOperation.NPCToCompanion(character, currStarter);
                }
            }), 100, null, null);
            starter.AddRepeatablePlayerLine("sms_tavernmaid_ask_what_result", "sms_tavernmaid_ask_what", "close_window", GameTexts.FindText("sue_more_spouses_prisoner_punish_cancel", null).ToString(), null, null, 100, null);
            starter.AddDialogLine("sms_tavernmaid_accept_result", "sms_tavernmaid_accept_result", "sue_more_spouses_companion_become_spouse_accept", LoactionText("sms_tavernmaid_accept_result"), null, null, 100, null);



            //囚犯开始
            starter.AddPlayerLine("conversation_prisoner_chat_player", "prisoner_recruit_start_player", "sue_more_spouses_prisoner_punish_start", LoactionText("sue_more_spouses_prisoner_punish_start"), Condition(IsPrisioner),  null, 100, null, null);
            starter.AddDialogLine("sue_more_spouses_prisoner_beg_for_mercy", "sue_more_spouses_prisoner_punish_start", "sue_more_spouses_prisoner_beg_for_mercy", LoactionText("sue_more_spouses_prisoner_beg_for_mercy"), null, null, 100, null);
            
            starter.AddPlayerLine("sue_more_spouses_prisoner_punish_lord_become_spouse", "sue_more_spouses_prisoner_beg_for_mercy", "sue_more_spouses_prisoner_punish_result", LoactionText("sue_more_spouses_prisoner_punish_lord_become_spouse"), Condition(IsLord), Result(() => {
                ChangePrisonerLordToSpouse();
            }), 100, null, null) ;
            starter.AddPlayerLine("sue_more_spouses_prisoner_punish_lord_become_wanderer_companion", "sue_more_spouses_prisoner_beg_for_mercy", "sue_more_spouses_prisoner_punish_result", LoactionText("sue_more_spouses_prisoner_punish_lord_become_wanderer_companion"), Condition(IsLord), Result(() => {
                ChangePrisonerLordToFamily();
            }), 100, null, null);
            starter.AddRepeatablePlayerLine("sue_more_spouses_prisoner_punish_cancel", "sue_more_spouses_prisoner_beg_for_mercy", "close_window", GameTexts.FindText("sue_more_spouses_prisoner_punish_cancel", null).ToString(), null, null, 100, null);
          
            starter.AddDialogLine("sue_more_spouses_prisoner_punish_result", "sue_more_spouses_prisoner_punish_result", "sue_more_spouses_companion_become_spouse_accept", LoactionText("sue_more_spouses_prisoner_punish_accept"), null, null, 100, null);

            // starter.AddPlayerLine("sue_more_spouses_female_companion_become_spouse", "sue_more_spouses_female_companion_become_spouse", "female_prisoner_choice_accept", LoactionText("sue_more_spouses_punish_start"), null, null, 100, null, null);
            // campaignGameStarter.AddPlayerLine("conversation_prisoner_chat_player", "prisoner_recruit_start_player", "female_prisoner_choice", GameTexts.FindText("sue_more_spouses_female_prisoner_become_spouse_ask", null).ToString(), new ConversationSentence.OnConditionDelegate(isSuitableFemale), new ConversationSentence.OnConsequenceDelegate(ConversationResult), 100, null, null);
            //campaignGameStarter.AddPlayerLine("conversation_prisoner_chat_player", "hero_main_options", "female_prisoner_choice", GameTexts.FindText("sue_more_spouses_female_prisoner_become_spouse_ask", null).ToString(), new ConversationSentence.OnConditionDelegate(isSuitableFemale), new ConversationSentence.OnConsequenceDelegate(ConversationResult), 1000, null, null);


            //starter.AddDialogLine("female_prisoner_choice_result", "female_prisoner_choice", "female_prisoner_choice_refuse", GameTexts.FindText("sue_more_spouses_female_prisoner_become_spouse_result_refuse", null).ToString(), new ConversationSentence.OnConditionDelegate(() => this.acceptFlag != 1), null, 100, null);
        }


        public void changeSpousePrisonerStatusToActive()
        {
            Hero target = Hero.OneToOneConversationHero;
            if (null != target)
            {
                target.ChangeState(Hero.CharacterStates.Active);
            }
        }

        private void ChangePrisonerLordToSpouse()
        {
            Hero target = Hero.OneToOneConversationHero;
            if(null != target)
            {
                HeroRlationOperation.ChangePrisonerLordToSpouse(target);
            }
          

        }

        private void ChangePrisonerLordToFamily()
        {
            Hero target = Hero.OneToOneConversationHero;
            if (null != target)
            {
                HeroRlationOperation.ChangePrisonerLordToFamily(target);
            }
          
        }

        private String LoactionText(String idStr)
        {
            return GameTexts.FindText(idStr, null).ToString();
        }

        public bool isSpouseAndPrisoner()
        {
            Hero target = Hero.OneToOneConversationHero;
            return null != target && target.IsPrisoner && (target == Hero.MainHero.Spouse || Hero.MainHero.ExSpouses.Contains(target));
        }


        public bool CanBecomeSpouse()
        {
            Hero target = Hero.OneToOneConversationHero;
            return null != target && target.Spouse == null && !Hero.MainHero.ExSpouses.Contains(target) ;
        }

        public bool IsPrisioner()
        {
            Hero target = Hero.OneToOneConversationHero;
            return null!= target && MobileParty.MainParty.PrisonRoster.Contains(target.CharacterObject);
          
        }

        public bool IsNormalNPCLord()
        {
            Hero target = Hero.OneToOneConversationHero;
            return null != target && target.CharacterObject.Occupation != Occupation.Lord;
        }

        public bool IsLord()
        {
            Hero target = Hero.OneToOneConversationHero;
            return null != target && target.CharacterObject.Occupation == Occupation.Lord;
        }

        public bool IsNotLord()
        {
            Hero target = Hero.OneToOneConversationHero;
            return null != target ;
        }

        public bool IsPlayerCompanion()
        {
            Hero target = Hero.OneToOneConversationHero;
            return null != target && target.IsPlayerCompanion;
        }

        public bool IsNormalNPC()
        {
            //TaleWorlds.CampaignSystem.Occupation.TavernWench
            //FieldInfo fieldInfo = currStarter.GetType().GetField("_conversationManager", BindingFlags.NonPublic | BindingFlags.Instance);
           // ConversationManager  conversationManager = (ConversationManager)fieldInfo.GetValue(currStarter);
            bool flag = false;
            CharacterObject target = CharacterObject.OneToOneConversationCharacter;
            if (null != target)
            {
                flag = true;
            }
            return !IsPlayerCompanionAndCanBecomeSpouse() && !isSpouseAndPrisoner() && flag;
        }

        public bool IsPlayerCompanionAndCanBecomeSpouse()
        {
            return CanBecomeSpouse() && IsPlayerCompanion();
        }

        public ConversationSentence.OnConsequenceDelegate Result(ResultDelegate action)
        {
            return new ConversationSentence.OnConsequenceDelegate(action);
        }

        public  ConversationSentence.OnConditionDelegate Condition(ConditionDelegate action)
        {
            return new ConversationSentence.OnConditionDelegate(action);
        }

        public delegate bool ConditionDelegate();
        public delegate void ResultDelegate();

    }
}
