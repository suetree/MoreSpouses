using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Encyclopedia;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;

namespace SueMoreSpouses.view.troop
{
    class SpousesSelectTroopsItemVM : ViewModel
    {

		private readonly Action<SpousesSelectTroopsItemVM> _onAdd;

		private readonly Action<SpousesSelectTroopsItemVM> _onRemove;

		private int _currentAmount;

		private int _maxAmount;

		private ImageIdentifierVM _visual;

		private bool _isSelected;

		private bool _isRosterFull;

		private bool _isLocked;

		private string _name;

		private string _amountText;

		private StringItemWithHintVM _tierIconData;

		private StringItemWithHintVM _typeIconData;

		public TroopRosterElement Troop
		{
			get;
			private set;
		}

		[DataSourceProperty]
		public int MaxAmount
		{
			get
			{
				return this._maxAmount;
			}
			set
			{
				if (value != this._maxAmount)
				{
					this._maxAmount = value;
					base.OnPropertyChangedWithValue(value, "MaxAmount");
					this.UpdateAmountText();
				}
			}
		}

		[DataSourceProperty]
		public bool IsSelected
		{
			get
			{
				return this._isSelected;
			}
			set
			{
				if (value != this._isSelected)
				{
					this._isSelected = value;
					base.OnPropertyChangedWithValue(value, "IsSelected");
				}
			}
		}

		[DataSourceProperty]
		public bool IsRosterFull
		{
			get
			{
				return this._isRosterFull;
			}
			set
			{
				if (value != this._isRosterFull)
				{
					this._isRosterFull = value;
					base.OnPropertyChangedWithValue(value, "IsRosterFull");
				}
			}
		}

		[DataSourceProperty]
		public bool IsLocked
		{
			get
			{
				return this._isLocked;
			}
			set
			{
				if (value != this._isLocked)
				{
					this._isLocked = value;
					base.OnPropertyChangedWithValue(value, "IsLocked");
				}
			}
		}

		[DataSourceProperty]
		public int CurrentAmount
		{
			get
			{
				return this._currentAmount;
			}
			set
			{
				if (value != this._currentAmount)
				{
					this._currentAmount = value;
					base.OnPropertyChangedWithValue(value, "CurrentAmount");
					this.IsSelected = (value > 0);
					this.UpdateAmountText();
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChangedWithValue(value, "Name");
				}
			}
		}

		[DataSourceProperty]
		public string AmountText
		{
			get
			{
				return this._amountText;
			}
			set
			{
				if (value != this._amountText)
				{
					this._amountText = value;
					base.OnPropertyChangedWithValue(value, "AmountText");
				}
			}
		}

		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return this._visual;
			}
			set
			{
				if (value != this._visual)
				{
					this._visual = value;
					base.OnPropertyChangedWithValue(value, "Visual");
				}
			}
		}

		[DataSourceProperty]
		public StringItemWithHintVM TierIconData
		{
			get
			{
				return this._tierIconData;
			}
			set
			{
				if (value != this._tierIconData)
				{
					this._tierIconData = value;
					base.OnPropertyChangedWithValue(value, "TierIconData");
				}
			}
		}

		[DataSourceProperty]
		public StringItemWithHintVM TypeIconData
		{
			get
			{
				return this._typeIconData;
			}
			set
			{
				if (value != this._typeIconData)
				{
					this._typeIconData = value;
					base.OnPropertyChangedWithValue(value, "TypeIconData");
				}
			}
		}

		public SpousesSelectTroopsItemVM(TroopRosterElement troop, Action<SpousesSelectTroopsItemVM> onAdd, Action<SpousesSelectTroopsItemVM> onRemove)
		{
			this._onAdd = onAdd;
			this._onRemove = onRemove;
			this.Troop = troop;
			this.MaxAmount = this.Troop.Number - this.Troop.WoundedNumber;
			this.Visual = new ImageIdentifierVM(CampaignUIHelper.GetCharacterCode(troop.Character, false));
			this.Name = troop.Character.Name.ToString();
			this.TierIconData = CampaignUIHelper.GetCharacterTierData(this.Troop.Character, false);
			this.TypeIconData = CampaignUIHelper.GetCharacterTypeData(this.Troop.Character, false);
		}

		private void ExecuteAdd()
		{
			Action<SpousesSelectTroopsItemVM> expr_06 = this._onAdd;
			if (expr_06 == null)
			{
				return;
			}
			expr_06.DynamicInvokeWithLog(new object[]
			{
				this
			});
		}

		private void ExecuteRemove()
		{
			Action<SpousesSelectTroopsItemVM> expr_06 = this._onRemove;
			if (expr_06 == null)
			{
				return;
			}
			expr_06.DynamicInvokeWithLog(new object[]
			{
				this
			});
		}

		private void UpdateAmountText()
		{
			GameTexts.SetVariable("LEFT", this.CurrentAmount);
			GameTexts.SetVariable("RIGHT", this.MaxAmount);
			this.AmountText = GameTexts.FindText("str_LEFT_over_RIGHT", null).ToString();
		}

		private void ExecuteLink()
		{
			if (this.Troop.Character != null)
			{
				EncyclopediaManager arg_47_0 = Campaign.Current.EncyclopediaManager;
				Hero expr_27 = this.Troop.Character.HeroObject;
				arg_47_0.GoToLink(((expr_27 != null) ? expr_27.EncyclopediaLink : null) ?? this.Troop.Character.EncyclopediaLink);
			}
		}
	}
}
