using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace SueMoreSpouses.view.sp
{
    class BattleHistorySPScoreVM : ViewModel
    {
		private string _nameText = "";

		private int _kill;

		private int _dead;

		private int _wounded;

		private int _routed;

		private int _remaining;

		private int _readyToUpgrade;

		private bool _isMainParty;

		private bool _isMainHero;

		[DataSourceProperty]
		public string NameText
		{
			get
			{
				return this._nameText;
			}
			set
			{
				if (value != this._nameText)
				{
					this._nameText = value;
					base.OnPropertyChanged("NameText");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMainHero
		{
			get
			{
				return this._isMainHero;
			}
			set
			{
				if (value != this._isMainHero)
				{
					this._isMainHero = value;
					base.OnPropertyChanged("IsMainHero");
				}
			}
		}

		[DataSourceProperty]
		public bool IsMainParty
		{
			get
			{
				return this._isMainParty;
			}
			set
			{
				if (value != this._isMainParty)
				{
					this._isMainParty = value;
					base.OnPropertyChanged("IsMainParty");
				}
			}
		}

		[DataSourceProperty]
		public int Kill
		{
			get
			{
				return this._kill;
			}
			set
			{
				if (value != this._kill)
				{
					this._kill = value;
					base.OnPropertyChanged("Kill");
				}
			}
		}

		[DataSourceProperty]
		public int Dead
		{
			get
			{
				return this._dead;
			}
			set
			{
				if (value != this._dead)
				{
					this._dead = value;
					base.OnPropertyChanged("Dead");
				}
			}
		}

		[DataSourceProperty]
		public int Wounded
		{
			get
			{
				return this._wounded;
			}
			set
			{
				if (value != this._wounded)
				{
					this._wounded = value;
					base.OnPropertyChanged("Wounded");
				}
			}
		}

		[DataSourceProperty]
		public int Routed
		{
			get
			{
				return this._routed;
			}
			set
			{
				if (value != this._routed)
				{
					this._routed = value;
					base.OnPropertyChanged("Routed");
				}
			}
		}

		[DataSourceProperty]
		public int Remaining
		{
			get
			{
				return this._remaining;
			}
			set
			{
				if (value != this._remaining)
				{
					this._remaining = value;
					base.OnPropertyChanged("Remaining");
				}
			}
		}

		[DataSourceProperty]
		public int ReadyToUpgrade
		{
			get
			{
				return this._readyToUpgrade;
			}
			set
			{
				if (value != this._readyToUpgrade)
				{
					this._readyToUpgrade = value;
					base.OnPropertyChanged("ReadyToUpgrade");
				}
			}
		}


		public BattleHistorySPScoreVM()
        {
		
		}

		public void UpdateScores(String name, int numberRemaining, int killCount, int numberWounded, int numberRouted, int numberKilled, int numberReadyToUpgrade)
		{
			this._nameText = name;
			this.Kill = killCount;
			this.Dead = numberKilled;
			this.Wounded = numberWounded;
			this.Routed = numberRouted;
			this.Remaining = numberRemaining;
			this.ReadyToUpgrade = numberReadyToUpgrade;
		}
	}
}
