using System;
using System.Collections.Generic;
using TaleWorlds.GauntletUI;

namespace SueMoreSpouses.widget
{
    class SpouseCharacterTableauWidget : TextureWidget
    {

		private string _bannerCode;

		private string _bodyProperties;

		private string _charStringId;

		private int _stanceIndex;

		private uint _armorColor1;

		private uint _armorColor2;

		private bool _isFemale;

		private bool _isEquipmentAnimActive;

		private string _equipmentCode;

		private string _mountCreationKey;

		[Editor(false)]
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
					base.OnPropertyChanged(value, "BannerCodeText");
					base.SetTextureProviderProperty("BannerCodeText", value);
				}
			}
		}

		[Editor(false)]
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
					base.OnPropertyChanged(value, "BodyProperties");
					base.SetTextureProviderProperty("BodyProperties", value);
				}
			}
		}

		[Editor(false)]
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
					base.OnPropertyChanged(value, "CharStringId");
					base.SetTextureProviderProperty("CharStringId", value);
				}
			}
		}

		[Editor(false)]
		public int StanceIndex
		{
			get
			{
				return this._stanceIndex;
			}
			set
			{
				if (value != this._stanceIndex)
				{
					this._stanceIndex = value;
					base.OnPropertyChanged(value, "StanceIndex");
					base.SetTextureProviderProperty("StanceIndex", value);
				}
			}
		}

		[Editor(false)]
		public bool IsEquipmentAnimActive
		{
			get
			{
				return this._isEquipmentAnimActive;
			}
			set
			{
				if (value != this._isEquipmentAnimActive)
				{
					this._isEquipmentAnimActive = value;
					base.OnPropertyChanged(value, "IsEquipmentAnimActive");
					base.SetTextureProviderProperty("IsEquipmentAnimActive", value);
				}
			}
		}

		[Editor(false)]
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
					base.OnPropertyChanged(value, "IsFemale");
					base.SetTextureProviderProperty("IsFemale", value);
				}
			}
		}

		[Editor(false)]
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
					base.OnPropertyChanged(value, "EquipmentCode");
					base.SetTextureProviderProperty("EquipmentCode", value);
				}
			}
		}

		[Editor(false)]
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
					base.OnPropertyChanged(value, "MountCreationKey");
					base.SetTextureProviderProperty("MountCreationKey", value);
				}
			}
		}

		[Editor(false)]
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
					base.OnPropertyChanged(value, "ArmorColor1");
					base.SetTextureProviderProperty("ArmorColor1", value);
				}
			}
		}

		[Editor(false)]
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
					base.OnPropertyChanged(value, "ArmorColor2");
					base.SetTextureProviderProperty("ArmorColor2", value);
				}
			}
		}

		public SpouseCharacterTableauWidget(UIContext context) : base(context)
		{
			base.TextureProviderName = "SpouseCharacterTableauTextureProvider";
		}

		protected override void OnMousePressed()
		{
			base.SetTextureProviderProperty("CurrentlyRotating", true);
		}

		protected override void OnMouseReleased()
		{
			
			base.SetTextureProviderProperty("CurrentlyRotating", false);
		}
	}
}
