using SandBox.GauntletUI;
using SueMoreSpouses.operation;
using SueMoreSpouses.view.setting;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.LegacyGUI.Missions;

namespace SueMoreSpouses.view
{
    class SpouseServiceVM : ViewModel
    {

        SpouseClanVM _parentView;
        GauntletClanScreen _parentScreen;
        MBBindingList<SpouseSettingsGroupVM> _settingGroups  ;
        List<SpouseSettingGroup> _spouseSettingGroups;

        List<Hero> _spouses = new List<Hero>();

        MBBindingList<SpouseItemVM> _spouseViews;

        bool _isFemaleSelected = true;
        bool _isSettingSelected = false;
        Hero _selectedHero;

        SpouseItemVM _currentSpouseView;
        SpouseViewModel _selectedCharacter;

        SpouseItemVM _lastPrimaryView;

        bool _canGetPregnancy;
        bool _notPrimarySpouse;

        [DataSourceProperty]
        public string DisplayName
        {
            get
            {
                return new TextObject("{=sue_more_spouses_btn_mangager}Spouse Service", null).ToString();
            }
        }

        [DataSourceProperty]
        public string FemaleDoctorText
        {
            get
            {
                return new TextObject("{=suems_table_spouse}Spouse", null).ToString();
            }
        }

        [DataSourceProperty]
        public string SettingText
        {
            get
            {
                return new TextObject("{=suems_table_settings}Setting", null).ToString();
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
        public MBBindingList<SpouseItemVM> Spouses
        {
            get
            {
                return this._spouseViews;
            }
        }


        [DataSourceProperty]
       public MBBindingList<SpouseSettingsGroupVM> SettingGroups
        {
            get
            {
                return this._settingGroups;
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
        public SpouseViewModel SelectedCharacter
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

        [DataSourceProperty]
        public bool IsFemaleDoctorSelected
        {
            get
            {
                return this._isFemaleSelected;
            }
            set
            {
                if (value != this._isFemaleSelected)
                {
                    this._isFemaleSelected = value;
                    base.OnPropertyChanged( "IsFemaleDoctorSelected");
                }
            }
        }




        [DataSourceProperty]
        public bool IsSettingSelected
        {
            get
            {
                return this._isSettingSelected;
            }
            set
            {
                if (value != this._isSettingSelected)
                {
                    this._isSettingSelected = value;
                    base.OnPropertyChanged( "IsSettingSelected");
                }
            }
        }


        public SpouseServiceVM(SpouseClanVM parent, GauntletClanScreen parentScreen)
       {
            this._parentView = parent;
            this._parentScreen = parentScreen;
            this._settingGroups = new MBBindingList<SpouseSettingsGroupVM>();
            _spouseSettingGroups =  MoreSpouseSetting.Instance.GenerateSettingsProperties();
            _spouseSettingGroups.ForEach((obj) => {
                this._settingGroups.Add(new SpouseSettingsGroupVM(obj));
            });

            RefreshSpouse();

            this.RefreshValues();

        }

       public void RefreshSpouse()
        {
            this._spouses = new List<Hero>();
            if (null != this._spouseViews) {
                this._spouseViews.Clear();
            }
            this._spouseViews = new MBBindingList<SpouseItemVM>();
          
            if(null != Hero.MainHero.Spouse) this._spouses.Add(Hero.MainHero.Spouse);
            Hero.MainHero.ExSpouses.ToList().ForEach((obj) => {
                if (!this._spouses.Contains(obj))
                {
                    if (obj.IsAlive) {
                        this._spouses.Add(obj);
                    }
                  
                }
            });

            this._spouses.ForEach((obj) => {
                if (null != obj)
                {
                    SpouseItemVM vm = new SpouseItemVM(obj, OnSelectedSpouse);
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
                OnSelectedSpouse(this._spouseViews.First());
            }
        }


        public void OnSelectedSpouse(SpouseItemVM spouseItemVM)
        {
            if (spouseItemVM != this._currentSpouseView)
            {
                this._selectedHero = spouseItemVM.Hero;
                if (null == this.SelectedCharacter) {
                    this.SelectedCharacter = new SpouseViewModel(CharacterViewModel.StanceTypes.None);
                }
                this.SelectedCharacter.FillFrom(_selectedHero, -1);
                this.CanGetPregnancy = !this._selectedHero.IsPregnant;
                this.IsNotPrimarySpouse = this._selectedHero != Hero.MainHero.Spouse;
                spouseItemVM.IsSelected = true;
              
                if (null != _currentSpouseView)
                {
                    this._currentSpouseView.IsSelected = false;
                }
                this._currentSpouseView = spouseItemVM;
              
            }
          
        }

        public void GetPregnancy()
        {
            if (this._selectedHero.IsPregnant)
            {
                return;
            }
            SpouseOperation.GetPregnancyForHero(Hero.MainHero, this._selectedHero);
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
            if (null == this.SelectedCharacter) return;
            FieldInfo fieldInfo = this.SelectedCharacter.GetType().BaseType.GetField("_stanceIndex", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (null != fieldInfo)
            {
                fieldInfo.SetValue(this.SelectedCharacter, (int)CharacterViewModel.StanceTypes.None);
                this.SelectedCharacter.OnPropertyChanged("StanceIndex");
            }
        
        }

        public void ActionCelebrateVictory()
        {
            if (null == this.SelectedCharacter) return;
            FieldInfo fieldInfo = this.SelectedCharacter.GetType().BaseType.GetField("_stanceIndex", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (null != fieldInfo)
            {
                fieldInfo.SetValue(this.SelectedCharacter, (int)CharacterViewModel.StanceTypes.CelebrateVictory);
                this.SelectedCharacter.OnPropertyChanged("StanceIndex");
            }
        }

        public void CheckBody()
        {
            this._selectedCharacter.NonEquipment();
          
        }

        public void SetSelectedCategory(int index)
        {

            this.IsFemaleDoctorSelected = false;
            this.IsSettingSelected = false;
            if (index == 0)
            {
                this.IsFemaleDoctorSelected = true;
            }
            else if (index == 1)
            {
                this.IsSettingSelected = true;
            }
         
        }

        public void ExecuteCloseSettings()
        {
            this._parentView.CloseSettingView();
            MoreSpouseSetting.Instance.SaveSettingData();
            this.OnFinalize();
        }

        public new void OnFinalize()
        {
            base.OnFinalize();

            if(null != this._selectedCharacter) this._selectedCharacter.OnFinalize();
            this._selectedCharacter = null;

            bool flag = Game.Current != null;
            if (flag)
            {
                Game.Current.AfterTick = (Action<float>)Delegate.Remove(Game.Current.AfterTick, new Action<float>(this.AfterTick));
            }
            this._parentView = null;


         
        }

        public void AfterTick(float dt)
        {
            bool flag = this._parentView.IsHotKeyPressed("Exit");
            if (flag)
            {
                this.ExecuteCloseSettings();
            }
          
        }
    }
}
