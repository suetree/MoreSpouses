using SueMoreSpouses.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.CampaignSystem.ViewModelCollection;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace SueMoreSpouses.view
{
	class SpouseStatisticsItemVM : ViewModel
	{
		private readonly SpousesHeroStatistic _spousesStats;

		private ImageIdentifierVM _visual;

		private string _name;

		[DataSourceProperty]
		public ImageIdentifierVM Visual
		{
			get
			{
				return this._visual;
			}
			set
			{
				if (value != this._visual)
				{
					this._visual = value;
					base.OnPropertyChanged("Visual");
				}
			}
		}

		[DataSourceProperty]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				if (value != this._name)
				{
					this._name = value;
					base.OnPropertyChanged("Name");
				}
			}
		}

		[DataSourceProperty]
		public string TotalKillCount
		{
			get
			{
				return  "" + this._spousesStats.TotalKillCount;
			}
			
		}

		[DataSourceProperty]
		public string MVPCount
		{
			get
			{
				return "" + this._spousesStats.MVPCount;
			}

		}

		[DataSourceProperty]
		public string ZeroCount
		{
			get
			{
				return "" + this._spousesStats.ZeroCount;
			}

		}

		[DataSourceProperty]
		public string FightCount
		{
			get
			{
				return "" + this._spousesStats.FightCount;
			}

		}

		public SpouseStatisticsItemVM(SpousesHeroStatistic spousesStats)
		{
			this._spousesStats = spousesStats;
			CharacterCode characterCode = CampaignUIHelper.GetCharacterCode(spousesStats.StatsHero.CharacterObject, false);
			this.Visual = new ImageIdentifierVM(characterCode);
			this.Name = spousesStats.StatsHero.Name.ToString();
		}
	}
}
