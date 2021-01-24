using SueMoreSpouses.data;
using SueMoreSpouses.data.sp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SueMoreSpouses.view.sp
{
    class BattleHistorySPRewardVM : ViewModel
    {
		private float _renownChange;
		private float _influenceChange;
		private float _moraleChange;
		private float _goldChange;
		private float _playerEarnedLootPercentage;

		[DataSourceProperty]
		public String RenownChange
		{
			get
			{
				return this._renownChange.ToString("F2");
			}
		}

		[DataSourceProperty]
		public String RenownChangeText
		{
			get
			{
				return new TextObject("{=sms_battle_record_history_reward_renownChange}RenownChange", null).ToString();
			}

		}

		[DataSourceProperty]
		public String InfluenceChange
		{
			get
			{
				return this._influenceChange.ToString("F2");
			}
			
		}

		[DataSourceProperty]
		public String InfluenceChangeText
		{
			get
			{
				return new TextObject("{=sms_battle_record_history_reward_influenceChange}InfluenceChange", null).ToString();
			}

		}

		[DataSourceProperty]
		public String MoraleChange
		{
			get
			{
				return this._moraleChange.ToString("F2") ; 
			}
			
		}

		[DataSourceProperty]
		public String MoraleChangeText
		{
			get
			{
				return new TextObject("{=sms_battle_record_history_reward_MoraleChange}MoraleChange", null).ToString();
			}

		}

		[DataSourceProperty]
		public String GoldChange
		{
			get
			{
				return  this._goldChange.ToString("F2");
			}
		}

		[DataSourceProperty]
		public String GoldChangeText
		{
			get
			{
				return new TextObject("{=sms_battle_record_history_reward_GoldChange}GoldChange", null).ToString();
			}
		}

		[DataSourceProperty]
		public String PlayerEarnedLootPercentage
		{
			get
			{
				return Math.Round(this._playerEarnedLootPercentage, 2) +""; 
			}
			
		}

		[DataSourceProperty]
		public String PlayerEarnedLootPercentageText
		{
			get
			{
				return new TextObject("{=sms_battle_record_history_reward_PlayerEarnedLootPercentage}PlayerEarnedLootPercentage", null).ToString();
			}

		}


		public BattleHistorySPRewardVM(SpousesBattleRecordReward battleRecordReward)
		{ 
			if(null != battleRecordReward)
			{
				FillData(battleRecordReward);
           	}
		}

		public void FillData(SpousesBattleRecordReward battleRecordReward)
		{
			if(null == battleRecordReward)
			{
				battleRecordReward = new SpousesBattleRecordReward(0, 0, 0,0 ,0);
			}
			this._renownChange = battleRecordReward.RenownChange;
			this._influenceChange = battleRecordReward.InfluenceChange;
			this._moraleChange = battleRecordReward.MoraleChange;
			this._goldChange = battleRecordReward.GoldChange;
			this._playerEarnedLootPercentage = battleRecordReward.PlayerEarnedLootPercentage;

			base.OnPropertyChanged("RenownChange");
			base.OnPropertyChanged("InfluenceChange");
			base.OnPropertyChanged("MoraleChange");
			base.OnPropertyChanged("GoldChange");
			base.OnPropertyChanged("PlayerEarnedLootPercentage");
		}
	}
}
