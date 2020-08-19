
using SandBox.GauntletUI;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SueMoreSpouses.view
{
    class SpouseClanVM : ViewModel
    {
		private GauntletMovie _currentMovie;
		GauntletLayer _spouseServiceLayer;
		GauntletClanScreen _parentScreen;
		SpouseServiceVM _spouseServiceView;

		

		[DataSourceProperty]
		public string BtnName
		{
			get
			{
				return new TextObject("{=sue_more_spouses_btn_mangager}Spouse Service", null).ToString();
			}
		}

	

		public SpouseClanVM(GauntletClanScreen gauntletClanScreen)
		{
			this._parentScreen = gauntletClanScreen;
		}

		public void ShowSpouseServiceView()
        {
			
				bool flag = this._spouseServiceLayer == null;
				if (flag)
				{
					this._spouseServiceLayer = new GauntletLayer(200, "GauntletLayer");
					this._spouseServiceView = new SpouseServiceVM(this, this._parentScreen);
					this._currentMovie = this._spouseServiceLayer.LoadMovie("SpouseService", this._spouseServiceView);
					this._spouseServiceLayer.IsFocusLayer = true;
					ScreenManager.TrySetFocus(this._spouseServiceLayer);
					this._spouseServiceLayer.Input.RegisterHotKeyCategory(HotKeyManager.GetCategory("GenericPanelGameKeyCategory"));
					this._parentScreen.AddLayer(this._spouseServiceLayer);
					this._spouseServiceLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);
				}
		
		}

		public void CloseSettingView()
		{
			bool flag = this._spouseServiceLayer != null;
			if (flag)
			{
				this._spouseServiceLayer.ReleaseMovie(this._currentMovie);
				this._parentScreen.RemoveLayer(this._spouseServiceLayer);
				this._spouseServiceLayer.InputRestrictions.ResetInputRestrictions();
				this._spouseServiceLayer = null;
				this._spouseServiceView = null;
				this.RefreshValues();
			}
		}

		public bool IsHotKeyPressed(string hotkey)
		{
			bool flag = this._spouseServiceLayer != null;
			return flag && this._spouseServiceLayer.Input.IsHotKeyReleased(hotkey);
		}

	}
}
