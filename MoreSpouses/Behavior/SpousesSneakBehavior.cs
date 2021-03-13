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
using SueMoreSpouses.Utils;
using HarmonyLib;
using TaleWorlds.CampaignSystem.Conversation.Tags;
using TaleWorlds.Library;
using SandBox.View.Menu;
using SueMoreSpouses.view.troop;
using static TaleWorlds.CampaignSystem.Hero;

namespace SueMoreSpouses.Behavior
{

	//tavern   lordshall  prison  center
	class SpousesSneakBehavior : CampaignBehaviorBase
	{

		enum SneakType {
			LordShall, Center, Tavern, Prison
		}

	
		int _alertCoefficient = 0; //警戒系数， 随着时间和战斗变化
		private int AlertRate = 2; //系数倍率 3能开始打一场
		int _alertCoefficientReduceWeek = 0;

		CampaignGameStarter _gameStarter;

		IEnumerable<MobileParty> _parties;

		IEnumerable<Hero> _lordHerosWithOutParty;

		MobileParty _tempMainMobile;

		List<CharacterObject> _prisoners;

		private int AlertReducWeekPeriod = 1; //警戒降低，周期， 单位周

		private int SneakMaxNum = 20;

		private int EscapeAlertNum = 10; //偷袭者会逃跑的警戒值

	

		private SneakType _sneakType = SneakType.LordShall; //0=领主大厅，1=中心， 2=酒馆， 3=监狱

		private MobileParty _tempTargetParty;
		private Settlement _lastSettlement;

