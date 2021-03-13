using Helpers;
using SandBox.GauntletUI;
using SueMoreSpouses.Behavior;
using SueMoreSpouses.Data;
using SueMoreSpouses.Operation;
using SueMoreSpouses.Screen.State;
using SueMoreSpouses.view;
using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SueMoreSpouses.View.Service
{
    class SpouseServiceVM : ViewModel
    {
        List<Hero> _spouses = new List<Hero>();
        MBBindingList<SpouseServiceItemVM> _spouseViews;
        Hero _selectedHero;
        SpouseServiceItemVM _currentSpouseView;
        SpouseCharacterVM _selectedCharacter;
        SpouseServiceItemVM _lastPrimaryView;
        bool _canGetPregnancy;
        bool _notPrimarySpouse;

        public SpouseServiceVM()
        {
            RefreshSpouse();
        }


        [DataSourceProperty]
        public MBBindingList<SpouseServiceItemVM> Spouses
        {
            get
            {
                return this._spouseViews;
            }
        }

        [DataSourceProperty]
        public string PregnancyText
        {
            get
            {
                return new TextObject("{=suems_table_doctor_pregnancy}Get Pregnancy", null).ToString();
            }
        }

        [DataSourceProperty]
        public string SetPrimarySpouseText
        {
            get
            {
                return new TextObject("{=suems_table_doctor_primary_spouse}Primary Spouse", null).ToString();
            }
        }

        [DataSourceProperty]
        public string DivorceText
        {
            get
            {
                return new TextObject("{=suems_table_doctor_divorce}divorce", null).ToString();
            }
        }

        [DataSourceProperty]
        public bool CanGetPregnancy
        {
            get
            {
                return this._canGetPregnancy;
            }
            set
            {
                if (value != this._canGetPregnancy)
                {
                    this._canGetPregnancy = value;
                    base.OnPropertyChanged("CanGetPregnancy");
                }
            }
        }

        [DataSourceProperty]
        public bool IsNotPrimarySpouse
        {
            get
            {
                return this._notPrimarySpouse;
            }
            set
            {
                if (value != this._notPrimarySpouse)
                {
                    this._notPrimarySpouse = value;
                    base.OnPropertyChanged("IsNotPrimarySpouse");
                }
            }
        }

        [DataSourceProperty]
        public SpouseCharacterVM SelectedCharacter
        {
            get
            {
                return this._selectedCharacter;
            }
            set
            {
                if (value != this._selectedCharacter)
                {
                    this._selectedCharacter = value;
                    base.OnPropertyChanged("SelectedCharacter");
                }
            }
        }


        public void RefreshSpouse()
        {
            this._spouses = new List<Hero>();
            if (null != this._spouseViews)
            {
                this._spouseViews.Clear();
            }
            this._spouseViews = new MBBindingList<SpouseServiceItemVM>();

            if (null != Hero.MainHero.Spouse) this._spouses.Add(Hero.MainHero.Spouse);
            Hero.MainHero.ExSpouses.ToList().ForEach((obj) =>
            {
                if (!this._spouses.Contains(obj))
                {
                    if (obj.IsAlive)
                    {
                        this._spouses.Add(obj);
                    }

                }
            });

            this._spouses.ForEach((obj) =>
            {
                if (null != obj)
                {
                    SpouseServiceItemVM vm = new SpouseServiceItemVM(obj, OnSelectedSpouse);
                    this._spouseViews.Add(vm);
                    if (obj == Hero.MainHero.Spouse)
                    {
                        this._lastPrimaryView = vm;
                    }
                }

            });

            if (this._spouses.Count > 0)
            {
                this._selectedHero = this._spouses.First();
                SpouseServiceItemVM defaultItem = this._spouseViews.First();
                defaultItem.IsSelected = true;
                OnSelectedSpouse(defaultItem);
            }
        }


        public void OnSelectedSpouse(SpouseServiceItemVM spouseItemVM)
        {
            if (spouseItemVM != this._currentSpouseView)
            {

                if (null != _currentSpouseView)
                {
                    this._currentSpouseView.IsSelected = false;
                }
                this._currentSpouseView = spouseItemVM;

                this._selectedHero = spouseItemVM.Hero;
                if (null == this.SelectedCharacter)
                {
                    this.SelectedCharacter = new SpouseCharacterVM(SpouseCharacterVM.StanceTypes.None);
                }
                this.SelectedCharacter.FillFrom(_selectedHero.CharacterObject, -1);
                this.CanGetPregnancy = !this._selectedHero.IsPregnant;
                this.IsNotPrimarySpouse = this._selectedHero != Hero.MainHero.Spouse;
            }

        }

        public void OnDivorceClick()
        {
            if (null == this.SelectedCharacter) return;
            TextObject textObject = GameTexts.FindText("sms_divorce_sure", null);
            StringHelpers.SetCharacterProperties("SUE_HERO", this._selectedHero.CharacterObject, null, textObject);
            InquiryData inquiryData = new InquiryData(textObject.ToString(), string.Empty, true, true,
                GameTexts.FindText("sms_sure", null).ToString(),
                GameTexts.FindText("sms_cancel", null).ToString(), () =>
                {
                    SpouseOperation.Divorce(this._selectedHero);
                }, null);
            InformationManager.ShowInquiry(inquiryData, false);

        }

        public void ExecuteDate()
        {
            //	CampaignMission.OpenBattleMission(PlayerEncounter.GetBattleSceneForMapPosition(MobileParty.MainParty.Position2D));
            String scenc = PlayerEncounter.GetBattleSceneForMapPosition(MobileParty.MainParty.Position2D);
            List<Hero> heros = new List<Hero>();
            heros.AddRange(this._spouses);
            heros.Add(Hero.MainHero);
            SpousesMissons.OpenDateMission(scenc, heros);
        }

        public void GetPregnancy()
        {
            if (this._selectedHero.IsPregnant)
            {
                return;
            }

            SpouseOperation.GetPregnancyForHero(Hero.MainHero, this._selectedHero);
            if (Hero.MainHero.IsFemale && !Hero.MainHero.IsPregnant)
            {
                SpouseOperation.GetPregnancyForHero(this._selectedHero, Hero.MainHero);
            }
            this.CanGetPregnancy = false;
            if (null != this._currentSpouseView) this._currentSpouseView.RefreshValues();

        }

        public void SetPrimarySpouse()
        {
            if (this._selectedHero == Hero.MainHero.Spouse)
            {
                return;
            }
            SpouseOperation.SetPrimarySpouse(this._selectedHero);
            this.IsNotPrimarySpouse = false;
            if (null != this._currentSpouseView) this._currentSpouseView.RefreshValues();
            if (null != this._lastPrimaryView) this._lastPrimaryView.RefreshValues();
            this._lastPrimaryView = this._currentSpouseView;
        }


        public void ActionStand()
        {
        
        }


        public void ExecuteFaceDetailsCreator()
        {
            TaleWorlds.Core.FaceGen.ShowDebugValues = true;
            FaceDetailsCreatorState state = Game.Current.GameStateManager.CreateState<FaceDetailsCreatorState>(new object[]
                   {
                    this._selectedHero,
                   });

            Game.Current.GameStateManager.PushState(state, 0);
        }

        public void ActionCelebrateVictory()
        {
      
        }


        public new void OnFinalize()
        {
            base.OnFinalize();
            if (null != this._selectedCharacter) this._selectedCharacter.OnFinalize();
            this._selectedCharacter = null;
        }
    }
}
