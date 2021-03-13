using SueMoreSpouses.Behavior;
using SueMoreSpouses.Data;
using SueMoreSpouses.view.sp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Core.ViewModelCollection;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SueMoreSpouses.view
{
    class SpousesBattleStatisticVM : ViewModel
    {

        SpouseClanVM _parentView;
        BattleHistoryMainVM _historyMainVM;
        SpousesStatisticsVM _spousesStatisticsVM;
        int _tableSelectedIndex = 0;

        [DataSourceProperty]
        public bool IsStatsTableSelected
        {
            get
            {
                return this._tableSelectedIndex == 0;
            }
        }

        [DataSourceProperty]
        public string BattleStatisticText
        {
            get
            {
                return new TextObject("{=sms_battle_record_statistic}Battle Statistic", null).ToString();
            }
        }
        [DataSourceProperty]
        public string BattleHistoryText
        {
            get
            {
                return new TextObject("{=sms_battle_record_history}Battle Histroy", null).ToString();
            }
        }


        [DataSourceProperty]
        public string ClanAllDataText
        {
            get
            {
                return new TextObject("{=sms_battle_record_clear_all_data}Clear", null).ToString();
            }
        }

        [DataSourceProperty]
        public bool IsHistoryTableSelected
        {
            get
            {
                return this._tableSelectedIndex == 1;
            }
        }

        [DataSourceProperty]
        public BattleHistoryMainVM HistoryMain
        {
            get
            {
                return this._historyMainVM;
            }
        }

        [DataSourceProperty]
        public SpousesStatisticsVM SpousesStatistics
        {
            get
            {
                return this._spousesStatisticsVM;
            }
        }

        public SpousesBattleStatisticVM(SpouseClanVM parentView)
        {
            this._parentView = parentView;
            this._historyMainVM = new BattleHistoryMainVM();
            this._spousesStatisticsVM = new SpousesStatisticsVM();
        }
      

        public void SetBattleSelectedCategory(int index)
        {
            if(this._tableSelectedIndex != index)
            {
                this._tableSelectedIndex = index;
                base.OnPropertyChanged("IsStatsTableSelected");
                base.OnPropertyChanged("IsHistoryTableSelected");
            }
           
        }

        public void ClanAllRecordData()
        {
            TextObject textObject = GameTexts.FindText("sms_battle_record_clear_all_data_sure", null);
            InquiryData inquiryData = new InquiryData(textObject.ToString(), string.Empty, true, true,
                GameTexts.FindText("sms_sure", null).ToString(),
                GameTexts.FindText("sms_cancel", null).ToString(), () =>
                {
                    Campaign.Current.GetCampaignBehavior<SpousesStatsBehavior>().ClanAllData();
                    this._parentView.CloseSettingView();
                }, () => { });
            InformationManager.ShowInquiry(inquiryData, false);

        }




    }
}