		public override void RegisterEvents()
		{
			CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.AddGameMenu));
			CampaignEvents.OnNewGameCreatedEvent.AddNonSerializedListener(this, new Action<CampaignGameStarter>(this.AddGameMenu));
			CampaignEvents.MapEventEnded.AddNonSerializedListener(this, new Action<MapEvent>(this.OnPlayerBattleEnd));
		}

		public override void SyncData(IDataStore dataStore)
		{
		
			dataStore.SyncData<int>("AlertCoefficient", ref this._alertCoefficient);
			dataStore.SyncData<int>("AlertCoefficientWeek", ref this._alertCoefficientReduceWeek);
		}

	



		private void OnPlayerBattleEnd(MapEvent mapEvent)
		{
            if (mapEvent.IsPlayerMapEvent)
            {
				if (null != this._tempMainMobile)
				{
					//把临时部队英雄移回主要玩家部队
					ICollection<TroopRosterElement> result = this._tempMainMobile.MemberRoster.RemoveIf(obj => obj.Character != null);
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
			
					MobileParty.MainParty.Party.Owner = Hero.MainHero;
					if (mapEvent.WinningSide == mapEvent.PlayerSide)//玩家打赢
					{
						AlertCoefficientIncrease();
						if (this._sneakType == SneakType.Prison)
						{
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
												InformationManager.DisplayMessage(new InformationMessage(ShowTextObject("{=sms_sneak_rescue_successful}Successful rescue") + character.HeroObject.Name, Colors.Green));
											}
											else
											{
												MobileParty.MainParty.AddPrisoner(character, 1);
											}
										
										}
									}
								}
								//移除当前定居点的囚犯
								RemovePrisonInSettlement();
							}
						}
						
					}
					else //玩家打输了
					{
						AlertCoefficientReduce();
						if (null != this._tempTargetParty)
						{
							if (this._tempTargetParty.PrisonRoster.Count > 0)
							{
								if (null != this._lastSettlement && this._lastSettlement.Parties.Count > 0)
								{
									ICollection<TroopRosterElement> prisonList = this._tempTargetParty.PrisonRoster.RemoveIf(obj => obj.Character != null);
									foreach (TroopRosterElement element in prisonList)
									{
										if (element.Character != MainHero.CharacterObject)
                                        {
											this._lastSettlement.Parties[0].PrisonRoster.AddToCounts(element.Character, 1);
										}
										
									}
								}
								else
								{
									this._tempTargetParty.PrisonRoster.Reset();
								}
							}
						}
					}
					this._tempTargetParty.RemoveParty();
					this._tempTargetParty = null;
					
					this._tempMainMobile.RemoveParty();
					this._tempMainMobile = null;

					this._lastSettlement = null;
				}
			}
		}

		private void RemovePrisonInSettlement()
        {

			List<PartyBase> list = new List<PartyBase>
			{
				this._lastSettlement.GetComponent<Town>().Owner
			};
				foreach (MobileParty current in this._lastSettlement.GetComponent<Town>().Owner.Settlement.Parties)
				{
					if (current.IsCommonAreaParty || current.IsGarrison)
					{
						list.Add(current.Party);
					}
				}
				foreach (PartyBase partyBase in list)
				{
					foreach (CharacterObject character in this._prisoners)
					{
						if (character.IsHero)
						{
							if (partyBase.PrisonRoster.FindIndexOfTroop(character) >= 0)
							{
							    character.HeroObject.ChangeState(CharacterStates.Active);
								partyBase.PrisonRoster.RemoveTroop(character);

							}
						}
					}	
				}
		}

		private void AlertCoefficientReduce()
		{
			this._alertCoefficient--;
			if (this._alertCoefficient < 0)
			{
				this._alertCoefficient = 0;
			}
			InformationManager.DisplayMessage(new InformationMessage(ShowTextObject("{=sms_sneak_alarm_value}Alarm Value") + ": " + this._alertCoefficient, Colors.Green));
		}

		private void AlertCoefficientIncrease()
		{
			this._alertCoefficient++;
			if (this._alertCoefficient > 10)
			{
				this._alertCoefficient = 10;
			}
			InformationManager.DisplayMessage(new InformationMessage(ShowTextObject("{=sms_sneak_alarm_value}Alarm Value") + ": " + this._alertCoefficient, Colors.Red));

			if (this._alertCoefficient >= EscapeAlertNum)
			{
				
			}
			
		}

		public void AddGameMenu(CampaignGameStarter gameStarter)
		{
			this._gameStarter = gameStarter;

			//PlayerTownVisitCampaignBehavior
			gameStarter.AddGameMenuOption("town", "sms_sneak", "{=sms_sneak_on_secret_mission}Go on a secret mission", ShowCondition, SwitchStealMenuStart, false, 3, false);
			gameStarter.AddGameMenuOption("castle", "sms_sneak", "{=sms_sneak_on_secret_mission}Go on a secret mission", ShowCondition, SwitchStealMenuStart, false, 3, false);
			
			gameStarter.AddGameMenu("sms_sneak", "{=sms_sneak_menu_describe}Attack a Lord and take him prisoner. Attack the dungeon and save your companions", new OnInitDelegate(MenuInit), GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.none, null);
			gameStarter.AddGameMenuOption("sms_sneak", "sms_sneak_lords_shall", "{=sms_sneak_attack_lord}Attack the Lord", HasLordWithOutParty, SwitchStealMenuLordWithoutParty, false, -1, false);
			//gameStarter.AddGameMenuOption("sms_sneak", "sms_sneak_party_center", "袭击部队", HasLordParty, SwitchStealMenuParty, false, -1, false);
			//gameStarter.AddGameMenuOption("sms_sneak", "sms_sneak_party_tavern", "袭击酒馆", HasTavern, BattleInTavern, false, -1, false);
			gameStarter.AddGameMenuOption("sms_sneak", "sms_sneak_party_prison", "{=sms_sneak_attack_dungeon}Attack the dungeon", HasPrison, BattleInPrison, false, -1, false);

			gameStarter.AddGameMenuOption("sms_sneak", "sms_sneak_leave", "{=sms_sneak_leave}Leave", null, Leave, true, -1, false);

		}

		private void MenuInit(MenuCallbackArgs args)
		{
			this._lastSettlement = Settlement.CurrentSettlement;
			this._parties = Settlement.CurrentSettlement.Parties.Where(obj =>  null != obj.ActualClan &&  obj.ActualClan != Clan.PlayerClan );
			this._lordHerosWithOutParty = Settlement.CurrentSettlement.HeroesWithoutParty.Where(obj => obj.CharacterObject.Occupation == Occupation.Lord && obj.Clan != Clan.PlayerClan);
			if(null != this._prisoners) this._prisoners.Clear();
			if (null != Settlement.CurrentSettlement.GetComponent<Town>())
			{
				int k = this._lastSettlement.Parties.Count;
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
			//MenuPartyInit(args);
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

			_gameStarter.AddGameMenuOption("sms_sneak_party", "sms_sneak_leave", "{=sms_sneak_leave}Leave", null, Leave, true, -1, false);
		}

		private void MenuLordWithoutPartyInit(MenuCallbackArgs args)
		{
			_gameStarter.AddGameMenu("sms_sneak_lord_whitout_party", "{=sms_sneak_attack_lord_describe}At this point, you see some lords in the hall, with a small number of troops", null, GameOverlays.MenuOverlayType.SettlementWithBoth, GameMenu.MenuFlags.none, null);
			if (null != this._lordHerosWithOutParty && this._lordHerosWithOutParty.Count() > 0)
			{
				foreach (Hero current in this._lordHerosWithOutParty)
				{
					_gameStarter.AddGameMenuOption("sms_sneak_lord_whitout_party", "sms_sneak_hero_name", current.Name.ToString(), null, BattleInLordShall, false, -1, false);
				}
			}

			_gameStarter.AddGameMenuOption("sms_sneak_lord_whitout_party", "sms_sneak_leave", "{=sms_sneak_leave}Leave", null, Leave, true, -1, false);
		}

		public bool ShowCondition(MenuCallbackArgs args)
		{
			return (Campaign.Current.IsNight);
			//return true;
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
			List<CharacterObject> list = Settlement.CurrentSettlement.GetComponent<SettlementComponent>().GetPrisonerHeroes();
			return null != Settlement.CurrentSettlement.GetComponent<Town>() && list.Count > 0;
		}

		private void BattleInTavern(MenuCallbackArgs args)
		{
			this._sneakType = SneakType.Tavern;
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
			this._sneakType = SneakType.Prison;
			int upgradeLevel = Settlement.CurrentSettlement.GetComponent<Town>().GetWallLevel();
			String scene = LocationComplex.Current.GetLocationWithId("prison").GetSceneName(upgradeLevel);
			//int num = Campaign.Current.Models.LocationModel.GetSettlementUpgradeLevel(PlayerEncounter.LocationEncounter);
			scene = "sms_prison";
			//scene = "sue_test";
		    this._tempTargetParty = MBObjectManager.Instance.CreateObject<MobileParty>("sms_prison");
			this._tempTargetParty.Party.Owner = Settlement.CurrentSettlement.OwnerClan.Leader;

			AddRandomTroopToParty(this._tempTargetParty);
			this._tempTargetParty.SetCustomName(new TextObject(Settlement.CurrentSettlement.Name + "监狱警卫队"));
		
			SelectMainPartyMember(args, () => {
				PreBattle(this._tempTargetParty);
				OpenBattleJustHero(scene, upgradeLevel);
			}, SneakMaxNum);

		}


		public void BattleInLordShall(MenuCallbackArgs args)
		{
			this._sneakType = SneakType.LordShall;
			Hero target = this._lordHerosWithOutParty.Where(obj => obj.Name.ToString() == args.Text.ToString()).ToList().GetRandomElement();
			if (null == target)
			{
				PlayerEncounter.LeaveSettlement();
				PlayerEncounter.Finish(true);
				return;
			}
			int upgradeLevel = Settlement.CurrentSettlement.GetComponent<Town>().GetWallLevel();
			String scene = LocationComplex.Current.GetLocationWithId("lordshall").GetSceneName(upgradeLevel);
			//empire_castle_keep_a_l3_interior

			this._tempTargetParty = target.Clan.CreateNewMobileParty(target);
			AddRandomTroopToParty(this._tempTargetParty);
			SelectMainPartyMember(args, () => {
				PreBattle(this._tempTargetParty);
				OpenBattleJustHero(scene, upgradeLevel);
			}, SneakMaxNum);

		}

		public void BattleInCenter(MenuCallbackArgs args)
		{
			this._sneakType = SneakType.Center;
			MobileParty targetParty = this._parties.Where(obj => obj.Name.ToString() == args.Text.ToString()).ToList().GetRandomElement();
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

	

		private bool CanChangeStatusOfTroop(CharacterObject character)
		{
			return !character.IsPlayerCharacter 
				
				&& !character.IsNotTransferableInPartyScreen
				&& !character.IsNotTransferableInHideouts
				&& character.IsHero;
		}


		private void AddRandomTroopToParty(MobileParty targetParty)
		{
			int num = 5 + AlertRate * this._alertCoefficient;
			CharacterObject character = CharacterObject.All.Where(obj => obj.IsSoldier && obj.Tier >= 4 && obj.Culture == Settlement.CurrentSettlement.Culture).ToList().GetRandomElement();
			targetParty.MemberRoster.AddToCounts(character, num);
			character = CharacterObject.All.Where(obj => obj.IsInfantry && obj.IsSoldier && obj.Tier < 4 && obj.Culture == Settlement.CurrentSettlement.Culture).ToList().GetRandomElement();
			targetParty.MemberRoster.AddToCounts(character, num);
		}

		private void SelectMainPartyMember(MenuCallbackArgs args, Action nextStep, int maxNum)
		{
			if (null == this._tempMainMobile)
			{
				this._tempMainMobile = MBObjectManager.Instance.CreateObject<MobileParty>("sms_sneak_temp_party");
			}
			else
			{
				this._tempMainMobile.MemberRoster.Reset();
			}

			int count = MobileParty.MainParty.MemberRoster.Count;
		
			TroopRoster strongestTroopRoster = TroopRoster.CreateDummyTroopRoster();
			List<Hero> includeHeros = new List<Hero>();
			includeHeros.Add(Hero.MainHero);
			
			FlattenedTroopRoster strongestAndPriorTroops = GameComponent.GetStrongestAndPriorTroops(MobileParty.MainParty, maxNum, includeHeros);
			strongestTroopRoster.Add(strongestAndPriorTroops);
			bool execueted = OpenSlelectTroops(args, strongestTroopRoster, maxNum, new Func<CharacterObject, bool>(this.CanChangeStatusOfTroop), new Action<TroopRoster>((troop) => {
				DealPatyTroop(troop);
				nextStep();
				
			}));
			if (!execueted)
			{   //如果没有执行，就走随机筛选
				TroopRoster troopRosters = MobileParty.MainParty.MemberRoster;
				List<TroopRosterElement> list = troopRosters.GetTroopRoster();
				TroopRoster battleTroopRoster = TroopRoster.CreateDummyTroopRoster();
				foreach (TroopRosterElement element in list)
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
					this._tempMainMobile.MemberRoster.AddToCounts(element.Character, 1);
				}
				else
				{
					this._tempMainMobile.MemberRoster.AddToCounts(element.Character, element.Number);
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


		private string ShowTextObject(String text)
        {
			return new TextObject(text).ToString();
        }
	}
}
