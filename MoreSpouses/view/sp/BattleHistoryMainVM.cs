using SueMoreSpouses.behavior;
using SueMoreSpouses.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Library;

namespace SueMoreSpouses.view.sp
{
    class BattleHistoryMainVM : ViewModel
    {
        MBBindingList<BattleHitoryRecordVM> _battleRecordViews;
        List<SpousesBattleRecord> _battleRecords;
        private BattleHitoryRecordVM _lastBattleRecordSelected;

        BattleHistorySPVM _historySP;

        [DataSourceProperty]
        public MBBindingList<BattleHitoryRecordVM> HistoryBattleRecords
        {
            get
            {
                return this._battleRecordViews;
            }
        }

        [DataSourceProperty]
        public BattleHistorySPVM HistorySP
        {
            get
            {
                return this._historySP;
            }
        }

        public BattleHistoryMainVM()
        {
            InitBattleRecordData();
          
        }

        public void InitBattleRecordData()
        {
            this._battleRecordViews = new MBBindingList<BattleHitoryRecordVM>();
            this._battleRecords = Campaign.Current.GetCampaignBehavior<SpousesStatsBehavior>().SpousesBattleRecords();
            this._battleRecords.ForEach(obj => {
                if (null != obj)
                {
                    this._battleRecordViews.Add(new BattleHitoryRecordVM(obj, OnBattleRecordSelected));
                }
            });
            if (this._battleRecordViews.Count > 0)
            {
                this._lastBattleRecordSelected = this._battleRecordViews.First();
                this._lastBattleRecordSelected.IsSelected = true;
                this._historySP = new BattleHistorySPVM(_lastBattleRecordSelected.GetSpousesBattleRecord);
                this.HistorySP.SetBattleRecord(_lastBattleRecordSelected.GetSpousesBattleRecord);
            }
        }

        private void OnBattleRecordSelected(BattleHitoryRecordVM selectedRecord)
        {
            if (null != _lastBattleRecordSelected)
            {
                this._lastBattleRecordSelected.IsSelected = false;
            }
            this._lastBattleRecordSelected = selectedRecord;
            this.HistorySP.SetBattleRecord(selectedRecord.GetSpousesBattleRecord);


        }

    }
}
