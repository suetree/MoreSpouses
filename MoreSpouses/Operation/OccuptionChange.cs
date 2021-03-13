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

        public static void ChangeToWanderer(CharacterObject target)
        {
            if (target.Occupation == Occupation.Wanderer) return;

            FieldInfo fieldInfo = target.GetType().GetField("_originCharacter", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            PropertyInfo propertyInfo = typeof(CharacterObject).GetProperty("Occupation");
            if (null != propertyInfo && null != propertyInfo.DeclaringType)
            {
                propertyInfo = propertyInfo.DeclaringType.GetProperty("Occupation");
                if (null != propertyInfo)
                {
                    propertyInfo.SetValue(target, Occupation.Wanderer, null);
                }
            }
            List<CharacterObject> list = CharacterObject.Templates.Where(obj => obj.Occupation == Occupation.Wanderer).ToList();
            CharacterObject wanderer = list.OrderBy(_ => Guid.NewGuid()).First();
            if (null != fieldInfo)
            {
                fieldInfo.SetValue(target, wanderer);
            }
            else
            {
                FieldInfo fieldInfoId = target.GetType().GetField("_originCharacterStringId", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (null != fieldInfoId)
                {
                    
                    fieldInfoId.SetValue(target, wanderer.StringId);
                }
            }

            target.HeroObject.IsNoble = false;
        }

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
