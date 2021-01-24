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
using TaleWorlds.GauntletUI;
using static TaleWorlds.Core.ItemObject;

namespace SueMoreSpouses.utils
{
    class HeroInitPropertyUtils
    {

        public static  void ChangeBodyProperties(Hero hero, BodyProperties bodyProperties)
        {
            FieldInfo fieldInfo = hero.GetType().GetField("StaticBodyProperties", BindingFlags.NonPublic | BindingFlags.Instance);
          
            if (null != fieldInfo && null != bodyProperties)
            {
                fieldInfo.SetValue(hero, bodyProperties);
            }
           
        }

        public static void InitTrait(Hero hero)
        {
            Random randomTraits = new Random();
            hero.SetTraitLevel(TraitObject.Find("Valor"), randomTraits.Next(0, 5));
            hero.SetTraitLevel(TraitObject.Find("Manager"), randomTraits.Next(0, 5));
            hero.SetTraitLevel(TraitObject.Find("Calculating"), randomTraits.Next(0, 5));
            hero.SetTraitLevel(TraitObject.Find("Politician"), randomTraits.Next(0, 5));
            hero.SetTraitLevel(TraitObject.Find("Commander"), randomTraits.Next(0, 5));
            hero.SetTraitLevel(TraitObject.Find("HopliteFightingSkills"), randomTraits.Next(0, 5));
        }


