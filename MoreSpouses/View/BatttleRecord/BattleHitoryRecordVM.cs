
using SueMoreSpouses.Data;
using System;
using TaleWorlds.Library;

namespace SueMoreSpouses.view
{
    class BattleHitoryRecordVM : ViewModel
    {
        private readonly SpousesBattleRecord _recordData;
        private readonly Action<BattleHitoryRecordVM> _onRecordSelected;

        private bool _isSelected;

        public SpousesBattleRecord GetSpousesBattleRecord
        {
            get {

                return this._recordData;
            }
         }

        [DataSourceProperty]
        public string Name
        {
            get
            {
                return this._recordData.Name;
            }
        }

        [DataSourceProperty]
        public bool IsSelected
        {
            get
            {
                return this._isSelected;
            }
            set
            {
                if (value != this._isSelected)
                {
                    this._isSelected = value;
                    base.OnPropertyChanged("IsSelected");
                }
            }
        }

        public SpousesBattleRecord RecordData {
            get { return _recordData; } 
        }

        public BattleHitoryRecordVM(SpousesBattleRecord data, Action<BattleHitoryRecordVM> recordSelected)
		{
			this._recordData = data;
            this._onRecordSelected = recordSelected;

        }

        public void OnHistoryRecordSelected()
        {
            if (!this.IsSelected)
            {
                this.IsSelected = true;
                this._onRecordSelected(this);
            }
          
        }

    }
}
