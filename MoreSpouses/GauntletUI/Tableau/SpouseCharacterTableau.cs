using System;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;

namespace SueMoreSpouses.Tableau
{
    class SpouseCharacterTableau
    {
		private MatrixFrame _mountSpawnPoint;

		private float _animationFrequencyThreshold = 2.5f;

		private MatrixFrame _frame;

		private MatrixFrame _initialSpawnFrame;

		private AgentVisuals _agentVisuals;

		private GameEntity _mountEntity;

		private Scene _tableauScene;

		private MBAgentRendererSceneController _agentRendererSceneController;

		private Camera _continuousRenderCamera;

		private float _cameraRatio;

		private MatrixFrame _camPos;

		private bool _initialized;

		private int _tableauSizeX;

		private int _tableauSizeY;

		private uint _clothColor1 = new Color(1f, 1f, 1f, 1f).ToUnsignedInteger();

		private uint _clothColor2 = new Color(1f, 1f, 1f, 1f).ToUnsignedInteger();

		private bool _isRotatingCharacter;

		private string _mountCreationKey = "";

		private string _equipmentCode = "";

		private bool _isEquipmentAnimActive;

		private float _animationGap;

		private bool _isEnabled;

		private const float RenderScale = 1f;

		private string _bodyPropertiesCode;

		private BodyProperties _bodyProperties = BodyProperties.Default;

		private bool _isFemale;

		private CharacterViewModel.StanceTypes _stanceIndex;

		private Equipment _equipment;

		private Banner _banner;

		private static readonly ActionIndexCache act_character_developer_idle = ActionIndexCache.Create("act_character_developer_idle");

		private static readonly ActionIndexCache act_cheer_1 = ActionIndexCache.Create("act_cheer_1");

		private static readonly ActionIndexCache act_inventory_idle_start = ActionIndexCache.Create("act_inventory_idle_start");

		private static readonly ActionIndexCache act_inventory_glove_equip = ActionIndexCache.Create("act_inventory_glove_equip");

		private static readonly ActionIndexCache act_inventory_cloth_equip = ActionIndexCache.Create("act_inventory_cloth_equip");

		private static readonly ActionIndexCache act_horse_stand = ActionIndexCache.Create("act_inventory_idle_start");

		private static readonly ActionIndexCache act_camel_stand = ActionIndexCache.Create("act_inventory_idle_start");

		public Texture Texture
		{
			get;
			private set;
		}

		private TableauView View
		{
			get
			{
				if (this.Texture != null)
				{
					return this.Texture.TableauView;
				}
				return null;
			}
		}

		public SpouseCharacterTableau()
		{
			this._equipment = new Equipment();
			this.SetEnabled(true);
		}

		private void SetEnabled(bool enabled)
		{
			this._isEnabled = enabled;
			TableauView view = this.View;
			if (view != null)
			{
				view.SetEnable(this._isEnabled);
			}
		}

		public void SetTargetSize(int width, int height)
		{
			this._isRotatingCharacter = false;
			if (width <= 0 || height <= 0)
			{
				this._tableauSizeX = 10;
				this._tableauSizeY = 10;
			}
			else
			{
				this._tableauSizeX = (int)((float)width * 1f);
				this._tableauSizeY = (int)((float)height * 1f);
			}
			this._cameraRatio = (float)this._tableauSizeX / (float)this._tableauSizeY;
			TableauView view = this.View;
			if (view != null)
			{
				view.SetEnable(false);
			}
			this.Texture = TableauView.AddTableau(new RenderTargetComponent.TextureUpdateEventHandler(this.CharacterTableauContinuousRenderFunction), this._tableauScene, this._tableauSizeX, this._tableauSizeY);
		}

		public void OnFinalize()
		{
			if (this._continuousRenderCamera != null)
			{
				this._continuousRenderCamera.ReleaseCameraEntity();
				this._continuousRenderCamera = null;
			}
			AgentVisuals expr_26 = this._agentVisuals;
			if (expr_26 != null)
			{
				expr_26.Reset();
			}
			this._agentVisuals = null;
			TableauView expr_3E = this.View;
			if (expr_3E != null)
			{
				expr_3E.AddClearTask();
			}
			this.Texture = null;
			if (this._tableauScene != null)
			{
				MBAgentRendererSceneController.DestructAgentRendererSceneController(this._tableauScene, this._agentRendererSceneController);
				this._agentRendererSceneController = null;
				this._tableauScene = null;
			}
		}

