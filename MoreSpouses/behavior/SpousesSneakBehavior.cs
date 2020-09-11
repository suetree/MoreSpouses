using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.GameMenus;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.ObjectSystem;
using TaleWorlds.CampaignSystem.Overlay;
using TaleWorlds.CampaignSystem.SandBox;
using SueMoreSpouses.utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Conversation.Tags;
using TaleWorlds.Library;
using SandBox.View.Menu;
using SueMoreSpouses.view.troop;

namespace SueMoreSpouses.behavior
{

	//tavern   lordshall  prison  center
	class SpousesSneakBehavior : CampaignBehaviorBase
	{
		
		Hero _sneaker;
		int _alertCoefficient = 0;
		int _alertCoefficientReduceWeek = 0;

		CampaignGameStarter _gameStarter;

		IEnumerable<MobileParty> _parties;

		IEnumerable<Hero> _lordHeros;

		MobileParty _tempMobile;

		List<CharacterObject> _prisoners;

		private int AlertReducWeekPeriod = 2; //警戒降低，周期， 单位周

		private int SneakMaxNum = 20;

		private int EscapeAlertNum = 10; //偷袭者会逃跑的警戒值

		private int AlertRate = 4; //3能开始打一场

		private MobileParty _tempTargetParty;
		private Settlement _lastSettlement;

