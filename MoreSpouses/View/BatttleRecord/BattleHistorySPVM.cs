using SueMoreSpouses.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace SueMoreSpouses.view.sp
{
    class BattleHistorySPVM : ViewModel
    {
        SpousesBattleRecord _battleRecord;

		BattleHistorySPSideVM _attackers;
		BattleHistorySPSideVM _defenders;

		BattleHistorySPRewardVM _battleHistoryReward;

		private int _battleResultIndex = -1;
		private string _battleResult;
		private bool _isPalyerWin;

		public SpousesBattleRecord BattleRecord
		{
			get
			{
				return this._battleRecord;
			}
			set
			{
				if (value != this._battleRecord)
				{
					this._battleRecord = value;
					base.OnPropertyChangedWithValue(value, "BattleRecord");
				}
			}
		}

		[DataSourceProperty]
		public BattleHistorySPRewardVM BattleHistoryReward
		{
			get
			{
				return this._battleHistoryReward;
			}
			set
			{
				if (value != this._battleHistoryReward)
				{
					this._battleHistoryReward = value;
					base.OnPropertyChangedWithValue(value, "BattleHistoryReward");
				}
			}
		}

		[DataSourceProperty]
		public BattleHistorySPSideVM Attackers
		{
			get
			{
				return this._attackers;
			}
			set
			{
				if (value != this._attackers)
				{
					this._attackers = value;
					base.OnPropertyChangedWithValue(value, "Attackers");
				}
			}
		}

		[DataSourceProperty]
		public BattleHistorySPSideVM Defenders
		{
			get
			{
				return this._defenders;
			}
			set
			{
				if (value != this._defenders)
				{
					this._defenders = value;
					base.OnPropertyChangedWithValue(value, "Defenders");
				}
			}
		}

		[DataSourceProperty]
		public bool IsPalyerWin
		{
			get
			{
				return _isPalyerWin;
			}
			set
			{
				if (value != this._isPalyerWin)
				{
					this._isPalyerWin = value;
					base.OnPropertyChangedWithValue(value, "IsPalyerWin");
				}
			}

		}


		[DataSourceProperty]
		public int BattleResultIndex
		{
			get
			{
				return this._battleResultIndex;
			}
			set
			{
				if (value != this._battleResultIndex)
				{
					this._battleResultIndex = value;
					base.OnPropertyChangedWithValue(value, "BattleResultIndex");
				}
			}
		}

		[DataSourceProperty]
		public string BattleResult
		{
			get
			{
				return this._battleResult;
			}
			set
			{
				if (value != this._battleResult)
				{
					this._battleResult = value;
					base.OnPropertyChangedWithValue(value, "BattleResult");
				}
			}
		}

		public BattleHistorySPVM(SpousesBattleRecord battleRecord)
        {
			this._attackers = new BattleHistorySPSideVM();
			this._defenders = new BattleHistorySPSideVM();
			this.BattleHistoryReward = new BattleHistorySPRewardVM(battleRecord.RecordReward);
			SetBattleRecord(battleRecord);

		}

        public void SetBattleRecord(SpousesBattleRecord battleRecord)
        {
			this.BattleResultIndex = battleRecord.BattleResultIndex;
			this.BattleResult = ((battleRecord.BattleResultIndex == 1) ? GameTexts.FindText("str_victory", null).ToString() : GameTexts.FindText("str_defeat", null).ToString());
			this.IsPalyerWin = (battleRecord.BattleResultIndex == 1);
			this._battleRecord = battleRecord;
			this.Attackers.FillHistorySide(battleRecord.AttackerSide);
			this.Defenders.FillHistorySide(battleRecord.DefenderSide);
			this.BattleHistoryReward.FillData(battleRecord.RecordReward);
			this.RefreshValues();
		}
    }
}
