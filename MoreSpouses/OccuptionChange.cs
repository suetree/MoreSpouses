using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace SueMoreSpouses
{
    class OccuptionChange
    {

        public void ChangeToLord(CharacterObject characterObject)
        {

            ChangeOccupation(characterObject, Hero.MainHero.CharacterObject);
        }

        /*public static void ChangeToWanderer(CharacterObject target)
        {
            CharacterObject origin = null;
            *//*    List<CharacterObjectOccuptionRecord> records = JsonConvert.DeserializeObject<List<CharacterObjectOccuptionRecord>>(recordStr);
                 CharacterObjectOccuptionRecord record = records.Find((obj) => ((null != obj && null != obj.StringId) && obj.StringId == characterObject.StringId));
                 CharacterObjectOccuptionTemplate occuptionTemplate = null ;
                 if (null != record && null!= record.OccuptionTemplates && record.OccuptionTemplates.Count() > 0)
                 {
                     occuptionTemplate = record.OccuptionTemplates.Find((obj) => obj.Occuption == "Wanderer");
                 }

                 if(null != occuptionTemplate && null != occuptionTemplate.Template)
                 {
                     origin = CharacterObject.Templates.ToList().Find((obj) => (obj.StringId == occuptionTemplate.Template));
                 }*//*
          
            if (null == origin)
            {
                origin = CharacterObject.Templates.ToList().Find((obj) => (obj.Occupation == Occupation.Wanderer && obj.Culture == target.Culture ));
            }
            if (null != origin)
            {
                ChangeOccupation(target, origin);
            }
            target.HeroObject.IsNoble = false;
        }*/

        public static void ChangeOccupationToLord(CharacterObject target)
        {
            if (target.Occupation == Occupation.Lord) return;

            FieldInfo fieldInfo = target.GetType().GetField("_originCharacter", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo propertyInfo = typeof(CharacterObject).GetProperty("Occupation");
            if (null != propertyInfo && null != propertyInfo.DeclaringType)
            {
                propertyInfo = propertyInfo.DeclaringType.GetProperty("Occupation");
                if (null != propertyInfo)
                {
                    propertyInfo.SetValue(target, Occupation.Lord, null);
                }
            }
            if (null != fieldInfo)
            {
                fieldInfo.SetValue(target, CharacterObject.PlayerCharacter);
            }
            else
            {
                FieldInfo fieldInfoId = target.GetType().GetField("_originCharacterStringId", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (null != fieldInfoId)
                {
                    fieldInfoId.SetValue(target, CharacterObject.PlayerCharacter.StringId);
                }
            }
        }

        private static void ChangeOccupation(CharacterObject target, CharacterObject origin )
        {
            if (target.Occupation == origin.Occupation) return;
            FieldInfo fieldInfo = target.GetType().GetField("_originCharacter", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            CharacterObject originalCharacterObject = (CharacterObject)fieldInfo.GetValue(target);
            PropertyInfo propertyInfo = typeof(CharacterObject).GetProperty("Occupation");
            if (null != propertyInfo && null != propertyInfo.DeclaringType)
            {
                propertyInfo = propertyInfo.DeclaringType.GetProperty("Occupation");
                if (null != propertyInfo)
                {
                    propertyInfo.SetValue(target, origin.Occupation, null);
                }
            }

            fieldInfo.SetValue(target, origin);
        }


        private String loadSaveData()
        {
            return "";
        }

        private void saveData(String json)
        {

        }
    }
}