		public override void RegisterEvents()
		{
			CampaignEvents.DailyTickEvent.AddNonSerializedListener(this, new Action(this.DailyTick));
			CampaignEvents.WeeklyTickEvent.AddNonSerializedListener(this, new Action(this.WeeklyTick));
			CampaignEvents.OnSessionLaunchedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.OnSessionLaunched));
			CampaignEvents.LocationCharactersAreReadyToSpawnEvent.AddNonSerializedListener(this, new Action<Dictionary<string, int>>(this.LocationCharactersAreReadyToSpawn));
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.AddGameMenu));
			CampaignEvents.HeroKilledEvent.AddNonSerializedListener(this, new Action<Hero, Hero, KillCharacterAction.KillCharacterActionDetail, bool>(this.HeroKilled));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.AddGameMenu));
			CampaignEvents.OnPlayerBattleEndEvent.AddNonSerializedListener(this, new Action<MapEvent>(this.OnPlayerBattleEnd));
		}

		public override void SyncData(IDataStore dataStore)
		{
			dataStore.SyncData<Hero>("Sneaker", ref this._sneaker);
			dataStore.SyncData<int>("AlertCoefficient", ref this._alertCoefficient);
			dataStore.SyncData<int>("AlertCoefficientWeek", ref this._alertCoefficientReduceWeek);
		}

		private void HeroKilled(Hero victim, Hero killer, KillCharacterAction.KillCharacterActionDetail actionDetail, bool showNotification)
		{
			if (null != this._sneaker && victim == this._sneaker)
			{
				///this._sneaker = null;
			}
		}


		private void OnPlayerBattleEnd(MapEvent mapEvent)
		{
			if (null != this._tempMobile)
			{
				if (mapEvent.WinningSide == mapEvent.PlayerSide)
				{
					AlertCoefficientIncrease();
					ICollection<TroopRosterElement> result = _tempMobile.MemberRoster.RemoveIf(obj => obj.Character != null);
					foreach (TroopRosterElement element in result)
					{
						if (element.Character.IsHero)
						{
							MobileParty.MainParty.MemberRoster.AddToCounts(element.Character, 1);
						}
						else
						{
							MobileParty.MainParty.MemberRoster.AddToCounts(element.Character, element.Number);
						}
					}

					if (null != this._prisoners && this._prisoners.Count > 0)
					{
						foreach (CharacterObject character in this._prisoners)
						{
							if (character.IsHero)
							{
								PartyBase partyBase = character.HeroObject.PartyBelongedToAsPrisoner;
								if (null != partyBase)
								{
									partyBase.MemberRoster.AddToCounts(character, -1);
									
									if (character.HeroObject.Clan == Clan.PlayerClan)
									{
										character.HeroObject.ChangeState(Hero.CharacterStates.Active);
										MobileParty.MainParty.MemberRoster.AddToCounts(character, 1);
										InformationManager.DisplayMessage(new InformationMessage("成功营救" + character.HeroObject.Name, Colors.Green));
									}
									else
									{
										MobileParty.MainParty.AddPrisoner(character, 1);
									}
								}
							}
							
						}
					}
				}
				else
				{
					AlertCoefficientReduce();
					this._tempMobile.MemberRoster.Reset();
					if(null != this._tempTargetParty)
					{
						this._tempTargetParty.MemberRoster.Reset();
						if(this._tempTargetParty.PrisonRoster.Count > 0)
						{
							if (null != this._lastSettlement && this._lastSettlement.Parties.Count > 0)
							{
								
								this._lastSettlement.Parties[0].PrisonRoster.Add(this._tempTargetParty.PrisonRoster.ToFlattenedRoster());
								this._lastSettlement = null;
							}
							else
							{
								this._tempTargetParty.PrisonRoster.Reset();
							}
						
						}
						this._tempTargetParty = null;
					}
				}
				this._tempMobile = null;
				
			}
		}

		private void AlertCoefficientReduce()
		{
			this._alertCoefficient--;
			if (this._alertCoefficient < 0)
			{
				this._alertCoefficient = 0;
			}
			InformationManager.DisplayMessage(new InformationMessage("警戒系数" + this._alertCoefficient, Colors.Green));
		}

		private void AlertCoefficientIncrease()
		{
			this._alertCoefficient++;
			if (this._alertCoefficient > 10)
			{
				this._alertCoefficient = 10;
			}
			InformationManager.DisplayMessage(new InformationMessage("警戒系数" + this._alertCoefficient, Colors.Red));

			if (this._alertCoefficient >= EscapeAlertNum)
			{
				if (null != this._sneaker && MobileParty.MainParty.MemberRoster.Contains(this._sneaker.CharacterObject))
				{
					MobileParty.MainParty.MemberRoster.AddToCounts(this._sneaker.CharacterObject, -1);
					this._sneaker.ChangeState(Hero.CharacterStates.Active);
					InformationManager.AddQuickInformation(new TextObject(this._sneaker.Name.ToString() + "已经跑路"),
					  0, null, "event:/ui/notification/alert");
				}
			}
			
		}

		public void AddGameMenu(CampaignGameStarter gameStarter)
		{
			this._gameStarter = gameStarter;

			//PlayerTownVisitCampaignBehavior
			gameStarter.AddGameMenuOption("town", "sms_sneak", "执行秘密任务", ShowCondition, SwitchStealMenuStart, false, 3, false);
			gameStarter.AddGameMenuOption("castle", "sms_sneak", "执行秘密任务", ShowCondition, SwitchStealMenuStart, false, 3, false);
			
			gameStarter.AddGameMenu("sms_sneak", "听取专业人士建议, 袭击领主，将会袭击没带部队的领主. 袭击部队, 将会袭击正在呆在城里的部队. 袭击监狱，将会获得所有囚犯. 袭击酒馆将获得一些", new OnInitDelegate(MenuInit), GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.none, null);
			gameStarter.AddGameMenuOption("sms_sneak", "sms_sneak_lords_shall", "袭击领主", HasLordWithOutParty, SwitchStealMenuLordWithoutParty, false, -1, false);
			gameStarter.AddGameMenuOption("sms_sneak", "sms_sneak_party_center", "袭击部队", HasLordParty, SwitchStealMenuParty, false, -1, false);
			gameStarter.AddGameMenuOption("sms_sneak", "sms_sneak_party_tavern", "袭击酒馆", HasTavern, BattleInTavern, false, -1, false);
			gameStarter.AddGameMenuOption("sms_sneak", "sms_sneak_party_prison", "袭击监牢", HasPrison, BattleInPrison, false, -1, false);

			gameStarter.AddGameMenuOption("sms_sneak", "sms_sneak_leave", "有人来了赶紧跑", null, Leave, true, -1, false);

		}

		private void MenuInit(MenuCallbackArgs args)
		{
			this._lastSettlement = Settlement.CurrentSettlement;
			this._parties = Settlement.CurrentSettlement.Parties.Where(obj =>  null != obj.ActualClan &&  obj.ActualClan != Clan.PlayerClan );
			this._lordHeros = Settlement.CurrentSettlement.HeroesWithoutParty.Where(obj => obj.CharacterObject.Occupation == Occupation.Lord && obj.Clan != Clan.PlayerClan);
			if(null != this._prisoners) this._prisoners.Clear();
			if (null != Settlement.CurrentSettlement.GetComponent<Town>())
			{
				this._prisoners = Settlement.CurrentSettlement.GetComponent<Town>().GetPrisonerHeroes();
			}
			

			GameMenuManager gameMenuManager = (GameMenuManager)ReflectUtils.ReflectField("_gameMenuManager", _gameStarter);
			if (null != gameMenuManager)
			{
				Dictionary<string, GameMenu> gamemenu = (Dictionary<string, GameMenu>)ReflectUtils.ReflectField("_gameMenus", gameMenuManager);
				List<string> menuKey = gamemenu.Keys.ToList();
				List<GameMenu> menuValue = gamemenu.Values.ToList();
				for (int i = 1; i < menuValue.Count; i++)
				{
					if (menuValue[i].StringId == "sms_sneak_party" || menuValue[i].StringId == "sms_sneak_lord_whitout_party")
					{
						gamemenu.Remove(menuKey[i]);
					}
				}
			}
			MenuPartyInit(args);
			MenuLordWithoutPartyInit(args);
		}

		private  void MenuPartyInit(MenuCallbackArgs args)
		{
			
			_gameStarter.AddGameMenu("sms_sneak_party", "此时，你看到一些部队正在操练， 选择一支部队进行袭击", null, GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.none, null);
			if (null != this._parties  && this._parties.Count() > 0)
			{
				foreach (MobileParty current in this._parties)
				{
					_gameStarter.AddGameMenuOption("sms_sneak_party", "sms_sneak_party_name", current.Name.ToString(), null, BattleInCenter, false, -1, false);
				}
			}

			_gameStarter.AddGameMenuOption("sms_sneak_party", "sms_sneak_leave", "有人来了赶紧跑", null, Leave, true, -1, false);
		}

		private void MenuLordWithoutPartyInit(MenuCallbackArgs args)
		{
			_gameStarter.AddGameMenu("sms_sneak_lord_whitout_party", "此时，你看到一些领主正在大厅，带着微量部队", null, GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.none, null);
			if (null != this._lordHeros && this._lordHeros.Count() > 0)
			{
				foreach (Hero current in this._lordHeros)
				{
					_gameStarter.AddGameMenuOption("sms_sneak_lord_whitout_party", "sms_sneak_hero_name", current.Name.ToString(), null, BattleInLordShall, false, -1, false);
				}
			}

			_gameStarter.AddGameMenuOption("sms_sneak_lord_whitout_party", "sms_sneak_leave", "有人来了赶紧跑", null, Leave, true, -1, false);
		}

		public bool ShowCondition(MenuCallbackArgs args)
		{
			return (null != this._sneaker  
				&& MobileParty.MainParty.MemberRoster.Contains(this._sneaker.CharacterObject)
				&& Campaign.Current.IsNight
				);
		}

		public void SwitchStealMenuStart(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("sms_sneak");
		}

		public void SwitchStealMenuParty(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("sms_sneak_party");
		}


		public void SwitchStealMenuLordWithoutParty(MenuCallbackArgs args)
		{
			GameMenu.SwitchToMenu("sms_sneak_lord_whitout_party");
		}

		public bool HasLordParty(MenuCallbackArgs args)
		{
			return Settlement.CurrentSettlement.Parties.Where(obj => null != obj.ActualClan &&  obj.ActualClan != Clan.PlayerClan).Count() > 0;
		}

		public bool HasLordWithOutParty(MenuCallbackArgs args)
		{
			return Settlement.CurrentSettlement.HeroesWithoutParty.Where(obj => obj.CharacterObject.Occupation == Occupation.Lord && obj.Clan != Clan.PlayerClan).Count() > 0;
		}


		public bool HasTavern(MenuCallbackArgs args)
		{
			return Settlement.CurrentSettlement.IsTown;
		}

		public bool HasPrison(MenuCallbackArgs args)
		{
			return null != Settlement.CurrentSettlement.GetComponent<Town>() && Settlement.CurrentSettlement.GetComponent<SettlementComponent>().GetPrisonerHeroes().Count > 0;
		}

		private void BattleInTavern(MenuCallbackArgs args)
		{

			int upgradeLevel = Settlement.CurrentSettlement.GetComponent<Town>().GetWallLevel();
			String scene = LocationComplex.Current.GetLocationWithId("tavern").GetSceneName(upgradeLevel);
			int num = Campaign.Current.Models.LocationModel.GetSettlementUpgradeLevel(PlayerEncounter.LocationEncounter);

			//CampaignMission.OpenIndoorMission(scene, num, LocationComplex.Current.GetLocationWithId("prison"), null);
			this._tempTargetParty = MBObjectManager.Instance.CreateObject<MobileParty>("sms_prison");
			AddRandomTroopToParty(this._tempTargetParty);
			SelectMainPartyMember(args, () => {
				PreBattle(this._tempTargetParty);
				OpenBattleJustHero(scene, upgradeLevel);
			}, 20);

		}

		private void BattleInPrison(MenuCallbackArgs args)
		{
		
			int upgradeLevel = Settlement.CurrentSettlement.GetComponent<Town>().GetWallLevel();
			String scene = LocationComplex.Current.GetLocationWithId("prison").GetSceneName(upgradeLevel);
			int num = Campaign.Current.Models.LocationModel.GetSettlementUpgradeLevel(PlayerEncounter.LocationEncounter);

			//CampaignMission.OpenIndoorMission(scene, num, LocationComplex.Current.GetLocationWithId("prison"), null);
			this._tempTargetParty = MBObjectManager.Instance.CreateObject<MobileParty>("sms_prison");
			AddRandomTroopToParty(this._tempTargetParty);
			SelectMainPartyMember(args, () => {
				PreBattle(this._tempTargetParty);
				OpenBattleJustHero(scene, upgradeLevel);
			}, 20);

		}

		public void BattleInCenter(MenuCallbackArgs args)
		{
			MobileParty targetParty = this._parties.Where(obj => obj.Name.ToString() == args.Text.ToString()).GetRandomElement();
			if (null == targetParty)
			{
				PlayerEncounter.LeaveSettlement();
				PlayerEncounter.Finish(true);
				return;
			}

			int upgradeLevel = Settlement.CurrentSettlement.GetComponent<Town>().GetWallLevel();
			String scene = LocationComplex.Current.GetLocationWithId("center").GetSceneName(upgradeLevel);
			PreBattle(targetParty);
			StartBattleNormal(scene, upgradeLevel);

			
		}

		public void BattleInLordShall(MenuCallbackArgs args)
		{
			Hero target = this._lordHeros.Where(obj => obj.Name.ToString() == args.Text.ToString()).GetRandomElement();
			if (null == target)
			{
				PlayerEncounter.LeaveSettlement();
				PlayerEncounter.Finish(true);
				return;
			}
			int upgradeLevel = Settlement.CurrentSettlement.GetComponent<Town>().GetWallLevel();
			String scene = LocationComplex.Current.GetLocationWithId("lordshall").GetSceneName(upgradeLevel);

			MobileParty targetParty = target.Clan.CreateNewMobileParty(target);
			AddRandomTroopToParty(targetParty);
			SelectMainPartyMember(args, () => {
				PreBattle(targetParty);
				OpenBattleJustHero(scene, upgradeLevel);
			}, SneakMaxNum);
			
			//InformationManager.AddQuickInformation();
		}

		private bool CanChangeStatusOfTroop(CharacterObject character)
		{
			return !character.IsPlayerCharacter 
				&& character != this._sneaker.CharacterObject
				&& !character.IsNotTransferable && character.IsHero;
		}


		private void AddRandomTroopToParty(MobileParty targetParty)
		{
			int num = 5 + AlertRate * this._alertCoefficient;
			
			CharacterObject character = CharacterObject.All.Where(obj => obj.IsSoldier && obj.Tier >= 4 && obj.Culture == Settlement.CurrentSettlement.Culture).GetRandomElement();
			targetParty.MemberRoster.AddToCounts(character, num);
			character = CharacterObject.All.Where(obj => obj.IsInfantry && obj.IsSoldier && obj.Tier < 4 && obj.Culture == Settlement.CurrentSettlement.Culture).GetRandomElement();
			targetParty.MemberRoster.AddToCounts(character, num);
		}

		private void SelectMainPartyMember(MenuCallbackArgs args, Action nextStep, int maxNum)
		{
			if (null == this._tempMobile)
			{
				this._tempMobile = MBObjectManager.Instance.CreateObject<MobileParty>("sms_sneak_temp_party");
			}
			else
			{
				this._tempMobile.MemberRoster.Reset();
			}

			int count = MobileParty.MainParty.MemberRoster.Count;
		
			TroopRoster strongestTroopRoster = TroopRoster.CreateDummyTroopRoster();
			List<Hero> includeHeros = new List<Hero>();
			includeHeros.Add(Hero.MainHero);
			includeHeros.Add(this._sneaker);
			FlattenedTroopRoster strongestAndPriorTroops = GameComponent.GetStrongestAndPriorTroops(MobileParty.MainParty, maxNum, includeHeros);
			strongestTroopRoster.Add(strongestAndPriorTroops);
			bool execueted = OpenSlelectTroops(args, strongestTroopRoster, maxNum, new Func<CharacterObject, bool>(this.CanChangeStatusOfTroop), new Action<TroopRoster>((troop) => {
				DealPatyTroop(troop);
				nextStep();
				
			}));
			if (!execueted)
			{   //如果没有执行，就走随机筛选
				TroopRoster troopRosters = MobileParty.MainParty.MemberRoster;
				TroopRoster battleTroopRoster = TroopRoster.CreateDummyTroopRoster();
				foreach (TroopRosterElement element in troopRosters)
				{
					if (element.Character.IsHero && !element.Character.IsPlayerCharacter)
					{
						if (battleTroopRoster.Count < maxNum)
						{
							battleTroopRoster.AddToCounts(element.Character, element.Number);
						}
					}
				};
				DealPatyTroop(battleTroopRoster);
				nextStep();
			}
			
		}

		private void DealPatyTroop(TroopRoster battleTroopRoster)
		{
			TroopRoster troopRosters = MobileParty.MainParty.MemberRoster;
			ICollection<TroopRosterElement> needMoveTroop =  troopRosters.RemoveIf(obj => !battleTroopRoster.Contains(obj.Character));
			foreach (TroopRosterElement element in needMoveTroop)
			{
				if (element.Character.IsHero)
				{
					this._tempMobile.MemberRoster.AddToCounts(element.Character, 1);
				}
				else
				{
					this._tempMobile.MemberRoster.AddToCounts(element.Character, element.Number);
				}
			}
		}

		bool OpenSlelectTroops(MenuCallbackArgs args, TroopRoster initialRoster, int maxNum, Func<CharacterObject, bool> canChangeStatusOfTroop, Action<TroopRoster> onDone)
		{
			bool excueted = false; ;
			if (null != args.MenuContext.Handler as MenuViewContext)
			{
				excueted = true;
				MenuViewContext menuViewContext = (MenuViewContext)args.MenuContext.Handler;
				MenuView menuView = null; 
				 menuView =  menuViewContext.AddMenuView<SpousesDefaultSelectTroops>(new object[]
				{
				  initialRoster,
				  maxNum,
				  canChangeStatusOfTroop,
				  onDone,
				  new Action(() => {
					  if(null != menuView)
				      menuViewContext.RemoveMenuView(menuView);
				  }) 
				});
			}

			return excueted;
		}

		private void PreBattle(MobileParty targetParty)
		{
			PlayerEncounter.LeaveSettlement();
			PlayerEncounter.Finish(true);
			StartBattleAction.Apply(targetParty.Party, MobileParty.MainParty.Party);
			PlayerEncounter.RestartPlayerEncounter(MobileParty.MainParty.Party, targetParty.Party, true);
			PlayerEncounter.Update();
		}

		private void StartBattleNormal(String scene, int upgradeLevel)
		{
			string civilianUpgradeLevelTag = Campaign.Current.Models.LocationModel.GetCivilianUpgradeLevelTag(upgradeLevel);
			GameComponent.OpenBattleNormal(scene, civilianUpgradeLevelTag);
		}

		private void OpenBattleJustHero(String scene, int upgradeLevel)
		{
			string civilianUpgradeLevelTag = Campaign.Current.Models.LocationModel.GetCivilianUpgradeLevelTag(upgradeLevel);
			GameComponent.OpenBattleJustHero(scene, civilianUpgradeLevelTag);
		}

		public void Leave(MenuCallbackArgs args)
		{
			PlayerEncounter.LeaveSettlement();
			PlayerEncounter.Finish(true);
		}


		public void OnSessionLaunched(CampaignGameStarter starter)
		{
			starter.AddDialogLine("sms_sneak_talk", "start", "sms_sneak_talk_resquest", "我会使用爬绳工具, 如果你邀请我加入，0点过后，我们将可以对城镇进行袭击。不过绳子有限只能带20个英雄", this.IsConversationAgentBarber, null, 100, null);
			starter.AddPlayerLine("sms_sneak_talk_resquest", "sms_sneak_talk_resquest", "sms_sneak_talk_resquest_answer", "你是个天才，我需要你", null, ConversationResuelt, 100, null);
			starter.AddPlayerLine("sms_sneak_talk_resquest", "sms_sneak_talk_resquest", "close_window", GameTexts.FindText("sue_more_spouses_prisoner_punish_cancel", null).ToString(), null, null, 100, null);
			starter.AddDialogLine("sms_sneak_talk_resquest_answer", "sms_sneak_talk_resquest_answer", "close_window", "偷袭别人使我快乐", null, null, 100, null);
			starter.AddDialogLine("sms_sneak_talk_again", "start", "sms_sneak_talk_again_none", "我身上带有20条绳子，我们可以出发了", this.IsConversationAgentBarber2, null, 100, null);
			starter.AddPlayerLine("sms_sneak_talk_again_none", "sms_sneak_talk_again_none", "close_window", "做的好， 为你鼓掌", null, null, 100, null);
		
		}


		public bool IsConversationAgentBarber()
		{
			return this._sneaker.CharacterObject == CharacterObject.OneToOneConversationCharacter
				&& !MobileParty.MainParty.MemberRoster.Contains(this._sneaker.CharacterObject); 
		}

		public bool IsConversationAgentBarber2()
		{
			return this._sneaker.CharacterObject == CharacterObject.OneToOneConversationCharacter
				&& MobileParty.MainParty.MemberRoster.Contains(this._sneaker.CharacterObject);
		}

		public void ConversationResuelt()
		{
			if (!MobileParty.MainParty.MemberRoster.Contains(_sneaker.CharacterObject))
			{
				MobileParty.MainParty.MemberRoster.AddToCounts(_sneaker.CharacterObject, 1);
			}
			AddCompanionAction.Apply(Clan.PlayerClan, _sneaker);
		}

		public void LocationCharactersAreReadyToSpawn(Dictionary<string, int> unusedUsablePointCount)
		{
			Settlement settlement = PlayerEncounter.LocationEncounter.Settlement;
			Location locationWithId = settlement.LocationComplex.GetLocationWithId("tavern");
			if (CampaignMission.Current.Location == locationWithId && null != _sneaker && _sneaker.CurrentSettlement == settlement
				&& this._sneaker.IsAlive
				&& !MobileParty.MainParty.MemberRoster.Contains(this._sneaker.CharacterObject)
				)
			{
				this.AddVillageCenterCharacters(locationWithId);
			}
		}

		private void AddVillageCenterCharacters(Location locationWithId)
		{
			locationWithId.AddCharacter(CreateLocationCharacter());
		}

		private LocationCharacter CreateLocationCharacter()
		{
			String actionSetCode = "as_human_villager_in_tavern";
			UniqueTroopDescriptor uniqueNo = default(UniqueTroopDescriptor);
			LocationCharacter locationCharacter =  new LocationCharacter(new AgentData(new PartyAgentOrigin(null, this._sneaker.CharacterObject, -1, uniqueNo, false)).Monster(Campaign.Current.HumanMonsterSettlement).NoHorses(false), 
				SandBoxManager.Instance.AgentBehaviorManager.AddWandererBehaviors,
				 "sms_npc_sneck", false, LocationCharacter.CharacterRelations.Friendly, actionSetCode, true, false, null, false, false, false);
			return locationCharacter;
		}

	
		public void DailyTick()
        {
            if (this._sneaker == null || this._sneaker.IsDead)
            {
				Settlement randomSettlement = Settlement.All.Where(obj => obj.IsTown).GetRandomElement();

				CultureObject culture = randomSettlement.Culture;
	
				CharacterObject co =  CharacterObject.CreateFrom(culture.FemaleBeggar, true);
				CharacterObject characterObject = MBObjectManager.Instance.CreateObject<CharacterObject>();
				characterObject.Culture = co.Culture;
				characterObject.Age = (float)MBRandom.RandomInt(22, 30);
				characterObject.DefaultFormationGroup = co.DefaultFormationGroup;
				characterObject.StaticBodyPropertiesMin = co.StaticBodyPropertiesMin;
				characterObject.StaticBodyPropertiesMax = co.StaticBodyPropertiesMax;
				characterObject.IsFemale = true;
				characterObject.Level = co.Level;
				characterObject.HairTags = co.HairTags;
				characterObject.BeardTags = co.BeardTags;
				characterObject.InitializeEquipmentsOnLoad(co.AllEquipments.ToList<Equipment>());
				characterObject.Name =  co.Name;

				this._sneaker =  HeroCreator.CreateSpecialHero(characterObject, randomSettlement);
				this._sneaker.Name = new TextObject("\"偷袭者\"" + this._sneaker.Name.ToString(), null) ;
				HeroInitPropertyUtils.InitAttributeAndFouse(this._sneaker);
				HeroInitPropertyUtils.FillBattleEquipment(this._sneaker);

				randomSettlement = Hero.MainHero.CurrentSettlement;
				if (null != randomSettlement && randomSettlement.IsTown)
				{
					CharacterChangeLocation(this._sneaker, this._sneaker.CurrentSettlement, randomSettlement);
					InformationManager.DisplayMessage(new InformationMessage(this._sneaker.Name.ToString() + " 出现在" + randomSettlement.Name, Colors.Blue));
				}
				
            }
        }

        public void WeeklyTick()
        {
			if (null != this._sneaker)
			{
				this._alertCoefficientReduceWeek++;
				if (this._alertCoefficientReduceWeek >= AlertReducWeekPeriod)
				{
					AlertCoefficientReduce();
					if (this._sneaker.CurrentSettlement != null
					&& this._sneaker.IsAlive
					&& !this._sneaker.IsPrisoner
					&& null == this._sneaker.PartyBelongedTo
					)
					{
						MoveSpecialCharacter(this._sneaker.CharacterObject, this._sneaker.CurrentSettlement);

					}
					this._alertCoefficientReduceWeek = 0;
				}
			
				if ( this._sneaker.HeroState == Hero.CharacterStates.NotSpawned)
				{
					this._sneaker.ChangeState(Hero.CharacterStates.Active);

				}

				if (this._sneaker.Age >= 50)
				{
					this._sneaker.BirthDay = HeroHelper.GetRandomBirthDayForAge((float)22);
					InformationManager.DisplayMessage(new InformationMessage(this._sneaker.Name.ToString() + "变年轻了", Colors.Blue));
				}

				
			}
		
        }

		public bool MoveSpecialCharacter(CharacterObject character, Settlement startPoint)
		{
			bool result = false;
			Settlement settlement = null;
			float num = 9999f;
			foreach (Settlement current in Campaign.Current.Settlements)
			{
				if (current.IsTown && current != startPoint)
				{
					float num2 = 10000f;
					float num3;
					if (Campaign.Current.Models.MapDistanceModel.GetDistance(current, startPoint, 60f, out num3))
					{
						num2 = (num3 + 10f) * (MBRandom.RandomFloat + 0.1f);
					}
					if (num2 < num)
					{
						settlement = current;
						num = num2;
					}
				}
			}
			if (settlement != null)
			{
				this.CharacterChangeLocation(character.HeroObject, startPoint, settlement);
				
				InformationManager.DisplayMessage(new InformationMessage(this._sneaker.Name.ToString() + " 出现在" + settlement.Name, Colors.Blue));
				result = true;
			}
			return result;
		}

		public void CharacterChangeLocation(Hero hero, Settlement startLocation, Settlement endLocation)
		{
			if (startLocation != null)
			{
				LeaveSettlementAction.ApplyForCharacterOnly(hero);
			}
			EnterSettlementAction.ApplyForCharacterOnly(hero, endLocation);
			if (hero != null)
			{
				hero.SpcDaysInLocation = 0;
			}
		}
	}
}
