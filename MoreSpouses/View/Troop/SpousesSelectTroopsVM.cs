using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
namespace SueMoreSpouses.view.troop
{
    class SpousesSelectTroopsVM : ViewModel
    {

		private readonly Action<TroopRoster> _onDone;

		private readonly TroopRoster _initialRoster;

		private readonly Func<CharacterObject, bool> _canChangeChangeStatusOfTroop;

		private int _maxSelectableTroopCount;

		private int _currentTotalSelectedTroopCount;

		public bool IsFiveStackModifierActive;

		public bool IsEntireStackModifierActive;

		private bool _isEnabled;

		private string _doneText;

		private string _cancelText;

		private string _titleText;

		private string _currentSelectedAmountText;

		private string _currentSelectedAmountTitle;

		private int _maxNum;

		private MBBindingList<SpousesSelectTroopsItemVM> _troops;

		[DataSourceProperty]
		public bool IsEnabled
		{
			get
			{
				return this._isEnabled;
			}
			set
			{
				if (value != this._isEnabled)
				{
					this._isEnabled = value;
					base.OnPropertyChangedWithValue(value, "IsEnabled");
				}
			}
		}

		[DataSourceProperty]
		public MBBindingList<SpousesSelectTroopsItemVM> Troops
		{
			get
			{
				return this._troops;
			}
			set
			{
				if (value != this._troops)
				{
					this._troops = value;
					base.OnPropertyChangedWithValue(value, "Troops");
				}
			}
		}

		[DataSourceProperty]
		public string DoneText
		{
			get
			{
				return this._doneText;
			}
			set
			{
				if (value != this._doneText)
				{
					this._doneText = value;
					base.OnPropertyChangedWithValue(value, "DoneText");
				}
			}
		}

		[DataSourceProperty]
		public string CancelText
		{
			get
			{
				return this._cancelText;
			}
			set
			{
				if (value != this._cancelText)
				{
					this._cancelText = value;
					base.OnPropertyChangedWithValue(value, "CancelText");
				}
			}
		}

