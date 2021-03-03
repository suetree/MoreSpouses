using Helpers;
using SandBox.GauntletUI;
using StoryMode.CharacterCreationSystem;
using SueMoreSpouses.Behavior;
using SueMoreSpouses.Data;
using SueMoreSpouses.Operation;
using SueMoreSpouses.Screen.State;
using SueMoreSpouses.view.setting;
using SueMoreSpouses.View.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade.LegacyGUI.Missions;

namespace SueMoreSpouses.view
{
    class SpouseDashboardVM : ViewModel
    {

        SpouseClanVM _parentView;
        GauntletClanScreen _parentScreen;
        MBBindingList<SpouseSettingsGroupVM> _settingGroups  ;
        List<SpouseSettingGroup> _spouseSettingGroups;

        bool _isFemaleSelected = true;
        bool _isSettingSelected = false;
        bool _isStatisticsSelected = false;

        SpouseServiceVM _spouseServiceView;
        SpousesBattleStatisticVM _spousesBattleStats;

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
        public string RecordText
        {
            get
            {
                return new TextObject("{=sms_battle_record}Battle Record", null).ToString();
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
        public SpouseServiceVM SpousesService
        {
            get
            {
                return this._spouseServiceView;
            }
        }

        [DataSourceProperty]
        public SpousesBattleStatisticVM SpousesBattleStats
        {
            get
            {
                return this._spousesBattleStats;
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
        public bool IsStatisticsSelected
        {
            get
            {
                return this._isStatisticsSelected;
            }
            set
            {
                if (value != this._isStatisticsSelected)
                {
                    this._isStatisticsSelected = value;
                    base.OnPropertyChanged("IsStatisticsSelected");
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


        public SpouseDashboardVM(SpouseClanVM parent, GauntletClanScreen parentScreen)
       {
            this._parentView = parent;
            this._parentScreen = parentScreen;
            this._settingGroups = new MBBindingList<SpouseSettingsGroupVM>();
          
            _spouseSettingGroups =  MoreSpouseSetting.Instance.GenerateSettingsProperties();
            _spouseSettingGroups.ForEach((obj) => {
                this._settingGroups.Add(new SpouseSettingsGroupVM(obj));
            });

            this._spousesBattleStats = new SpousesBattleStatisticVM(this._parentView);

            this._spouseServiceView = new SpouseServiceVM();

            this.RefreshValues();
        }

   

        public void SetSelectedCategory(int index)
        {

            this.IsFemaleDoctorSelected = false;
            this.IsSettingSelected = false;
            this.IsStatisticsSelected = false;
            if (index == 0)
            {
                this.IsFemaleDoctorSelected = true;
            }
            else if (index == 1)
            {
                this.IsSettingSelected = true;
            }else if (index == 2)
            {
                this.IsStatisticsSelected = true;
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

            //this._spouseServiceView.OnFinalize();
           

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