		public void SetBodyProperties(string bodyPropertiesCode)
		{
			if (this._bodyPropertiesCode != bodyPropertiesCode)
			{
				this._bodyPropertiesCode = bodyPropertiesCode;
				BodyProperties bodyProperties;
				if (!string.IsNullOrEmpty(bodyPropertiesCode) && BodyProperties.FromString(bodyPropertiesCode, out bodyProperties))
				{
					this._bodyProperties = bodyProperties;
				}
				else
				{
					this._bodyProperties = BodyProperties.Default;
				}
				this._initialized = false;
				this.RefreshCharacterTableau(null);
				this.ForceRefresh();
			}
		}

		public void SetStanceIndex(int index)
		{
			if (this._initialized && this._stanceIndex != (CharacterViewModel.StanceTypes)index)
			{
				switch (index)
				{
					case 0:
						this._agentVisuals.SetAction(SpouseCharacterTableau.act_inventory_idle_start, 0f);
						break;
					case 1:
						this._camPos = this._tableauScene.ReadAndCalculateInitialCamera();
						this._camPos.Elevate(-2f);
						this._camPos.Advance(0.5f);
						break;
					case 2:
					case 4:
						if (this._agentVisuals != null)
						{
							this._camPos = this._tableauScene.ReadAndCalculateInitialCamera();
							if (this._equipment[10].Item != null)
							{
								this._camPos.Advance(0.5f);
								this.AddMount(true);
								this._agentVisuals.SetAction(this._mountEntity.Skeleton.GetActionAtChannel(0), this._mountEntity.Skeleton.GetAnimationParameterAtChannel(0));
								for (int i = 0; i < 4; i++)
								{
									this._mountEntity.Skeleton.TickAnimationsAndForceUpdate(0.1f, this._frame, true);
									this._agentVisuals.GetVisuals().GetSkeleton().TickAnimationsAndForceUpdate(0.1f, this._frame, true);
								}
							}
							else
							{
								this._camPos.Elevate(-2f);
								this._camPos.Advance(0.5f);
								this._agentVisuals.SetAction(SpouseCharacterTableau.act_inventory_idle_start, 0f);
							}
						}
						break;
					case 3:
						{
							AgentVisuals expr_175 = this._agentVisuals;
							if (expr_175 != null)
							{
								expr_175.SetAction(SpouseCharacterTableau.act_cheer_1, 0f);
							}
							break;
						}
				}
			}
			this._stanceIndex = (CharacterViewModel.StanceTypes)index;
		}

		private void ForceRefresh()
		{
			if (this._initialized)
			{
				int stanceIndex = (int)this._stanceIndex;
				this._stanceIndex = CharacterViewModel.StanceTypes.None;
				this.SetStanceIndex(stanceIndex);
			}
		}

		public void SetIsFemale(bool isFemale)
		{
			this._isFemale = isFemale;
		}

		public void SetEquipmentCode(string equipmentCode)
		{
			if (this._equipmentCode != equipmentCode && !string.IsNullOrEmpty(equipmentCode))
			{
				Equipment oldEquipment = Equipment.CreateFromEquipmentCode(this._equipmentCode);
				this._equipmentCode = equipmentCode;
				this._equipment = Equipment.CreateFromEquipmentCode(equipmentCode);
				this.RefreshCharacterTableau(oldEquipment);
			}else if (null == equipmentCode)
			{

			}
		}

		public void SetIsEquipmentAnimActive(bool value)
		{
			this._isEquipmentAnimActive = value;
		}

		public void SetMountCreationKey(string value)
		{
			if (this._mountCreationKey != value)
			{
				this._mountCreationKey = value;
				this.RefreshCharacterTableau(null);
			}
		}

		public void SetBannerCode(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				this._banner = null;
			}
			else
			{
				this._banner = BannerCode.CreateFrom(value).CalculateBanner();
			}
			this.RefreshCharacterTableau(null);
		}

		public void SetArmorColor1(uint clothColor1)
		{
			if (this._clothColor1 != clothColor1)
			{
				this._clothColor1 = clothColor1;
				this.RefreshCharacterTableau(null);
			}
		}

		public void SetArmorColor2(uint clothColor2)
		{
			if (this._clothColor2 != clothColor2)
			{
				this._clothColor2 = clothColor2;
				this.RefreshCharacterTableau(null);
			}
		}

