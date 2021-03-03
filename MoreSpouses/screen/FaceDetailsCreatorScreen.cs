using StoryMode.View.CharacterCreationSystem;
using SueMoreSpouses.Screen.State;
using SueMoreSpouses.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.Engine.Screens;
using TaleWorlds.InputSystem;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.View.Screen;

namespace SueMoreSpouses.Screen
{
	[GameStateScreen(typeof(FaceDetailsCreatorState))]
	class FaceDetailsCreatorScreen : ScreenBase, IGameStateListener
    {

        private BodyGeneratorView _facegenLayer;
		private bool _oldGameStateManagerDisabledStatus;
		FaceDetailsCreatorState _faceDetailsCreatorState;
		public FaceDetailsCreatorScreen(FaceDetailsCreatorState faceDetailsCreatorState)
		{
			LoadingWindow.EnableGlobalLoadingWindow(false);
			this._faceDetailsCreatorState = faceDetailsCreatorState;

			this._facegenLayer = new BodyGeneratorView(new ControlCharacterCreationStage(this.OnExit), GameTexts.FindText("str_done", null), new ControlCharacterCreationStage(this.OnExit), GameTexts.FindText("str_cancel", null), faceDetailsCreatorState.EditHero.CharacterObject, false, null);
		}

		protected override void OnFrameTick(float dt)
		{
			base.OnFrameTick(dt);
			this._facegenLayer.OnTick(dt);
			if (this._facegenLayer.SceneLayer.Input.IsHotKeyReleased("Exit"))
			{
				this.OnExit();
			}
		}
	
		public void OnExit()
		{
			HeroFaceUtils.UpdatePlayerCharacterBodyProperties(this._faceDetailsCreatorState.EditHero, this._facegenLayer.BodyGen.CurrentBodyProperties, this._facegenLayer.BodyGen.IsFemale);
			Game.Current.GameStateManager.PopState(0);
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._oldGameStateManagerDisabledStatus = Game.Current.GameStateManager.ActiveStateDisabledByUser;
			Game.Current.GameStateManager.ActiveStateDisabledByUser = true;
			base.AddLayer(this._facegenLayer.GauntletLayer);
		}

		protected override void OnFinalize()
		{
			base.OnFinalize();
			if (LoadingWindow.GetGlobalLoadingWindowState())
			{
				LoadingWindow.DisableGlobalLoadingWindow();
			}
			Game.Current.GameStateManager.ActiveStateDisabledByUser = this._oldGameStateManagerDisabledStatus;
		}

		protected override void OnActivate()
		{
			base.OnActivate();
			base.AddLayer(this._facegenLayer.SceneLayer);
		}

		protected override void OnDeactivate()
		{
			base.OnDeactivate();
			this._facegenLayer.OnFinalize();
			LoadingWindow.EnableGlobalLoadingWindow(false);
			InformationManager.HideInformations();
			Mission current = Mission.Current;
			if (current != null)
			{
				foreach (Agent expr_39 in current.Agents)
				{
					expr_39.EquipItemsFromSpawnEquipment();
					expr_39.UpdateAgentProperties();
				}
			}
		}

		void IGameStateListener.OnActivate()
		{
		}

		void IGameStateListener.OnDeactivate()
		{
		}

		void IGameStateListener.OnInitialize()
		{
		}

		void IGameStateListener.OnFinalize()
		{
		}

	}
}