        public static void InitAttributeAndFouse(Hero hero)
        {
            int defaultAttributesLevel = 4;
            int addAttributesLevel = 4;
            hero.HeroDeveloper.UnspentAttributePoints = 10;
            hero.HeroDeveloper.UnspentFocusPoints = 10;
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

                hero.HeroDeveloper.AddFocus(DefaultSkills.OneHanded, 5, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.TwoHanded, 5, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Crossbow, 5, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Bow, 5, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Steward, 5, false);



            }
            else if (hero.CharacterObject.Occupation == Occupation.Townsfolk)
            {
                hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Endurance, addAttributesLevel, false);
                hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Vigor, addAttributesLevel, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Riding, 5, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Polearm, 5, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Throwing, 5, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Medicine, 5, false);
                FillBattleEquipment(hero);
            }
            else if (hero.CharacterObject.Occupation == Occupation.Villager)
            {
                hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Control, addAttributesLevel, false);
                hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Endurance, addAttributesLevel, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Bow, 5, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Riding, 5, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Medicine, 5, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Engineering, 5, false);

            }
            else
            {
                hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Control, addAttributesLevel, false);
                hero.HeroDeveloper.AddAttribute(CharacterAttributesEnum.Endurance, addAttributesLevel, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Medicine, 5, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Charm, 5, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Roguery, 5, false);
                hero.HeroDeveloper.AddFocus(DefaultSkills.Athletics, 5, false);
            }
        }


        public static void InitHeroSkill(Hero hero)
        {
            int minSkillLevel = 5;
            int maxSikllLevel = 100;
            Random random = new Random();
            if (MBRandom.RandomFloat < 0.2f)
            {
                minSkillLevel += 30;
                maxSikllLevel += 30;
                InformationManager.DisplayMessage(new InformationMessage(hero.Name + " gain more power"));
            }

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


        public static void InitHeroForNPC(Hero hero)
        {
            InitTrait(hero);
            InitAttributeAndFouse(hero);
            FillBattleEquipment(hero);
            InitHeroSkill(hero);

        }

        public static  void FillBattleEquipment(Hero hero)//sit =0 步兵， 1=骑兵，2=远程, N其他
        {

            int site = 4;
            switch (hero.CharacterObject.Occupation)
            {
                default: site = 3;  break;
                case Occupation.TavernWench: site = 0; break;
                case Occupation.Townsfolk: site = 1; break;
                case Occupation.Villager: site = 2; break;
            }
        

            int tier = MoreSpouseSetting.Instance.SettingData.NPCCharaObjectFromTier;
            if(tier > CharacterObject.MaxCharacterTier)
            {
                tier = CharacterObject.MaxCharacterTier;
            }
           
                Equipment equipment = hero.BattleEquipment;
                //筛选公共装备
                List<ItemObject> source = ItemObject.All.ToList().FindAll((obj) =>(IsBattleEquipmentItem(obj, hero.Culture, tier) ));
        
                //头盔
                //ItemObject toukui = source.FindAll((obj) =>(obj.Type == ItemTypeEnum.HeadArmor && !obj.IsCivilian)).GetRandomElement();
                ItemObject toukui = GetItemObject(ItemTypeEnum.HeadArmor, hero.Culture, tier).GetRandomElement();
                if (null != toukui) equipment[EquipmentIndex.Head] = new EquipmentElement(toukui);
                //肩甲
                // ItemObject shoulder = GetItemObject(ItemTypeEnum.Cape, hero.Culture, tier).GetRandomElement();
                ItemObject shoulder = GetItemObject(ItemTypeEnum.Cape, hero.Culture, tier).GetRandomElement();
                if (null != shoulder) equipment[EquipmentIndex.Cape] = new EquipmentElement(shoulder);
                //身甲
                //ItemObject body = source.FindAll((obj) => (obj.Type == ItemTypeEnum.BodyArmor && !obj.IsCivilian)).GetRandomElement();
                ItemObject body = GetItemObject(ItemTypeEnum.BodyArmor, hero.Culture, tier).GetRandomElement();
                if (null != body) equipment[EquipmentIndex.Body] = new EquipmentElement(body);
               
                //手甲
                //ItemObject hand = source.FindAll((obj) => (obj.Type == ItemTypeEnum.HandArmor && !obj.IsCivilian)).GetRandomElement();
                ItemObject hand = GetItemObject(ItemTypeEnum.HandArmor, hero.Culture, tier).GetRandomElement();
                if (null != hand) equipment[EquipmentIndex.Gloves] = new EquipmentElement(hand);
               
                //腿甲
                //ItemObject leg = source.FindAll((obj) => (obj.Type == ItemTypeEnum.LegArmor && !obj.IsCivilian)).GetRandomElement();
                ItemObject leg = GetItemObject(ItemTypeEnum.LegArmor, hero.Culture, tier).GetRandomElement();
                if (null != leg) equipment[EquipmentIndex.Leg] = new EquipmentElement(leg);
           
                ItemObject horse = GetItemObject(ItemTypeEnum.Horse, hero.Culture, tier).GetRandomElement();
                ItemObject horseHarness = GetItemObject(ItemTypeEnum.HorseHarness, hero.Culture, tier).GetRandomElement();

                ItemObject oneHanded = GetItemObject(ItemTypeEnum.OneHandedWeapon, hero.Culture, tier).GetRandomElement();
                ItemObject twoHanded = GetItemObject(ItemTypeEnum.TwoHandedWeapon, hero.Culture, tier).GetRandomElement();
                ItemObject shield = GetItemObject(ItemTypeEnum.Shield, hero.Culture, tier).GetRandomElement();
                ItemObject thrown = GetItemObject(ItemTypeEnum.Thrown, hero.Culture, tier).GetRandomElement();
            
                ItemObject polearm = GetItemObject(ItemTypeEnum.Polearm, hero.Culture, tier).GetRandomElement();
                ItemObject bow = GetItemObject(ItemTypeEnum.Bow, hero.Culture, tier).GetRandomElement();
                ItemObject arrwos = GetItemObject(ItemTypeEnum.Arrows, hero.Culture, tier).GetRandomElement();

                if (site == 0)
                { 
                    //剑盾，双手， 标枪1袋
                    if (null != oneHanded) equipment[EquipmentIndex.Weapon0] = new EquipmentElement(oneHanded);
                    if (null != shield) equipment[EquipmentIndex.Weapon1] = new EquipmentElement(shield);
                    if (null != twoHanded) equipment[EquipmentIndex.Weapon2] = new EquipmentElement(twoHanded);
                    if (null != thrown) equipment[EquipmentIndex.Weapon3] = new EquipmentElement(thrown);
                }
                else if (site == 1)
                {    //骑枪， 马 马甲，标枪 剑盾
                    if (null != oneHanded) equipment[EquipmentIndex.Weapon0] = new EquipmentElement(oneHanded);
                    if (null != shield) equipment[EquipmentIndex.Weapon1] = new EquipmentElement(shield);
                    if (null != polearm) equipment[EquipmentIndex.Weapon2] = new EquipmentElement(polearm);
                    if (null != thrown) equipment[EquipmentIndex.Weapon3] = new EquipmentElement(thrown);

                    if (null != horse) equipment[EquipmentIndex.Horse] = new EquipmentElement(horse);
                    if (null != horseHarness) equipment[EquipmentIndex.HorseHarness] = new EquipmentElement(horseHarness);
                }
                else if (site == 2)
                {
                    //双手，弓箭 箭
                    if (null != twoHanded) equipment[EquipmentIndex.Weapon0] = new EquipmentElement(twoHanded);
                    if (null != bow) equipment[EquipmentIndex.Weapon1] = new EquipmentElement(bow);
                    if (null != arrwos) equipment[EquipmentIndex.Weapon2] = new EquipmentElement(arrwos);
                    if (null != arrwos) equipment[EquipmentIndex.Weapon3] = new EquipmentElement(arrwos);
                }else
                {
                    if (null != oneHanded) equipment[EquipmentIndex.Weapon0] = new EquipmentElement(oneHanded);
                    if (null != shield) equipment[EquipmentIndex.Weapon1] = new EquipmentElement(shield);
                    if (null != bow) equipment[EquipmentIndex.Weapon2] = new EquipmentElement(bow);
                    if (null != arrwos) equipment[EquipmentIndex.Weapon3] = new EquipmentElement(arrwos);
                }
            
        }


        private static List<ItemObject> GetItemObject(ItemTypeEnum typeEnum, BasicCultureObject culture, int tier)
        {
            List<ItemObject> weaponsources = ItemObject.All.ToList().FindAll((obj) => (obj.Culture == culture
            && typeEnum == obj.ItemType && GetTierByObjectItemItemTiers(obj.Tier) == tier));
            if(weaponsources.Count == 0)
            {
                weaponsources = ItemObject.All.ToList().FindAll((obj) => (obj.Culture == culture && typeEnum == obj.ItemType));
            }

            if (weaponsources.Count == 0)
            {
                weaponsources = ItemObject.All.ToList().FindAll((obj) => ( typeEnum == obj.ItemType && GetTierByObjectItemItemTiers(obj.Tier) == tier));
            }

            if (weaponsources.Count == 0)
            {
                weaponsources = ItemObject.All.ToList().FindAll((obj) => (typeEnum == obj.ItemType ));
            }
            return weaponsources;
        }

        private static bool IsBattleEquipmentItem(ItemObject item, BasicCultureObject culture, int tier)
        {
            return item.Culture == culture && GetTierByObjectItemItemTiers(item.Tier) == tier;
        }

        private static int  GetTierByObjectItemItemTiers(ItemTiers tier)
        {
            switch (tier)
            {
                default: return 6;
                case ItemTiers.Tier1: return 1;
                case ItemTiers.Tier2: return 2;
                case ItemTiers.Tier3: return 3;
                case ItemTiers.Tier4: return 4;
                case ItemTiers.Tier5: return 5;
                case ItemTiers.Tier6: return 6;
            }
        }
    }
}
