using SandBox.View.Map;
using SandBox.View.Menu;
using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.GameMenu.ManageHideoutTroops;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.View.Missions;

namespace SueMoreSpouses.view.troop
{
    [OverrideView(typeof(SpousesDefaultSelectTroops))]
    class GauntletMenuSpousesSelectTroops : MenuView
    {
		private readonly TroopRoster _initialRoster;
		private readonly Action<TroopRoster> _onDone;

		private readonly Func<CharacterObject, bool> _changeChangeStatusOfTroop;

		private readonly Action _onCanel;

		private GauntletLayer _gauntletLayer;

		private SpousesSelectTroopsVM _dataSource;

		private GauntletMovie _movie;

		private int _maxNum;

		public GauntletMenuSpousesSelectTroops(TroopRoster initialRoster, int maxNum ,Func<CharacterObject, bool> changeChangeStatusOfTroop, Action<TroopRoster> onDone, Action onCanel)
		{
			this._onCanel = onCanel;
			this._onDone = onDone;
			this._initialRoster = initialRoster;
			this._maxNum = maxNum;
			this._changeChangeStatusOfTroop = changeChangeStatusOfTroop;
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._dataSource = new SpousesSelectTroopsVM(this._initialRoster, this._maxNum, this._changeChangeStatusOfTroop, this._onDone)
			{
				IsEnabled = true
			};
			this._gauntletLayer = new GauntletLayer(205, "GauntletLayer")
			{
				Name = "MenuSpouseSelectTroops"
			};
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
			this._gauntletLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericCampaignPanelsGameKeyCategory"));
			this._movie = this._gauntletLayer.LoadMovie("SpousesSelectTroops", this._dataSource);
			this._gauntletLayer.IsFocusLayer = true;
			ScreenManager.TrySetFocus(this._gauntletLayer);
			base.MenuViewContext.AddLayer(this._gauntletLayer);
			MapScreen mapScreen;
			if ((mapScreen = (ScreenManager.TopScreen as MapScreen)) != null)
			{
				mapScreen.IsInHideoutTroopManage = true;
			}
		}

		protected override void OnFinalize()
		{
			this._gauntletLayer.IsFocusLayer = false;
			ScreenManager.TryLoseFocus(this._gauntletLayer);
			this._dataSource.OnFinalize();
			this._dataSource = null;
			this._gauntletLayer.ReleaseMovie(this._movie);
			base.MenuViewContext.RemoveLayer(this._gauntletLayer);
			this._movie = null;
			this._gauntletLayer = null;
			MapScreen mapScreen;
			if ((mapScreen = (ScreenManager.TopScreen as MapScreen)) != null)
			{
				mapScreen.IsInHideoutTroopManage = false;
			}
			base.OnFinalize();
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this._dataSource.IsFiveStackModifierActive = (this._gauntletLayer.Input.IsHotKeyDown("FiveStackModifier") || this._gauntletLayer.Input.IsHotKeyDown("FiveStackModifierAlt"));
			this._dataSource.IsEntireStackModifierActive = (this._gauntletLayer.Input.IsHotKeyDown("EntireStackModifier") || this._gauntletLayer.Input.IsHotKeyDown("EntireStackModifierAlt"));
			this._gauntletLayer.Input.IsHotKeyReleased("Exit");
			if (this._gauntletLayer.Input.IsHotKeyReleased("Exit"))
			{
				this._dataSource.IsEnabled = false;
			}
			if (!this._dataSource.IsEnabled)
			{
				//这里先留坑
				//base.MenuViewContext.CloseManageHideoutTroops();
				//base.MenuViewContext.RemoveMenuView(this._menuManageHideoutTroops);
				if (null != this._onCanel)
				{
					this._onCanel();
				}
			}
		}
	}
}