		private void RefreshCharacterTableau(Equipment oldEquipment = null)
		{
			if (!this._initialized)
			{
				this.FirstTimeInit();
			}
			if (this._initialized)
			{
				if (this._agentVisuals != null)
				{
					AgentVisualsData copyAgentVisualsData = this._agentVisuals.GetCopyAgentVisualsData();
					copyAgentVisualsData.BodyProperties(this._bodyProperties).SkeletonType(this._isFemale ? SkeletonType.Female : SkeletonType.Male).Frame(this._initialSpawnFrame).ActionSet(MBGlobals.HumanWarriorActionSet).Equipment(this._equipment).Banner(this._banner).UseMorphAnims(true).ClothColor1(this._clothColor1).ClothColor2(this._clothColor2);
					this._agentVisuals.Refresh(false, copyAgentVisualsData);
					if (oldEquipment != null && this._animationFrequencyThreshold <= this._animationGap && this._isEquipmentAnimActive)
					{
						if (this._equipment[EquipmentIndex.Gloves].Item != null && oldEquipment[EquipmentIndex.Gloves].Item != this._equipment[EquipmentIndex.Gloves].Item)
						{
							this._agentVisuals.GetVisuals().GetSkeleton().SetAgentActionChannel(0, SpouseCharacterTableau.act_inventory_glove_equip, 0f, -0.2f);
							this._animationGap = 0f;
						}
						else if (this._equipment[EquipmentIndex.Body].Item != null && oldEquipment[EquipmentIndex.Body].Item != this._equipment[EquipmentIndex.Body].Item)
						{
							this._agentVisuals.GetVisuals().GetSkeleton().SetAgentActionChannel(0, SpouseCharacterTableau.act_inventory_cloth_equip, 0f, -0.2f);
							this._animationGap = 0f;
						}
					}
				}
				if (this._equipment[10].Item != null)
				{
					this.AddMount(this._stanceIndex == CharacterViewModel.StanceTypes.OnMount);
					return;
				}
				this.RemoveMount();
			}
		}

		public void RotateCharacter(bool value)
		{
			this._isRotatingCharacter = value;
		}

		public void OnTick(float dt)
		{
			TickUserInputs(dt);
			if (this._isEnabled && this._isRotatingCharacter)
			{
				this.UpdateCharacterRotation((int)Input.MouseMoveX);
			}
			if (this._animationFrequencyThreshold > this._animationGap)
			{
				this._animationGap += dt;
			}
			if (this._isEnabled && this._agentVisuals != null)
			{
				this._agentVisuals.TickVisuals();
			}
			TableauView view = this.View;
			if (view != null)
			{
				if (this._continuousRenderCamera == null)
				{
					this._continuousRenderCamera = Camera.CreateCamera();
				}
				view.SetDoNotRenderThisFrame(false);
			}
		}

		private bool _RightMousePressed;

		private void TickUserInputs(float dt)
		{
			if (Input.DeltaMouseScroll != 0f)
			{
				float advance = Input.DeltaMouseScroll  / 720f;
				//this._camPos.Advance(advance); 上下移动
				this._camPos.Elevate(advance);
			}

			if (Input.IsPressed(InputKey.RightMouseButton))
			{
				this._RightMousePressed = true;
			
			}

			if (Input.IsReleased(InputKey.RightMouseButton))
			{
				this._RightMousePressed = false;

			}

			if (_RightMousePressed)
			{
				GameEntity entity = this._agentVisuals.GetEntity();
				this._frame = entity.GetFrame();
				this._frame.rotation.RotateAboutSide(-(float)Input.MouseMoveY * 3.14159274f / 720f);
				entity.SetFrame(ref this._frame);
			}
		}

		public void OnCharacterTableauMouseMove(int mouseMoveX)
		{
			this.UpdateCharacterRotation(mouseMoveX);
		}

		private void UpdateCharacterRotation(int mouseMoveX)
		{
			if (this._initialized && this._agentVisuals != null)
			{
				GameEntity entity = this._agentVisuals.GetEntity();
				this._frame = entity.GetFrame();
				this._frame.rotation.RotateAboutUp((float)mouseMoveX * 3.14159274f / 720f);
				//this._frame.rotation.RotateAboutSide((float)mouseMoveY * 3.14159274f / 720f);
				entity.SetFrame(ref this._frame);
			}
		}

