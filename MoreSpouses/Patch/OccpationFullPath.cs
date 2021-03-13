using HarmonyLib;
using SueMoreSpouses.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.EncyclopediaItems;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;

namespace SueMoreSpouses.Patch
{
   // [HarmonyPatch(typeof(EncyclopediaHeroPageVM), "Refresh")]
    class OccpationFullPath
    {
        public static void  Postfix(ref EncyclopediaHeroPageVM __instance)
        {
            if (null != ReflectUtils.ReflectField("_hero", __instance))
            {
                Hero hero = (Hero)ReflectUtils.ReflectField("_hero", __instance);
                string heroOccupationName = CampaignUIHelper.GetHeroOccupationName(hero);
                if (string.IsNullOrEmpty(heroOccupationName))
                {
                    heroOccupationName = System.Enum.GetName(typeof(Occupation), hero.CharacterObject.Occupation);
                    string definition3 = GameTexts.FindText("str_enc_sf_occupation", null).ToString();
                    __instance.Stats.Add(new StringPairItemVM(definition3, heroOccupationName, null));
                }

                //模板
                FieldInfo fieldInfo = hero.CharacterObject.GetType().GetField("_originCharacter", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (null != fieldInfo)
                {
                    object obj = fieldInfo.GetValue(hero.CharacterObject); ;
                    if (obj is CharacterObject)
                    {
                        CharacterObject originCharacter = (CharacterObject)obj;
                        __instance.Stats.Add(new StringPairItemVM("模板: ", originCharacter.StringId, null));
                    }
                   
                }
                
            }
          
        }

      
    }
}
