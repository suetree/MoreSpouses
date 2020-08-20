using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace SueMoreSpouses.view
{
    class SpouseViewModel : CharacterViewModel
    {
		private bool _isDead;


		private Hero _hero;
		private bool isNonEquipment;

		[DataSourceProperty]
		public bool IsDead
		{
			get
			{
				return this._isDead;
			}
			set
			{
				if (value != this._isDead)
				{
					this._isDead = value;
					base.OnPropertyChanged("IsDead");
				}
			}
		}

		public SpouseViewModel(CharacterViewModel.StanceTypes stance = CharacterViewModel.StanceTypes.None) : base(stance)
		{
		}

		public void ActionStand()
		{
			//this.StanceIndex = CharacterViewModel.StanceTypes.None;
		}

		public void ActionMounted()
		{

		}

		public void FillFrom(Hero hero, int seed = -1)
		{
			if (FaceGen.GetMaturityTypeWithAge(hero.Age) > BodyMeshMaturityType.Child)
			{
				this._hero = hero;
				this.isNonEquipment = false;
				base.FillFrom(hero.CharacterObject, seed);
				base.MountCreationKey = TaleWorlds.Core.MountCreationKey.GetRandomMountKey(hero.CharacterObject.Equipment[10].Item, hero.CharacterObject.GetMountKeySeed());
				this.IsDead = hero.IsDead;
				if (hero.Noncombatant)
				{
					Equipment expr_69 = hero.CivilianEquipment;
					this._equipment = ((expr_69 != null) ? expr_69.Clone(false) : null);
				}
				else
				{
					Equipment expr_84 = hero.BattleEquipment;
					this._equipment = ((expr_84 != null) ? expr_84.Clone(false) : null);
				}
				Equipment expr_9D = this._equipment;
			    base.EquipmentCode = ((expr_9D != null) ? expr_9D.CalculateEquipmentCode() : null);
				if (((hero != null) ? hero.ClanBanner : null) != null)
				{
					base.BannerCodeText = BannerCode.CreateFrom(hero.ClanBanner).Code;
				}
				IFaction expr_D9 = hero.MapFaction;
				base.ArmorColor1 = ((expr_D9 != null) ? expr_D9.Color : 0u);
				IFaction expr_F1 = hero.MapFaction;
				base.ArmorColor2 = ((expr_F1 != null) ? expr_F1.Color2 : 0u);
			}
		}

		public void NonEquipment()
		{
			if (null != this._hero)
			{
				BasicCharacterObject character = this._hero.CharacterObject;
				if (this.isNonEquipment)
				{
					this.isNonEquipment = false;
					this.BodyProperties = character.GetBodyProperties(character.Equipment, -1).ToString();
					;
				}
				else
				{
					this.isNonEquipment = true;
					Equipment equipment = new Equipment();
					this.BodyProperties = character.GetBodyProperties(equipment, -1).ToString();
				}
			}
		
		
			
		}


	}
}
