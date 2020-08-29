using HarmonyLib;
using Helpers;
using SueMoreSpouses.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;

namespace SueMoreSpouses.utils
{
    class HeroUtils
    {

        public static  void ChangeBodyProperties(Hero hero, BodyProperties bodyProperties)
        {
            FieldInfo fieldInfo = hero.GetType().GetField("StaticBodyProperties", BindingFlags.NonPublic | BindingFlags.Instance);
          
            if (null != fieldInfo && null != bodyProperties)
            {
                fieldInfo.SetValue(hero, bodyProperties);
            }
           
        }


        public static void InitHeroTraits(Hero hero)
        {
            int defaultlevel = 4;

            hero.SetTraitLevel(TraitObject.Find("Valor"), defaultlevel);
            hero.SetTraitLevel(TraitObject.Find("Manager"), defaultlevel);
            hero.SetTraitLevel(TraitObject.Find("Calculating"), defaultlevel);
            hero.SetTraitLevel(TraitObject.Find("Politician"), defaultlevel);
            hero.SetTraitLevel(TraitObject.Find("Commander"), defaultlevel);
            hero.SetTraitLevel(TraitObject.Find("HopliteFightingSkills"), defaultlevel);


            int defaultAttributesLevel = 4;
            int addAttributesLevel = 4;
            hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Vigor, defaultAttributesLevel, false);
            hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Control, defaultAttributesLevel, false);
            hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Endurance, defaultAttributesLevel, false);
            hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Cunning, defaultAttributesLevel, false);
            hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Social, defaultAttributesLevel, false);
            hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Intelligence, defaultAttributesLevel, false);
            if (hero.CharacterObject.Occupation == Occupation.TavernWench)
            {
                hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Vigor, addAttributesLevel, false);
                hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Control, addAttributesLevel, false);

                hero.HeroDeveloper.AddFocus(DefaultSkills.OneHanded, 6, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.TwoHanded, 6, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Crossbow, 6, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Bow, 6, false);

                FillBattleEquipment(hero, 0);

            }
            else if (hero.CharacterObject.Occupation == Occupation.Townsfolk)
            {
                hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Endurance, addAttributesLevel, false);
                hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Vigor, addAttributesLevel, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Riding, 6, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Polearm, 6, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Throwing, 6, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Medicine, 6, false);
                FillBattleEquipment(hero, 1);
            }
            else if (hero.CharacterObject.Occupation == Occupation.Villager)
            {
                hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Control, addAttributesLevel, false);
                hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Endurance, addAttributesLevel, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Bow, 6, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Riding, 6, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Medicine, 6, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Engineering, 6, false);
                FillBattleEquipment(hero, 2);
            }
            hero.HeroDeveloper.UnspentAttributePoints =10;
            hero.HeroDeveloper.UnspentFocusPoints = 10;
            int minSkillLevel = 5;
            int maxSikllLevel = 100;
            Random random = new Random();
          
            foreach (SkillObject sk in DefaultSkills.GetAllSkills())
            {
                for (int i = 0; i < random.Next(5); i++)
                {
                    random.Next(minSkillLevel, maxSikllLevel);
                }
                int skillLevel = random.Next(minSkillLevel, maxSikllLevel);
                hero.HeroDeveloper.ChangeSkillLevel(sk, skillLevel, false);
            }


           if (MoreSpouseSetting.Instance.SettingData.NPCCharaObjectSkillAuto)
            {
                foreach (SkillObject sk3 in DefaultSkills.GetAllSkills())
                {
                    hero.HeroDeveloper.TakeAllPerks(sk3);
                }
            }

        }

        private static  void FillBattleEquipment(Hero hero, int site)//sit =0 步兵， 1=骑兵，2=远程
        {
            int tier = MoreSpouseSetting.Instance.SettingData.NPCCharaObjectFromTier;
            if(tier > CharacterObject.MaxCharacterTier)
            {
                tier = CharacterObject.MaxCharacterTier;
            }
            CharacterObject characterObject = null; 
            if (site == 0)
            {
                characterObject =  CharacterObject.FindAll((obj) => (obj.Culture == hero.Culture && obj.IsSoldier && obj.IsInfantry && obj.Tier == tier)).GetRandomElement();
            }else if (site == 1)
            {
                characterObject = CharacterObject.FindAll((obj) => (obj.Culture == hero.Culture && obj.IsSoldier && obj.IsMounted && obj.Tier == tier)).GetRandomElement();
            }
            else if (site == 2)
            {
                characterObject = CharacterObject.FindAll((obj) => (obj.Culture == hero.Culture && obj.IsSoldier && obj.IsArcher && obj.Tier == tier)).GetRandomElement();
            }

            if (null == characterObject)
            {
                characterObject = CharacterObject.FindAll((obj) => (obj.Culture == hero.Culture && obj.IsSoldier && obj.Tier == tier)).GetRandomElement();
            }



            if (null == characterObject)
            {
                characterObject = CharacterObject.FindAll((obj) => (obj.Culture == hero.Culture)).GetRandomElement();
            }



            if (characterObject != null)
            {
                Equipment randomElement = characterObject.BattleEquipments.GetRandomElement<Equipment>();
                EquipmentHelper.AssignHeroEquipmentFromEquipment(hero, randomElement);
            }
        }
    }
}
