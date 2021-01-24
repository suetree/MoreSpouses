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
    class SpouseCharacterVM : ViewModel
	{
		public enum StanceTypes
		{
			None,
			EmphasizeFace,
			SideView,
			CelebrateVictory,
			OnMount
		}
		private bool _isDead;
		CharacterObject _character;
		private int _selectedIndex = 0;

		private string _mountCreationKey = "";

		private string _bodyProperties = "";

		private bool _isFemale;

		private int _stanceIndex;

		private uint _armorColor1;

		private uint _armorColor2;

		private string _equipmentCode;

		protected Equipment _equipment;

		private string _charStringId;

		protected string _bannerCode;

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

		[DataSourceProperty]
		public bool IsBattledSelected
		{
			get
			{
				return this._selectedIndex == 1;
			}
		}

		[DataSourceProperty]
		public bool IsCivilizedSelected
		{
			get
			{
				return this._selectedIndex == 0;
			}
		}

		[DataSourceProperty]
		public bool IsUnderwearSelected
		{
			get
			{
				return this._selectedIndex == 2;
			}
		}

		[DataSourceProperty]
		public string BannerCodeText
		{
			get
			{
				return this._bannerCode;
			}
			set
			{
				if (value != this._bannerCode)
				{
					this._bannerCode = value;
					base.OnPropertyChangedWithValue(value, "BannerCodeText");
				}
			}
		}

		[DataSourceProperty]
		public string BodyProperties
		{
			get
			{
				return this._bodyProperties;
			}
			set
			{
				if (value != this._bodyProperties)
				{
					this._bodyProperties = value;
					base.OnPropertyChangedWithValue(value, "BodyProperties");
				}
			}
		}

		[DataSourceProperty]
		public string MountCreationKey
		{
			get
			{
				return this._mountCreationKey;
			}
			set
			{
				if (value != this._mountCreationKey)
				{
					this._mountCreationKey = value;
					base.OnPropertyChangedWithValue(value, "MountCreationKey");
				}
			}
		}

		[DataSourceProperty]
		public string CharStringId
		{
			get
			{
				return this._charStringId;
			}
			set
			{
				if (value != this._charStringId)
				{
					this._charStringId = value;
					base.OnPropertyChangedWithValue(value, "CharStringId");
				}
			}
		}

		[DataSourceProperty]
		public int StanceIndex
		{
			get
			{
				return this._stanceIndex;
			}
			private set
			{
				if (value != this._stanceIndex)
				{
					this._stanceIndex = value;
					base.OnPropertyChangedWithValue(value, "StanceIndex");
				}
			}
		}

		[DataSourceProperty]
		public bool IsFemale
		{
			get
			{
				return this._isFemale;
			}
			set
			{
				if (value != this._isFemale)
				{
					this._isFemale = value;
					base.OnPropertyChangedWithValue(value, "IsFemale");
				}
			}
		}

		[DataSourceProperty]
		public string EquipmentCode
		{
			get
			{
				return this._equipmentCode;
			}
			set
			{
				if (value != this._equipmentCode)
				{
					this._equipmentCode = value;
					base.OnPropertyChangedWithValue(value, "EquipmentCode");
				}
			}
		}

		[DataSourceProperty]
		public uint ArmorColor1
		{
			get
			{
				return this._armorColor1;
			}
			set
			{
				if (value != this._armorColor1)
				{
					this._armorColor1 = value;
					base.OnPropertyChangedWithValue(value, "ArmorColor1");
				}
			}
		}

		[DataSourceProperty]
		public uint ArmorColor2
		{
			get
			{
				return this._armorColor2;
			}
			set
			{
				if (value != this._armorColor2)
				{
					this._armorColor2 = value;
					base.OnPropertyChangedWithValue(value, "ArmorColor2");
				}
			}
		}

		public SpouseCharacterVM()
		{
		}

		public SpouseCharacterVM(SpouseCharacterVM.StanceTypes stance = SpouseCharacterVM.StanceTypes.None)
		{
			this._equipment = new Equipment(false);
			this.EquipmentCode = this._equipment.CalculateEquipmentCode();
			this.StanceIndex = (int)stance;
		}

		public void SetEquipment(EquipmentIndex index, EquipmentElement item)
		{
			this._equipment[(int)index] = item;
			this.EquipmentCode = this._equipment.CalculateEquipmentCode();
		}

		public void SetSelectedTab(int index)
		{
			this._selectedIndex = index;
			base.OnPropertyChanged("IsBattledSelected");
			base.OnPropertyChanged("IsCivilizedSelected");
			base.OnPropertyChanged("IsUnderwearSelected");
			ChangeClothesBySelectedIndex();
		}

		public void ChangeClothesBySelectedIndex()
		{

			switch (this._selectedIndex)
			{
				default:
					FillEquipment(this._character.FirstCivilianEquipment);
					break;
				case 1:
					FillEquipment(this._character.FirstBattleEquipment);
					break;
				case 2:
					Equipment equipment = new Equipment();
					equipment[10] = this._character.Equipment[10];
					FillEquipment(equipment);
	
					break;
			}
		}

		public void FillFrom(CharacterObject character, int seed = -1)
		{
			if (FaceGen.GetMaturityTypeWithAge(character.Age) > BodyMeshMaturityType.Child)
			{

				this._character = character;

				if (character.Culture != null)
				{
					this.ArmorColor1 = character.Culture.Color;
					this.ArmorColor2 = character.Culture.Color2;
				}
				this.CharStringId = character.StringId;
				this.IsFemale = character.IsFemale;
				
				ChangeClothesBySelectedIndex();
			}
		}

	

		private void FillEquipment(Equipment equipment)
		{
			this.BodyProperties = this._character.GetBodyProperties(equipment, -1).ToString();
			this.MountCreationKey = TaleWorlds.Core.MountCreationKey.GetRandomMountKey((equipment != null) ? equipment[10].Item : null, Common.GetDJB2(this._character.StringId));
			this._equipment = ((equipment != null) ? equipment.Clone(false) : null);
			Equipment expr_C6 = equipment;
			this.EquipmentCode = ((expr_C6 != null) ? expr_C6.CalculateEquipmentCode() : null);
		}
	}
}