		private void FirstTimeInit()
		{
			if (this._equipment != null)
			{
				if (this._tableauScene == null)
				{
					this._tableauScene = Scene.CreateNewScene(true);
					this._tableauScene.SetName("CharacterTableau");
					this._tableauScene.DisableStaticShadows(true);
					this._agentRendererSceneController = MBAgentRendererSceneController.CreateNewAgentRendererSceneController(this._tableauScene, 32);
					SceneInitializationData initData = new SceneInitializationData(true);
					initData.InitPhysicsWorld = false;
					this._tableauScene.Read("inventory_character_scene", initData);
					this._tableauScene.SetShadow(true);
					this._camPos = this._tableauScene.ReadAndCalculateInitialCamera();
					this._mountSpawnPoint = this._tableauScene.FindEntityWithTag("horse_inv").GetGlobalFrame();
					MatrixFrame globalFrame = this._tableauScene.FindEntityWithTag("agent_inv").GetGlobalFrame();
					this._frame = globalFrame;
					this._initialSpawnFrame = globalFrame;
					this._tableauScene.RemoveEntity(this._tableauScene.FindEntityWithTag("agent_inv"), 99);
					this._tableauScene.RemoveEntity(this._tableauScene.FindEntityWithTag("horse_inv"), 100);
				}
				if (this._agentVisuals != null)
				{
					this._agentVisuals.Reset();
					this._agentVisuals = null;
				}
				if (this._bodyProperties != BodyProperties.Default)
				{
					this._agentVisuals = AgentVisuals.Create(new AgentVisualsData().Banner(this._banner).Equipment(this._equipment).BodyProperties(this._bodyProperties).Frame(this._frame).UseMorphAnims(true).ActionSet(MBGlobals.HumanWarriorActionSet).ActionCode(SpouseCharacterTableau.act_inventory_idle_start).Scene(this._tableauScene).Monster(Game.Current.HumanMonster).PrepareImmediately(true).SkeletonType(this._isFemale ? SkeletonType.Female : SkeletonType.Male).ClothColor1(this._clothColor1).ClothColor2(this._clothColor2), "CharacterTableaue", true, false);
					this._agentVisuals.SetAgentLodLevelExternal(0f);
				}
				this._initialized = true;
			}
		}

		private void AddMount(bool isRiderAgentMounted = false)
		{
			this.RemoveMount();
			HorseComponent horseComponent = this._equipment[10].Item.HorseComponent;
			Monster arg_39_0 = horseComponent.Monster;
			this._mountEntity = GameEntity.CreateEmpty(this._tableauScene, true);
			AnimationSystemData animationSystemData = arg_39_0.FillAnimationSystemData(MBGlobals.GetActionSet(horseComponent.Monster.ActionSetCode), 1f, false);
			AgentVisualsNativeData agentVisualsNativeData = arg_39_0.FillAgentVisualsNativeData();
			this._mountEntity.CreateSkeletonWithActionSet(ref agentVisualsNativeData, ref animationSystemData);
			if (isRiderAgentMounted)
			{
				this._mountEntity.Skeleton.SetAgentActionChannel(0, (horseComponent.Monster.MonsterUsage == "camel") ? SpouseCharacterTableau.act_camel_stand : SpouseCharacterTableau.act_horse_stand, MBRandom.RandomFloat, -0.2f);
			}
			else
			{
				this._mountEntity.Skeleton.SetAgentActionChannel(0, SpouseCharacterTableau.act_inventory_idle_start, MBRandom.RandomFloat, -0.2f);
			}
			this._mountEntity.EntityFlags |= EntityFlags.AnimateWhenVisible;
			MountVisualCreator.AddMountMeshToEntity(this._mountEntity, this._equipment[10].Item, this._equipment[11].Item, this._mountCreationKey, null);
			this._mountEntity.SetFrame(ref this._mountSpawnPoint);
		}

		private void RemoveMount()
		{
			if (this._mountEntity != null)
			{
				this._mountEntity.ClearComponents();
				this._mountEntity.Remove(101);
				this._mountEntity = null;
			}
		}

		internal void CharacterTableauContinuousRenderFunction(Texture sender, EventArgs e)
		{
			Scene scene = (Scene)sender.UserData;
			TableauView tableauView = sender.TableauView;
			if (scene == null)
			{
				tableauView.SetContinuousRendering(false);
				tableauView.SetDeleteAfterRendering(true);
				return;
			}
			scene.EnsurePostfxSystem();
			scene.SetDofMode(false);
			scene.SetMotionBlurMode(false);
			scene.SetBloom(true);
			scene.SetDynamicShadowmapCascadesRadiusMultiplier(0.31f);
			tableauView.SetRenderWithPostfx(true);
			float cameraRatio = this._cameraRatio;
			MatrixFrame camPos = this._camPos;
			if (this._continuousRenderCamera != null)
			{
				Camera continuousRenderCamera = this._continuousRenderCamera;
				this._continuousRenderCamera = null;
				continuousRenderCamera.SetFovVertical(0.7853982f, cameraRatio, 0.2f, 200f);
				continuousRenderCamera.Frame = camPos;
				tableauView.SetCamera(continuousRenderCamera);
				tableauView.SetScene(scene);
				tableauView.SetSceneUsesSkybox(false);
				tableauView.SetDeleteAfterRendering(false);
				tableauView.SetContinuousRendering(true);
				tableauView.SetDoNotRenderThisFrame(true);
				tableauView.SetClearColor(0u);
				tableauView.SetFocusedShadowmap(true, ref this._frame.origin, 1.55f);
			}
		}
	}
}