		[DataSourceProperty]
		public string TitleText
		{
			get
			{
				return this._titleText;
			}
			set
			{
				if (value != this._titleText)
				{
					this._titleText = value;
					base.OnPropertyChangedWithValue(value, "TitleText");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentSelectedAmountText
		{
			get
			{
				return this._currentSelectedAmountText;
			}
			set
			{
				if (value != this._currentSelectedAmountText)
				{
					this._currentSelectedAmountText = value;
					base.OnPropertyChangedWithValue(value, "CurrentSelectedAmountText");
				}
			}
		}

		[DataSourceProperty]
		public string CurrentSelectedAmountTitle
		{
			get
			{
				return this._currentSelectedAmountTitle;
			}
			set
			{
				if (value != this._currentSelectedAmountTitle)
				{
					this._currentSelectedAmountTitle = value;
					base.OnPropertyChangedWithValue(value, "CurrentSelectedAmountTitle");
				}
			}
		}

		public SpousesSelectTroopsVM(TroopRoster initialRoster, int maxNum,  Func<CharacterObject, bool> canChangeChangeStatusOfTroop, Action<TroopRoster> onDone)
		{
			this._canChangeChangeStatusOfTroop = canChangeChangeStatusOfTroop;
			this._onDone = onDone;
			this._maxNum = maxNum;
			this._initialRoster = initialRoster;
			this.InitList();
			this.RefreshValues();
			this._maxSelectableTroopCount = maxNum;
			this.OnCurrentSelectedAmountChange();
		}

		public override void RefreshValues()
		{
			base.RefreshValues();
			//this.TitleText = new TextObject("{=uQgNPJnc}Manage Troops", null).ToString();
			this.TitleText = new TextObject("{=sms_sneak_select_troop_title}Choose your battle companion", null).ToString();
			this.DoneText = GameTexts.FindText("str_done", null).ToString();
			this.CancelText = GameTexts.FindText("str_cancel", null).ToString();
			//this.CurrentSelectedAmountTitle = new TextObject("{=HURdAXDQ}Hideout Crew", null).ToString();
			this.CurrentSelectedAmountTitle = new TextObject("{=sms_sneak_select_troop_name}Action Group", null).ToString();
		}

		private void InitList()
		{
			this.Troops = new MBBindingList<SpousesSelectTroopsItemVM>();
			this._currentTotalSelectedTroopCount = 0;
			List<TroopRosterElement> list = MobileParty.MainParty.MemberRoster.GetTroopRoster();
			foreach (TroopRosterElement current in list)
			{
				if (current.Number - current.WoundedNumber > 0)
				{
					SpousesSelectTroopsItemVM manageHideoutTroopItemVM = new SpousesSelectTroopsItemVM(current, new Action<SpousesSelectTroopsItemVM>(this.OnAddCount), new Action<SpousesSelectTroopsItemVM>(this.OnRemoveCount));
					manageHideoutTroopItemVM.IsLocked = !this._canChangeChangeStatusOfTroop(current.Character);
					this.Troops.Add(manageHideoutTroopItemVM);
					int troopCount = this._initialRoster.GetTroopCount(current.Character);
					if (troopCount > 0)
					{
						manageHideoutTroopItemVM.CurrentAmount = troopCount;
						this._currentTotalSelectedTroopCount += troopCount;
					}
				}
			}
		}

		private void OnRemoveCount(SpousesSelectTroopsItemVM troopItem)
		{
			if (troopItem.CurrentAmount > 0)
			{
				int num = 1;
				if (this.IsEntireStackModifierActive)
				{
					num = Math.Min(troopItem.MaxAmount - troopItem.CurrentAmount, this._maxSelectableTroopCount - this._currentTotalSelectedTroopCount);
				}
				else if (this.IsFiveStackModifierActive)
				{
					num = Math.Min(Math.Min(troopItem.MaxAmount - troopItem.CurrentAmount, this._maxSelectableTroopCount - this._currentTotalSelectedTroopCount), 5);
				}
				troopItem.CurrentAmount -= num;
				this._currentTotalSelectedTroopCount -= num;
			}
			this.OnCurrentSelectedAmountChange();
		}

		private void OnAddCount(SpousesSelectTroopsItemVM troopItem)
		{
			if (troopItem.CurrentAmount < troopItem.MaxAmount && this._currentTotalSelectedTroopCount < this._maxSelectableTroopCount)
			{
				int num = 1;
				if (this.IsEntireStackModifierActive)
				{
					num = Math.Min(troopItem.MaxAmount - troopItem.CurrentAmount, this._maxSelectableTroopCount - this._currentTotalSelectedTroopCount);
				}
				else if (this.IsFiveStackModifierActive)
				{
					num = Math.Min(Math.Min(troopItem.MaxAmount - troopItem.CurrentAmount, this._maxSelectableTroopCount - this._currentTotalSelectedTroopCount), 5);
				}
				troopItem.CurrentAmount += num;
				this._currentTotalSelectedTroopCount += num;
			}
			this.OnCurrentSelectedAmountChange();
		}

		private void OnCurrentSelectedAmountChange()
		{
			using (IEnumerator<SpousesSelectTroopsItemVM> enumerator = this.Troops.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					enumerator.Current.IsRosterFull = (this._currentTotalSelectedTroopCount >= this._maxSelectableTroopCount);
				}
			}
			GameTexts.SetVariable("LEFT", this._currentTotalSelectedTroopCount);
			GameTexts.SetVariable("RIGHT", this._maxSelectableTroopCount);
			this.CurrentSelectedAmountText = GameTexts.FindText("str_LEFT_over_RIGHT_in_paranthesis", null).ToString();
		}

		private void ExecuteDone()
		{
			TroopRoster troopRoster = TroopRoster.CreateDummyTroopRoster();
			foreach (SpousesSelectTroopsItemVM current in this.Troops)
			{
				if (current.CurrentAmount > 0)
				{
					troopRoster.AddToCounts(current.Troop.Character, current.CurrentAmount, false, 0, 0, true, -1);
				}
			}
			this.IsEnabled = false;
			this._onDone.DynamicInvokeWithLog(new object[]
			{
				troopRoster
			});
		}

		private void ExecuteCancel()
		{
			this.IsEnabled = false;
		}

		private void ExecuteReset()
		{
			this.InitList();
			this._maxSelectableTroopCount = _maxNum;
			this.OnCurrentSelectedAmountChange();
		}
	}
}
