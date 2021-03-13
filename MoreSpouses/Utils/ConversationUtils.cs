using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SueMoreSpouses.Utils
{
    class ConversationUtils
    {

        public static ConversationManager GetConversationManager(CampaignGameStarter campaignGameStarter)
        {
            ConversationManager conversationManager = null; 
            FieldInfo fieldInfo = campaignGameStarter.GetType().GetField("_conversationManager", BindingFlags.NonPublic | BindingFlags.Instance);
            Object obj = fieldInfo.GetValue(campaignGameStarter);
            if (null != obj)
            {
                conversationManager = (ConversationManager)fieldInfo.GetValue(campaignGameStarter);
            }

            return conversationManager;
        }

        public static void ChangeCurrentCharaObject(CampaignGameStarter campaignGameStarter, Hero hero)
        {
            ConversationManager conversationManager = GetConversationManager( campaignGameStarter);
            if (null != conversationManager)
            {
                IAgent iagent =  conversationManager.ListenerAgent;
                if (iagent.GetType() == typeof(Agent))
                {
                    Agent agent = (Agent)iagent;
                    FieldInfo fieldInfo = agent.GetType().GetField("_character", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (null != fieldInfo)
                    {
                        fieldInfo.SetValue(agent, hero.CharacterObject);
                    }

                    HeroFaceUtils.UpdatePlayerCharacterBodyProperties(hero, agent.BodyPropertiesValue, hero.CharacterObject.IsFemale);
                  //  hero.CharacterObject.UpdatePlayerCharacterBodyProperties(agent.BodyPropertiesValue, hero.CharacterObject.IsFemale);

                    if (null != agent.Name)
                    {
                        hero.Name = new TextObject(String.Format("\"{0}\"", agent.Name) + hero.Name.ToString());
                        FieldInfo fieldInfo2 = agent.GetType().GetField("_name", BindingFlags.NonPublic | BindingFlags.Instance);
                        if (null != fieldInfo2)
                        {
                            fieldInfo2.SetValue(agent, hero.Name);
                        }
                    }
                  
                   
                }
               
            }
        }
    }
}
